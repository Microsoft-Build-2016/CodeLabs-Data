using Microsoft.Azure.Documents;

namespace MSCorp.AdventureWorks.Core.Domain
{
    /// <summary>
    /// A response from a repository save.
    /// </summary>
    public class SaveResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SaveResponse"/> class.
        /// </summary>
        /// <param name="identifier">The identifier.</param>
        /// <param name="document">The document.</param>
        public SaveResponse(Identifier identifier, Document document)
        {
            Identifier = identifier;
            Document = document;
        }

        /// <summary>
        /// Gets the identifier.
        /// </summary>
        public Identifier Identifier { get; private set; }
        
        /// <summary>
        /// Gets the document.
        /// </summary>
        public Document Document { get; private set; }

    }
}