using System;
using System.Globalization;
using Newtonsoft.Json;

namespace Common
{
    /// <summary>
    /// A struct representing a percentage
    /// </summary>
    public struct Percentage : IEquatable<Percentage>, IComparable<Percentage>, IComparable
    {
        private const string NumericFormat = "{0:N1}";
        private const string PercentageFormat = NumericFormat + FormatString;
        private const string FormatString = " %";

        /// <summary>
        /// Creates a new instance of a <see cref="Percentage"/> representing the given amount.
        /// </summary>
        /// <param name="percentageValue">the decimal amount of the percentage, 50m is equivalent to 50%</param>
        [JsonConstructor]
        public Percentage(decimal percentageValue) : this()
        {
            PercentageValue = Math.Round(percentageValue, 1, MidpointRounding.AwayFromZero);
        }

        /// <summary>
        /// Gets the percentage value.
        /// </summary>
        public decimal PercentageValue { get; private set; }

        /// <summary>
        /// Returns the multiplier used to reduce a value by this percentage
        /// </summary>
        [JsonIgnore]
        public decimal SubtractiveMultiplier
        {
            get { return 1 - (PercentageValue / 100); }
        }

        /// <summary>
        /// Returns the multiplier used to increase a value by this percentage
        /// </summary>
        [JsonIgnore]
        public decimal AdditiveMultiplier
        {
            get { return 1 + (PercentageValue / 100); }
        }

        /// <summary>
        /// Returns true if the percentage is zero
        /// </summary>
        public bool IsZero
        {
            get { return PercentageValue == 0; }
        }

        /// <summary>
        /// Implements the operator &lt;=.
        /// </summary>
        /// <param name="percentage1">The percentage1.</param>
        /// <param name="percentage2">The percentage2.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator <=(Percentage percentage1, Percentage percentage2)
        {
            return percentage1.LessThanOrEqual(percentage2);
        }

        /// <summary>
        /// Implements the operator &lt;.
        /// </summary>
        /// <param name="percentage1">The percentage1.</param>
        /// <param name="percentage2">The percentage2.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator <(Percentage percentage1, Percentage percentage2)
        {
            return percentage1.LessThan(percentage2);
        }

        /// <summary>
        /// Implements the operator &gt;.
        /// </summary>
        /// <param name="percentage1">The percentage1.</param>
        /// <param name="percentage2">The percentage2.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator >(Percentage percentage1, Percentage percentage2)
        {
            return percentage1.GreaterThan(percentage2);
        }

        /// <summary>
        /// Implements the operator &gt;=.
        /// </summary>
        /// <param name="percentage1">The percentage1.</param>
        /// <param name="percentage2">The percentage2.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator >=(Percentage percentage1, Percentage percentage2)
        {
            return percentage1.GreaterThanOrEqual(percentage2);
        }

        /// <summary>
        /// Lesses the than or equal.
        /// </summary>
        /// <param name="percentage">The percentage.</param>
        /// <returns></returns>
        public bool LessThanOrEqual(Percentage percentage)
        {
            return PercentageValue <= percentage.PercentageValue;
        }

        /// <summary>
        /// Lesses the than.
        /// </summary>
        /// <param name="percentage">The percentage.</param>
        /// <returns></returns>
        public bool LessThan(Percentage percentage)
        {
            return PercentageValue < percentage.PercentageValue;
        }

        /// <summary>
        /// Greaters the than or equal.
        /// </summary>
        /// <param name="percentage">The percentage.</param>
        /// <returns></returns>
        public bool GreaterThanOrEqual(Percentage percentage)
        {
            return PercentageValue >= percentage.PercentageValue;
        }

        /// <summary>
        /// Greaters the than.
        /// </summary>
        /// <param name="percentage">The percentage.</param>
        /// <returns></returns>
        public bool GreaterThan(Percentage percentage)
        {
            return PercentageValue > percentage.PercentageValue;
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="percentage1">The percentage1.</param>
        /// <param name="percentage2">The percentage2.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(Percentage percentage1, Percentage percentage2)
        {
            return !percentage1.Equals(percentage2);
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="percentage1">The percentage1.</param>
        /// <param name="percentage2">The percentage2.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(Percentage percentage1, Percentage percentage2)
        {
            return percentage1.Equals(percentage2);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        public bool Equals(Percentage other)
        {
            return PercentageValue == other.PercentageValue;
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="obj">Another object to compare to.</param>
        /// <returns>
        /// true if <paramref name="obj"/> and this instance are the same type and represent the same value; otherwise, false.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (!(obj is Percentage)) return false;
            return Equals((Percentage)obj);
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>
        /// A 32-bit signed integer that is the hash code for this instance.
        /// </returns>
        public override int GetHashCode()
        {
            return PercentageValue.GetHashCode();
        }

        /// <summary>
        /// Gets the zero.
        /// </summary>
        /// <value>The zero.</value>
        public static Percentage Zero
        {
            get { return new Percentage(0); }
        }

        /// <summary>
        /// Gets the format string.
        /// </summary>
        public static string PercentageFormatString
        {
            get { return PercentageFormat; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is over one hundred.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is over one hundred; otherwise, <c>false</c>.
        /// </value>
        [JsonIgnore]
        public bool IsOverOneHundred
        {
            get { return PercentageValue > 100; }
        }

        /// <summary>
        /// Gets a very large percentage to represent infinity when divide by zero
        /// </summary>        
        public static Percentage Max
        {
            get { return new Percentage(int.MaxValue); }
        }

        /// <summary>
        /// Gets a 100 percent
        /// </summary>
        /// <value>The one hundred.</value>
        public static Percentage OneHundred
        {
            get { return new Percentage(100); }
        }

        /// <summary>
        ///Returns the integer representation of this percentage
        /// </summary>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "integer")]
        public int ToInteger()
        {
            return (int)Math.Round(PercentageValue, 0, MidpointRounding.AwayFromZero);
        }

        /// <summary>
        /// Returns the decimal representation of this percentage
        /// </summary>
        public decimal ToDecimal()
        {
            return PercentageValue;
        }

        ///<summary>
        /// Compares the current instance with another object of the same type and returns an integer that indicates whether the current instance precedes, follows, or occurs in the same position in the sort order as the other object.
        ///</summary>
        ///<returns>
        /// A 32-bit signed integer that indicates the relative order of the objects being compared. 
        /// The return value has these meanings: 
        ///
        ///<value>Less than zero. This instance is less than <paramref name="obj" />.</value>
        ///<value>Zero. This instance is equal to <paramref name="obj" />.</value>
        ///<value>Greater than zero. This instance is greater than <paramref name="obj" />.</value>
        ///</returns>
        ///
        ///<param name="obj">
        /// An object to compare with this instance. 
        ///</param>
        ///<exception cref="System.ArgumentException">
        /// <paramref name="obj" /> is not the same type as this instance.
        ///</exception>
        ///<filterpriority>2</filterpriority>
        public int CompareTo(object obj)
        {
            if (obj is Percentage)
            {
                return CompareTo((Percentage)obj);
            }

            throw new ArgumentException("obj is not the same type as this instance.", "obj");
        }


        /// <summary>
        /// Compares this percentage with another object
        /// </summary>
        /// <param name="other">Percentage to compare</param>
        /// <returns>
        /// -1 if given percentage is greater than this percentage
        /// 0  if given percentage is equal to this percentage
        /// 1  if given percentage is less than this percentage
        /// </returns>
        public int CompareTo(Percentage other)
        {
            return PercentageValue.CompareTo(other.PercentageValue);
        }

        /// <summary>
        /// Returns the string representation of this percentage
        /// </summary>
        public override string ToString()
        {
            return PercentageFormat.FormatWith(PercentageValue);
        }

        /// <summary>
        /// Returns a numeric string representation of the <see cref="Percentage"/> object.
        /// </summary>
        public string ToNumericString()
        {
            return PercentageValue.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Parse a numeric string representing a unit of <see cref="Percentage"/>
        /// </summary>
        /// <exception cref="ArgumentException">
        /// Thrown if the string amount cannot be parsed to a <see cref="Percentage"/>
        /// </exception>
        public static Percentage Parse(string amount)
        {
            if (string.IsNullOrEmpty(amount))
            {
                return Zero;
            }

            string unformattedText = amount.Replace(FormatString, "");

            decimal parsedValue;

            if (unformattedText.TryParseDecimal(out parsedValue))
            {
                return new Percentage(parsedValue);
            }

            throw new InvalidOperationException("The amount {0} is not a valid percentage".FormatWith(amount));
        }

    }
}