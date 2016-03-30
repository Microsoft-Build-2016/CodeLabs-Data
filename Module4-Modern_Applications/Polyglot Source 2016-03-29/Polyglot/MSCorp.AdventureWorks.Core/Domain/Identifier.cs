using System;
using Common;
using Newtonsoft.Json;

namespace MSCorp.AdventureWorks.Core.Domain
{
    /// <summary>
    /// Represents a unique identifier.
    /// </summary>
    public class Identifier : IEquatable<Identifier>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Identifier"/> class.
        /// </summary>
        public Identifier()
        {
            Value = "";
            VersionNumber = VersionNumber.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Identifier"/> class.
        /// </summary>
        [JsonConstructor]
        public Identifier(string value, VersionNumber versionNumber)
        {
            Argument.CheckIfNullOrEmpty(value, "value");
            Argument.CheckIfNull(versionNumber, "VersionNumber");
            
            Value = value;
            VersionNumber = versionNumber;
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        public string Value { get; private set; }

        /// <summary>
        /// Gets the VersionNumber.
        /// </summary>
        public VersionNumber VersionNumber { get; private set; }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        public bool Equals(Identifier other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Value.Equals(other.Value) && Equals(VersionNumber, other.VersionNumber);
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Identifier) obj);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return (Value.GetHashCode() * 397) ^ (VersionNumber != null ? VersionNumber.GetHashCode() : 0);
            }
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        public static bool operator ==(Identifier left, Identifier right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        public static bool operator !=(Identifier left, Identifier right)
        {
            return !Equals(left, right);
        }
    }
}