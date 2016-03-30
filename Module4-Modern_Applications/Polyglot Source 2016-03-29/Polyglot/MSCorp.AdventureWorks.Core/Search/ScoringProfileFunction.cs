using Intergen.Common;
using Newtonsoft.Json;

namespace MSCorp.AdventureWorks.Core.Search
{
    /// <summary>
    /// A function within a scoring profile.
    /// </summary>
    public abstract class ScoringProfileFunction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScoringProfileFunction"/> class.
        /// </summary>
        protected ScoringProfileFunction(string fieldName, int boost, string interpolation)
        {
            Argument.CheckIfNullOrEmpty(fieldName, "fieldName");
            Argument.CheckIfNullOrEmpty(interpolation, "interpolation");

            FieldName = fieldName;
            Boost = boost;
            Interpolation = interpolation;
        }

        /// <summary>
        /// Gets the type.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods")]
        [JsonProperty("type")]
        public abstract string Type { get; }

        /// <summary>
        /// Gets or sets the name of the field.
        /// </summary>
        [JsonProperty("fieldName")]
        public string FieldName { get; set; }

        /// <summary>
        /// Gets or sets the boost.
        /// </summary>
        [JsonProperty("boost")]
        public int Boost { get; set; }

        /// <summary>
        /// Gets or sets the interpolation.
        /// </summary>
        [JsonProperty("interpolation")]
        public string Interpolation { get; set; }
    }
}