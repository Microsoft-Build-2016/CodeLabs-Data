namespace MSCorp.AdventureWorks.Core.Search
{
    /// <summary>
    /// An attribute which can be used for refining search results.
    /// </summary>
    public interface IRefiner
    {
        /// <summary>
        /// Gets the name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the sort value.
        /// </summary>
        int SortValue { get; }
    }
}