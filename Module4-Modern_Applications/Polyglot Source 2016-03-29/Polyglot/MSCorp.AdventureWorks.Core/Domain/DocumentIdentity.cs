namespace MSCorp.AdventureWorks.Core.Domain
{
    /// <summary>
    /// Defines the Document Identity columns
    /// </summary>
    public static class DocumentIdentity
    {
        /// <summary>
        /// ETAG
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Etag")]
        public static string Etag
        {
            get { return "_etag"; }
        }

        /// <summary>
        /// SELF Tag
        /// </summary>
        public static string Self
        {
            get { return "_self"; }
        }

        /// <summary>
        /// Attachments
        /// </summary>
        public static string Attachments
        {
            get { return "_attachments"; }
        }
    }
}