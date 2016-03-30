using System.Runtime.Serialization;
using System.Threading.Tasks;
using Common;
using Microsoft.Azure.Documents;
using MSCorp.AdventureWorks.Core.Repository;
using Newtonsoft.Json;

namespace MSCorp.AdventureWorks.Core.Domain
{
    /// <summary>
    /// Represents a customer.
    /// </summary>
    [DataContract]
    public class Customer : Document
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="Customer"/> class.
        /// </summary>
        public Customer()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Customer"/> class.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Customer(int customerKey, string emailAddress, string firstName, string lastName, Currency preferredCurrency)
        {
            Argument.CheckIfNullOrEmpty(emailAddress, "emailAddress");
            Argument.CheckIfNullOrEmpty(firstName, "firstName");
            Argument.CheckIfNullOrEmpty(lastName, "lastName");
            Argument.CheckIfNull(preferredCurrency, "preferredCurrency");

            EmailAddress = emailAddress;
            FirstName = firstName;
            LastName = lastName;
            CustomerKey = customerKey;
            PreferredCurrency = preferredCurrency;
        }

        /// <summary>
        /// Gets the email address.
        /// </summary>
        public int CustomerKey
        {
            get { return this.GetCustomValue<int>(); }
            set { this.SetCustomPropertyValue(value); }
        }

        /// <summary>
        /// Gets the preferred currency.
        /// </summary>
        public Currency PreferredCurrency
        {
            get { return this.GetCustomValue<Currency>(); }
            set { this.SetCustomPropertyValue(value); }
        }

        /// <summary>
        /// Gets the email address.
        /// </summary>
        public string EmailAddress
        {
            get { return this.GetCustomValue<string>(); }
            set { this.SetCustomPropertyValue(value); }
        }

        /// <summary>
        /// Gets the first name.
        /// </summary>
        public string FirstName
        {
            get { return this.GetCustomValue<string>(); }
            set { this.SetCustomPropertyValue(value); }
        }

        /// <summary>
        /// Gets the last name.
        /// </summary>
        public string LastName
        {
            get { return this.GetCustomValue<string>(); }
            set { this.SetCustomPropertyValue(value); }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is new.
        /// </summary>
        [JsonIgnore]
        public bool IsNew { get { return string.IsNullOrWhiteSpace(Id); } }

        /// <summary>
        /// Gets the identifier.
        /// </summary>
        [JsonIgnore]
        public Identifier Identifier
        {
            get
            {
                if (IsNew)
                {
                    return new Identifier();
                }
                return new Identifier(Id, new VersionNumber(ETag));
            }
            private set
            {
                Id = value.Value;
                SetPropertyValue(DocumentIdentity.Etag, value.VersionNumber.Value);
            }
        }

        /// <summary>
        /// Saves the <see cref="Customer"/> into the repository.
        /// </summary>
        public async Task Save(ICustomerRepository repository)
        {
            Argument.CheckIfNull(repository, "repository");
            SaveResponse response = await repository.Save(this);
            Id = response.Identifier.Value;
            SetPropertyValue(DocumentIdentity.Etag, response.Identifier.VersionNumber.Value);
        }
    }
}