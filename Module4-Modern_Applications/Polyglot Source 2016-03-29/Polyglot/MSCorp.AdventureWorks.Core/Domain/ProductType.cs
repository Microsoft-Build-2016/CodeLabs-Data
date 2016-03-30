using System;
using Common;

namespace MSCorp.AdventureWorks.Core.Domain
{
    /// <summary>
    /// Represents a Product Type. E.g. Jacket, Snowboard.
    /// </summary>
    public class ProductType : IEquatable<ProductType>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProductType"/> class.
        /// </summary>
        public ProductType(string name)
        {
            Argument.CheckIfNullOrEmpty(name, "name");
            Name =name;
        }

        /// <summary>
        /// Gets the name of the product type
        /// </summary>
        public string Name { get; private set; }

        #region Equality members

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.
        /// </returns>
        public bool Equals(ProductType other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Name, other.Name);
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" }, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ProductType)obj);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return (Name != null ? Name.GetHashCode() : 0);
        }

        /// <summary>
        /// ==s the specified left.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns></returns>
        public static bool operator ==(ProductType left, ProductType right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// !=s the specified left.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns></returns>
        public static bool operator !=(ProductType left, ProductType right)
        {
            return !Equals(left, right);
        }

        #endregion

    }
}