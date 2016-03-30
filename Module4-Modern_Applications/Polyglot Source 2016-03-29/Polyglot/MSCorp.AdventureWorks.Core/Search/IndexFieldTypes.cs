namespace MSCorp.AdventureWorks.Core.Search
{
    /// <summary>
    /// Known index field types.
    /// </summary>
    public static class IndexFieldTypes
    {
        /// <summary>
        /// The string
        /// </summary>
        public const string String = "Edm.String";

        /// <summary>
        /// The string collection
        /// </summary>
        public const string StringCollection = "Collection(Edm.String)";

        /// <summary>
        /// The int32
        /// </summary>
        public const string Int32 = "Edm.Int32";

        /// <summary>
        /// The double
        /// </summary>
        public const string Double = "Edm.Double";

        /// <summary>
        /// The boolean
        /// </summary>
        public const string Boolean = "Edm.Boolean";

        /// <summary>
        /// The date time
        /// </summary>
        public const string DateTime = "Edm.DateTimeOffset";

        /// <summary>
        /// The geography point
        /// </summary>
        public const string GeographyPoint = "Edm.GeographyPoint";
    }
}