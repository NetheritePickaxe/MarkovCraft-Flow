#nullable enable
using System.Collections.Generic;
using System.Xml.Linq;
namespace MarkovCraft.RuleGraph
{
    public static class RuleGraphCompiler
    {
        public static XElement Compile(RuleGraphData data)
        {
            return CompileNode(data.Root, data);
        }
        private static XElement CompileNode(RuleNodeData node, RuleGraphData graph)
        {
            XElement element = new XElement(node.Type);
            if (graph != null && ReferenceEquals(node, graph.Root))
            {
                if (!string.IsNullOrWhiteSpace(graph.Values))
                    element.SetAttributeValue("values", graph.Values);
                if (!string.IsNullOrWhiteSpace(graph.Symmetry))
                    element.SetAttributeValue("symmetry", graph.Symmetry);
                if (graph.Origin)
                    element.SetAttributeValue("origin", "true");
            }
            if (!string.IsNullOrWhiteSpace(node.Comment))
                element.SetAttributeValue("comment", node.Comment);
            if (node.Parameters != null)
            {
                foreach (var pair in node.Parameters)
                {
                    if (pair.Value == null) continue;
                    string value = pair.Value.ToString();
                    if (string.IsNullOrWhiteSpace(value)) continue;
                    element.SetAttributeValue(pair.Key, value);
                }
            }
            if (node.Rules != null)
            {
                foreach (var rule in node.Rules)
                {
                    element.Add(CompileRule(rule));
                }
            }
            if (node.Children != null)
            {
                foreach (var child in node.Children)
                {
                    element.Add(CompileNode(child, null));
                }
            }
            return element;
        }
        private static XElement CompileRule(RuleEntry rule)
        {
            XElement element = new XElement("rule");
            SetAttributeIfPresent(element, "in", rule.In);
            SetAttributeIfPresent(element, "out", rule.Out);
            SetAttributeIfPresent(element, "fin", rule.FileIn);
            SetAttributeIfPresent(element, "fout", rule.FileOut);
            SetAttributeIfPresent(element, "file", rule.File);
            SetAttributeIfPresent(element, "legend", rule.Legend);
            SetAttributeIfPresent(element, "symmetry", rule.Symmetry);
            if (rule.Probability != 1.0)
                element.SetAttributeValue("p", rule.Probability.ToString(System.Globalization.CultureInfo.InvariantCulture));
            return element;
        }
        private static void SetAttributeIfPresent(XElement element, string name, string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
                element.SetAttributeValue(name, value);
        }
    }
}
