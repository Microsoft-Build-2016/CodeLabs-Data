using System;
using Intergen.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MSCorp.AdventureWorks.Core.Search
{
    /// <summary>
    /// A magnitude based scoring profile.
    /// </summary>
    public class FreshnessScoringProfileFunction : ScoringProfileFunction
    {
        private readonly string _boostingDuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="FreshnessScoringProfileFunction" /> class.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="boost">The boost.</param>
        /// <param name="interpolation">The interpolation.</param>
        /// <param name="boostingDuration">Duration of the boosting.</param>
        public FreshnessScoringProfileFunction(string fieldName, int boost, string interpolation, string boostingDuration) : base(fieldName, boost, interpolation)
        {
            _boostingDuration = boostingDuration;
        }

        /// <summary>
        /// Gets the type.
        /// </summary>
        [JsonProperty("type")]
        public override string Type { get { return "freshness"; } }

        /// <summary>
        /// Gets the freshness.
        /// </summary>
        [JsonProperty("freshness")]
        public JObject Freshness 
        { 
            get
            {
                var freshness = new
                {
                    boostingDuration = _boostingDuration
                };
                return JObject.FromObject(freshness);
            }
        }
    }
}