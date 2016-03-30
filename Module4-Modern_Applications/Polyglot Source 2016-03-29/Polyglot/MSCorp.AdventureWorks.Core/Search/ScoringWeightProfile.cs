using Intergen.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MSCorp.AdventureWorks.Core.Search
{
    /// <summary>
    /// A scoring profile for adjusting the order of search results.
    /// </summary>
    public class ScoringWeightProfile : IProfileEntry
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScoringWeightProfile"/> class.
        /// </summary>
        public ScoringWeightProfile(string name, bool isDefault)
        {
            Argument.CheckIfNullOrEmpty(name, "name");
            Name = name;
            IsDefault = isDefault;
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this is a default profile.
        /// </summary>
        [JsonIgnore]
        public bool IsDefault { get; private set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic"), JsonProperty("text")]
        public JObject Text
        {
            get
            {
                return JObject.FromObject(new NameAndDescriptionWeightings());
            }
        }

    }

    public class NameAndDescriptionWeightings
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic"), JsonProperty("weights")]
        public JObject Weight
        {
            get
            {
                var weights = new
                {
                    Name = 10,
                    Description = 8
                };

                return JObject.FromObject(weights);
            }

        }

    }
}