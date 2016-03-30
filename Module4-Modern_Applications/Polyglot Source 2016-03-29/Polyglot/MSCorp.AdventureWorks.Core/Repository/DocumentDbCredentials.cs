using System;
using System.Security;
using Common;

namespace MSCorp.AdventureWorks.Core.Repository
{
    /// <summary>
    /// A set of credentials to access the a resource
    /// </summary>
    public class DocumentDbCredentials : IDisposable
    {
        private readonly string _endpointUrl;
        private readonly SecureString _authorizationKey;

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentDbCredentials"/> class.
        /// </summary>
        public DocumentDbCredentials(string endpointUrl, SecureString authorizationKey)
        {
            Argument.CheckIfNull(authorizationKey, "authorizationKey");
            Argument.CheckIfNull(endpointUrl, "endpointUrl");
            
            _endpointUrl = endpointUrl;
            _authorizationKey = authorizationKey;
        }

        /// <summary>
        /// Gets the endpoint URL for the document service
        /// </summary>
        public string EndpointUrl
        {
            get { return _endpointUrl; }
        }

        /// <summary>
        /// Gets the authorization key.
        /// </summary>
        public SecureString AuthorizationKey
        {
            get { return _authorizationKey; }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_authorizationKey != null)
                {
                    _authorizationKey.Dispose();
                }

            }
        }
    }
}