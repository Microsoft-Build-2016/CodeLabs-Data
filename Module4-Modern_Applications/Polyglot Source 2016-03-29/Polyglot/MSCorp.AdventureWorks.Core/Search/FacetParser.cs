using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Intergen.Common;
using Newtonsoft.Json.Linq;

namespace MSCorp.AdventureWorks.Core.Search
{
    /// <summary>
    /// Queries a search service response and retrieves a list of options for use in a refiner.
    /// </summary>
    public class FacetParser
    {
        private readonly JObject _response;

        /// <summary>
        /// Initializes a new instance of the <see cref="FacetParser"/> class.
        /// </summary>
        public FacetParser(JObject response)
        {
            Argument.CheckIfNull(response, "response");
            _response = response;
        }

        /// <summary>
        /// Gets the options to use for refinement.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public JToken[] Parse(string key)
        {
            Argument.CheckIfNullOrEmpty(key, "key");

            List<JToken> newOptions = new List<JToken>();
            try
            {
                JToken facet = _response["facets"][key];
                JEnumerable<JToken> facetResults = facet.Children();
                newOptions.AddRange(facetResults);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Unable to parse document for facet of type {0}.  Data was: {1}; Error was: {2}", key, _response, e.Message);
                return new JToken[0];
            }
                    
            return newOptions.ToArray();
        }
    }
}