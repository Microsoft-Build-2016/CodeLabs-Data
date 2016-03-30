using Common;

namespace MSCorp.AdventureWorks.Core.Domain
{
    /// <summary>
    /// Rerepsents the Product Textual representation
    /// </summary>
    public class ProductText
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; private set; }
        
        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductText"/> class.
        /// </summary>
        public ProductText(string name, string description)
        {
            Argument.CheckNullEmptyOrLength(description, "description", 1024);
            Argument.CheckNullEmptyOrLength(name, "name", 250);

            Name = name;
            Description = description;
        }
    }
}