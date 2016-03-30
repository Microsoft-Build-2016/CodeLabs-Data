using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Common;
using Microsoft.Azure.Documents;
using Newtonsoft.Json.Linq;

namespace MSCorp.AdventureWorks.Core.Domain
{
    /// <summary>
    /// Extension methods for documents.
    /// </summary>
    public static class DocumentExtensionMethods
    {
        /// <summary>
        /// Gets the custom value.
        /// </summary>
        public static T GetCustomValue<T>(this Resource document, [CallerMemberName] string propertyName = null)
        {
            Argument.CheckIfNull(document, "document");
            Argument.CheckIfNull(propertyName, "propertyName");
            return document.GetPropertyValue<T>(propertyName);
        }

        /// <summary>
        /// Sets the custom value.
        /// </summary>
        public static void SetCustomPropertyValue(this Resource document, object valueToSet, [CallerMemberName] string propertyName = null)
        {
            Argument.CheckIfNull(document, "document");
            Argument.CheckIfNull(propertyName, "propertyName");
            document.SetPropertyValue(propertyName, valueToSet);
        }

        /// <summary>
        /// Returns a JArray from a collection of <see cref="Document"/>s, based on the ToString() representation of each document.
        /// </summary>
        public static JArray ToJArray<T>(this IEnumerable<T> documents)
        {
            JObject[] content = documents
                            .Select(doc => JObject.Parse(doc.ToString()))
                            .ToArray();
            JArray jArray = new JArray(content);
            return jArray;
        }
    }
}