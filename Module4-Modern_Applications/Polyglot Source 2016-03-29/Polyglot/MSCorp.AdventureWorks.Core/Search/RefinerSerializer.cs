using System.Collections.Generic;
using Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MSCorp.AdventureWorks.Core.Search
{
    /// <summary>
    /// Serializes and deserializes an attribute which can be used for refining search results.
    /// </summary>
    public static class RefinerSerializer
    {
        /// <summary>
        /// Serializes the specified refiner attribute.
        /// </summary>
        public static string Serialize(IRefiner refiner)
        {
            Argument.CheckIfNull(refiner, "refiner");
            string jsonText = JsonConvert.SerializeObject(new {name = refiner.Name, sortValue=refiner.SortValue});
            return jsonText;
        }

        /// <summary>
        /// Deserializes the specified text.
        /// </summary>
        public static KeyValuePair<string, int> Deserialize(string text)
        {
            Argument.CheckIfNullOrEmpty(text, "text");
            JObject jObject = JObject.Parse(text);
            return new KeyValuePair<string, int>(jObject["name"].ToObject<string>(), jObject["sortValue"].ToObject<int>());
        }
    }
}