using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using Common;
using Microsoft.Azure.Search.Models;

namespace MSCorp.AdventureWorks.Core.Search
{
    /// <summary>
    /// Creates a Json representation of the index schema as required for the search service.
    /// </summary>
    public static class SearchSchemaGenerator
    {
        public const string SuggesterName = "Suggester";
        /// <summary>
        /// Creates the schema.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public static Index CreateSchema<T>() where T : ISearchIndexEntry
        {
            return CreateSchema(typeof (T));
        }

        /// <summary>
        /// Creates the schema.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public static Index CreateSchemaWithScoring<T>(params ScoringProfile[] scoringProfiles) where T : ISearchIndexEntry
        {
            return CreateSchema(typeof (T), scoringProfiles);
        }

        /// <summary>
        /// Creates the schema.
        /// </summary>
        public static Index CreateSchema(Type type, params ScoringProfile[] scoringProfiles)
        {
            Argument.CheckIfNull(type, "type");
            Argument.CheckIfNull(scoringProfiles, "scoringProfiles");


            Index index = new Index()
            {
                Fields = LoadFieldDefinitions(type),
                ScoringProfiles = scoringProfiles.ToList(),
                Suggesters = LoadSuggestors(type)
            };

            return index;
        }

        private static List<Suggester> LoadSuggestors(Type type)
        {
            Suggester suggester = new Suggester
            {
                Name = SuggesterName,
                SearchMode = SuggesterSearchMode.AnalyzingInfixMatching,
                SourceFields = new List<string>()
            };

            IEnumerable<PropertyInfo> properties =
                type.GetProperties()
                    .Where(prop => prop.IsDefined(typeof (IndexFieldAttribute), true))
                    .ToList();

            foreach (PropertyInfo property in properties)
            {
                IndexFieldAttribute fieldAttribute = property.GetCustomAttributes(typeof(IndexFieldAttribute)).OfType<IndexFieldAttribute>().Single();
                if (fieldAttribute.IsSuggestable)
                {
                    suggester.SourceFields.Add(property.Name);
                }
            }
            return suggester.SourceFields.Count > 0 ? new List<Suggester> {suggester} : null;
        } 
        private static List<Field> LoadFieldDefinitions(Type type)
        {
            List<Field> fieldList = new List<Field>();

            IEnumerable<PropertyInfo> properties =
                type.GetProperties()
                    .Where(prop => prop.IsDefined(typeof (IndexFieldAttribute), true))
                    .ToList();

            foreach (PropertyInfo property in properties)
            {
                var field = CreateField(property);
                fieldList.Add(field);
            }
            return fieldList;
        }

        private static Field CreateField(PropertyInfo property)
        {


            DataMemberAttribute memberAttribute = property.GetCustomAttributes(typeof(DataMemberAttribute)).OfType<DataMemberAttribute>().Single();
            IndexFieldAttribute fieldAttribute = property.GetCustomAttributes(typeof(IndexFieldAttribute)).OfType<IndexFieldAttribute>().Single();
            // Add name and type.  These should always be specified.
            string name = (string.IsNullOrWhiteSpace(memberAttribute.Name)) ? property.Name : memberAttribute.Name;

            Field field = new Field()
            {
                Name = name,
                Type = fieldAttribute.DataType,
                IsFacetable = fieldAttribute.IsFacetable,
                IsFilterable = fieldAttribute.IsFilterable,
                IsKey = fieldAttribute.IsKey,
                IsRetrievable = fieldAttribute.IsRetrievable,
                IsSearchable = fieldAttribute.IsSearchable,
                IsSortable = fieldAttribute.IsSortable
            };

            return field;
        }
    }
}