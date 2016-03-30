using Newtonsoft.Json;

namespace Common
{
    /// <summary>
    /// A version number.
    /// </summary>
    public class VersionNumber
    {
        private static readonly VersionNumber InternalEmpty = new VersionNumber(string.Empty, true);

        public VersionNumber() : this(string.Empty, true)
        {
        }

        private VersionNumber(string value, bool isEmpty)
        {
            Value = value;
            IsEmpty = isEmpty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VersionNumber"/> class.
        /// </summary>
        public VersionNumber(string value)
        {
            Argument.CheckIfNullOrEmpty(value, "value");
            Value = value;
        }

        /// <summary>
        /// Gets the empty.
        /// </summary>
        public static VersionNumber Empty
        {
            get { return InternalEmpty; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is empty.
        /// </summary>
        [JsonIgnore]
        public bool IsEmpty { get; private set; }

        /// <summary>
        /// Gets the value.
        /// </summary>
        public string Value { get; private set; }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        /// 	<c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (obj.GetType() != typeof(VersionNumber))
            {
                return false;
            }
            return Equals((VersionNumber)obj);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return (Value != null ? Value.GetHashCode() : 0);
        }

        /// <summary>
        /// Equalses the specified other.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns></returns>
        public bool Equals(VersionNumber other)
        {
            Argument.CheckIfNull(other, "other");

            return Equals(other.Value, Value);
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(VersionNumber left, VersionNumber right)
        {
            Argument.CheckIfNull(right, "right");
            Argument.CheckIfNull(left, "left");

            return left.Equals(right);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(VersionNumber left, VersionNumber right)
        {
            Argument.CheckIfNull(right, "right");
            Argument.CheckIfNull(left, "left");

            return !left.Equals(right);
        }
    }
}