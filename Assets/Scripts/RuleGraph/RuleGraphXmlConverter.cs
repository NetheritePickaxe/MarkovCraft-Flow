#nullable enable
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Linq;
namespace MarkovCraft.RuleGraph
{
    public static class RuleGraphXmlConverter
    {
        private static readonly HashSet<string> TopLevelAttributes = new HashSet<string> { "values", "symmetry", "origin" };
        public static RuleGraphData Convert(XElement root)
        {
            var data = new RuleGraphData();
            data.Values = root.Attribute("values")?.Value ?? string.Empty;
            data.Symmetry = root.Attribute("symmetry")?.Value ?? string.Empty;
            if (bool.TryParse(root.Attribute("origin")?.Value, out bool origin))
                data.Origin = origin;
            data.Root = ConvertNode(root);
            return data;
        }
        private static RuleNodeData ConvertNode(XElement element)
        {
            var node = new RuleNodeData { Type = element.Name.LocalName };
            foreach (var attr in element.Attributes())
            {
                string name = attr.Name.LocalName;
                if (TopLevelAttributes.Contains(name)) continue;
                node.Parameters[name] = attr.Value;
            }
            foreach (var child in element.Elements())
            {
                if (child.Name.LocalName == "rule")
                {
                    node.Rules.Add(ConvertRule(child));
                }
                else
                {
                    node.Children.Add(ConvertNode(child));
                }
            }
            return node;
        }
        private static RuleEntry ConvertRule(XElement element)
        {
            var rule = new RuleEntry();
            rule.In = element.Attribute("in")?.Value ?? string.Empty;
            rule.Out = element.Attribute("out")?.Value ?? string.Empty;
            rule.FileIn = element.Attribute("fin")?.Value ?? string.Empty;
            rule.FileOut = element.Attribute("fout")?.Value ?? string.Empty;
            rule.File = element.Attribute("file")?.Value ?? string.Empty;
            rule.Legend = element.Attribute("legend")?.Value ?? string.Empty;
            rule.Symmetry = element.Attribute("symmetry")?.Value ?? string.Empty;
            if (double.TryParse(element.Attribute("p")?.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out double p))
                rule.Probability = p;
            return rule;
        }
    }
}
