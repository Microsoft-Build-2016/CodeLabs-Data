using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MSCorp.AdventureWorks.Core.Search
{
    /// <summary>
    /// A magnitude based scoring profile.
    /// </summary>
    public class MagnitudeScoringProfileFunction : ScoringProfileFunction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MagnitudeScoringProfileFunction" /> class.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="boost">The boost.</param>
        /// <param name="interpolation">The interpolation.</param>
        /// <param name="boostingRangeStart">The boosting range start.</param>
        /// <param name="boostingRangeEnd">The boosting range end.</param>
        /// <param name="constantBoostBeyondRange">if set to <c>true</c> [constant boost beyond range].</param>
        public MagnitudeScoringProfileFunction(string fieldName, int boost, string interpolation, int boostingRangeStart, int boostingRangeEnd, bool constantBoostBeyondRange) : base(fieldName, boost, interpolation)
        {
            BoostingRangeStart = boostingRangeStart;
            BoostingRangeEnd = boostingRangeEnd;
            ConstantBoostBeyondRange = constantBoostBeyondRange;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MagnitudeScoringProfileFunction" /> class.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="boost">The boost.</param>
        /// <param name="interpolation">The interpolation.</param>
        /// <param name="boostingRangeStart">The boosting range start.</param>
        /// <param name="boostingRangeEnd">The boosting range end.</param>
        public MagnitudeScoringProfileFunction(string fieldName, int boost, string interpolation, int boostingRangeStart, int boostingRangeEnd) : this(fieldName, boost, interpolation, boostingRangeStart, boostingRangeEnd, false)
        {
        }

        /// <summary>
        /// Gets the type.
        /// </summary>
        [JsonProperty("type")]
        public override string Type { get { return "magnitude"; } }

        /// <summary>
        /// Gets or sets the boosting range start.
        /// </summary>
        [JsonIgnore]
        public int BoostingRangeStart { get; set; }

        /// <summary>
        /// Gets or sets the boosting range end.
        /// </summary>
        [JsonIgnore]
        public int BoostingRangeEnd { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether [constant boost beyond range].
        /// </summary>
        [JsonIgnore]
        public bool ConstantBoostBeyondRange { get; set; }

        /// <summary>
        /// Gets the magnitude.
        /// </summary>
        [JsonProperty("magnitude")]
        public JObject Magnitude 
        { 
            get
            {
                var magnitude = new
                    {
                        boostingRangeStart = BoostingRangeStart, 
                        boostingRangeEnd = BoostingRangeEnd,
                        constantBoostBeyondRange = ConstantBoostBeyondRange
                    };
                return JObject.FromObject(magnitude);
            }
        }
    }
}