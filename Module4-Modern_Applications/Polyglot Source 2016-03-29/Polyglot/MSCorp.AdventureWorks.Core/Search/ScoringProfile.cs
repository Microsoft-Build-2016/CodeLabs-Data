using System.Collections.Generic;
using Intergen.Common;
using Newtonsoft.Json;

namespace MSCorp.AdventureWorks.Core.Search
{
    /// <summary>
    /// A scoring profile for adjusting the order of search results.
    /// </summary>
    public class ScoringProfile : IProfileEntry
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScoringProfile"/> class.
        /// </summary>
        public ScoringProfile(string name) : this(name, false) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScoringProfile"/> class.
        /// </summary>
        public ScoringProfile(string name, bool isDefault)
        {
            Argument.CheckIfNullOrEmpty(name, "name");
            Name = name;
            IsDefault = isDefault;
            Functions = new List<ScoringProfileFunction>();
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

        /// <summary>
        /// Gets or sets the functions.
        /// </summary>
        [JsonProperty("functions")]
        public IEnumerable<ScoringProfileFunction> Functions { get; set; }
    }
}