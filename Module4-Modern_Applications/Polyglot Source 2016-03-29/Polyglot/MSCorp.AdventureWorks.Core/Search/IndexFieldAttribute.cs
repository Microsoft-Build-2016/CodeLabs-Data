using Microsoft.Azure.Search.Models;
using System;
using System.Linq;

namespace MSCorp.AdventureWorks.Core.Search
{
    /// <summary>
    /// Marks a property as an index field.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class IndexFieldAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IndexFieldAttribute"/> class.
        /// </summary>
        public IndexFieldAttribute()
        {
            FieldType = SearchDataType.String;
            IsSearchable = true;
            IsFilterable = true;
            IsSortable = true;
            IsRetrievable = true;
            IsFacetable = true;
        }

        /// <summary>
        /// Gets the type of the field.
        /// </summary>
        public SearchDataType FieldType { get; set; }
        public bool IsFacetable { get; set; }
        public bool IsFilterable { get; set; }
        public bool IsRetrievable { get; set; }


        private bool _isSearchable;

        public bool IsSearchable
        {
            get
            {
                if (FieldType != SearchDataType.StringCollection && FieldType != SearchDataType.String)
                {
                    return false;
                }
                return _isSearchable;
            }
            set { _isSearchable = value; }
        }
        public bool IsKey { get; set; }

        private bool _isSortable;

        public bool IsSortable
        {
            get
            {
                if (CollectionTypes.Contains(FieldType))
                {
                    return false;
                }
                return _isSortable;
            }
            set { _isSortable = value; }
        }

        public bool IsSuggestable { get; set; }

        public static string Key
        {
            get { return "type"; }
        }

        public DataType DataType
        {
            get
            {
                switch (FieldType)
                {
                    case SearchDataType.Boolean:
                        return DataType.Boolean;
                    case SearchDataType.BooleanCollection:
                        return DataType.Collection(DataType.Boolean);
                    case SearchDataType.DateTimeOffset:
                        return DataType.DateTimeOffset;
                    case SearchDataType.DateTimeOffsetCollection:
                        return DataType.Collection(DataType.DateTimeOffset);
                    case SearchDataType.Double:
                        return DataType.Double;
                    case SearchDataType.DoubleCollection:
                        return DataType.Collection(DataType.Double);
                    case SearchDataType.GeographyPoint:
                        return DataType.GeographyPoint;
                    case SearchDataType.GeographyPointCollection:
                        return DataType.Collection(DataType.GeographyPoint);
                    case SearchDataType.Int32:
                        return DataType.Int32;
                    case SearchDataType.Int32Collection:
                        return DataType.Collection(DataType.Int32);
                    case SearchDataType.Int64:
                        return DataType.Int64;
                    case SearchDataType.Int64Collection:
                        return DataType.Collection(DataType.Int64);
                    case SearchDataType.String:
                        return DataType.String;
                    case SearchDataType.StringCollection:
                        return DataType.Collection(DataType.String);
                    default:
                        return DataType.String;
                }
            }
        }

        private static SearchDataType[] CollectionTypes = { SearchDataType.BooleanCollection, SearchDataType.DateTimeOffsetCollection, SearchDataType.DoubleCollection, SearchDataType.GeographyPointCollection, SearchDataType.Int32Collection, SearchDataType.Int64Collection, SearchDataType.StringCollection };

    }

    public enum SearchDataType
    {
        Boolean,
        DateTimeOffset,
        Double,
        GeographyPoint,
        Int32,
        Int64,
        String,
        BooleanCollection,
        DateTimeOffsetCollection,
        DoubleCollection,
        GeographyPointCollection,
        Int32Collection,
        Int64Collection,
        StringCollection
    }
}