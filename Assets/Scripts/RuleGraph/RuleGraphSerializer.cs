#nullable enable
using System.IO;
using Newtonsoft.Json;
namespace MarkovCraft.RuleGraph
{
    public static class RuleGraphSerializer
    {
        private static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate,
            ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver()
        };
        public static RuleGraphData LoadFromFile(string path)
        {
            string json = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<RuleGraphData>(json, Settings) ?? new RuleGraphData();
        }
        public static RuleGraphData LoadFromJson(string json)
        {
            return JsonConvert.DeserializeObject<RuleGraphData>(json, Settings) ?? new RuleGraphData();
        }
        public static void SaveToFile(RuleGraphData data, string path)
        {
            string json = JsonConvert.SerializeObject(data, Settings);
            File.WriteAllText(path, json);
        }
        public static string ToJson(RuleGraphData data)
        {
            return JsonConvert.SerializeObject(data, Settings);
        }
    }
}
