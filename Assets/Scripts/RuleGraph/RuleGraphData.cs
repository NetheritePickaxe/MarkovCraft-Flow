#nullable enable
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
namespace MarkovCraft.RuleGraph
{
    [Serializable]
    public class RuleGraphData
    {
        [JsonProperty("metadata")]
        public RuleGraphMetadata Metadata { get; set; } = new RuleGraphMetadata();
        [JsonProperty("values")]
        public string Values { get; set; } = string.Empty;
        [JsonProperty("symmetry")]
        public string Symmetry { get; set; } = string.Empty;
        [JsonProperty("origin")]
        public bool Origin { get; set; } = false;
        [JsonProperty("palette")]
        public List<PaletteEntry> Palette { get; set; } = new List<PaletteEntry>();
        [JsonProperty("root")]
        public RuleNodeData Root { get; set; } = new RuleNodeData();
    }
    [Serializable]
    public class RuleGraphMetadata
    {
        [JsonProperty("name")]
        public string Name { get; set; } = "Untitled";
        [JsonProperty("description")]
        public string Description { get; set; } = string.Empty;
        [JsonProperty("author")]
        public string Author { get; set; } = string.Empty;
        [JsonProperty("version")]
        public string Version { get; set; } = "1.0.0";
        [JsonProperty("tags")]
        public List<string> Tags { get; set; } = new List<string>();
    }
    [Serializable]
    public class PaletteEntry
    {
        [JsonProperty("symbol")]
        public string Symbol { get; set; } = string.Empty;
        [JsonProperty("color")]
        public string Color { get; set; } = "#FFFFFF";
        [JsonProperty("blockState")]
        public string BlockState { get; set; } = string.Empty;
        [JsonProperty("description")]
        public string Description { get; set; } = string.Empty;
    }
    [Serializable]
    public class RuleNodeData
    {
        [JsonProperty("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString("N");
        [JsonProperty("type")]
        public string Type { get; set; } = "sequence";
        [JsonProperty("comment")]
        public string Comment { get; set; } = string.Empty;
        [JsonProperty("parameters")]
        public Dictionary<string, object> Parameters { get; set; } = new Dictionary<string, object>();
        [JsonProperty("rules")]
        public List<RuleEntry> Rules { get; set; } = new List<RuleEntry>();
        [JsonProperty("children")]
        public List<RuleNodeData> Children { get; set; } = new List<RuleNodeData>();
    }
    [Serializable]
    public class RuleEntry
    {
        [JsonProperty("in")]
        public string In { get; set; } = string.Empty;
        [JsonProperty("out")]
        public string Out { get; set; } = string.Empty;
        [JsonProperty("fin")]
        public string FileIn { get; set; } = string.Empty;
        [JsonProperty("fout")]
        public string FileOut { get; set; } = string.Empty;
        [JsonProperty("file")]
        public string File { get; set; } = string.Empty;
        [JsonProperty("legend")]
        public string Legend { get; set; } = string.Empty;
        [JsonProperty("symmetry")]
        public string Symmetry { get; set; } = string.Empty;
        [JsonProperty("p")]
        public double Probability { get; set; } = 1.0;
    }
}
