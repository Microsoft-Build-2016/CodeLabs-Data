namespace MSCorp.AdventureWorks.Core.Search
{
    public interface IProfileEntry
    {
        bool IsDefault { get; }

        string Name { get; }
    }
}