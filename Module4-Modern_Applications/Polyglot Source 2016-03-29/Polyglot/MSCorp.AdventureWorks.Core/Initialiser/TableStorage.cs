using Common;

namespace MSCorp.AdventureWorks.Core.Initialiser
{
    /// <summary>
    /// Table storage
    /// </summary>
    public class TableStorage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableStorage"/> class.
        /// </summary>
        public TableStorage(string name)
        {
            Argument.CheckIfNull(name, "Name");
            Name = name;
        }

        /// <summary>
        /// Gets the name of the table.
        /// </summary>
        public string Name { get; private set; }
    }
}