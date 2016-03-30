using System;
using System.Globalization;

namespace Common
{
    /// <summary>
    /// A struct representing a Gst rate
    /// </summary>
    public struct Gst : IEquatable<Gst>, IComparable<Gst>, IComparable
    {
        private readonly decimal _percentage;
        private bool _isNone;
        private const string GstFormat = "{0:N1}" + FormatString;
        private const string FormatString = " %";

        /// <summary>
        /// Creates a new instance of a <see cref="Gst"/> representing the given amount.
        /// </summary>
        /// <param name="percentage">the decimal amount of the percentage, 50m is equivalent to 50%</param>
        public Gst(decimal percentage)
        {
            _percentage = Math.Round(percentage, 1, MidpointRounding.AwayFromZero);
            _isNone = false;
        }

        /// <summary>
        /// Returns true if the percentage is zero
        /// </summary>
        public bool IsZero
        {
            get { return _percentage == 0; }
        }

        /// <summary>
        /// Implements the operator &lt;=.
        /// </summary>
        /// <param name="percentage1">The Gst percentage1.</param>
        /// <param name="percentage2">The Gst percentage2.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator <=(Gst percentage1, Gst percentage2)
        {
            return percentage1.LessThanOrEqual(percentage2);
        }

        /// <summary>
        /// Returns the multiplier used to add a value by this Gst
        /// If GST is .125 then will return 1.125
        /// </summary>
        public decimal Multiplier
        {
            get { return 1 + GstFraction; }
        }

        /// <summary>
        /// Returns the multiplier used to add a value by this Gst
        /// If GST is 12.5 then will return .125
        /// </summary>
        public decimal GstFraction
        {
            get { return (_percentage / 100); }
        }

        /// <summary>
        /// Implements the operator &lt;.
        /// </summary>
        /// <param name="percentage1">The Gst percentage1.</param>
        /// <param name="percentage2">The Gst percentage2.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator <(Gst percentage1, Gst percentage2)
        {
            return percentage1.LessThan(percentage2);
        }

        /// <summary>
        /// Implements the operator &gt;.
        /// </summary>
        /// <param name="percentage1">The Gst percentage1.</param>
        /// <param name="percentage2">The Gst percentage2.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator >(Gst percentage1, Gst percentage2)
        {
            return percentage1.GreaterThan(percentage2);
        }

        /// <summary>
        /// Implements the operator &gt;=.
        /// </summary>
        /// <param name="percentage1">The Gst percentage1.</param>
        /// <param name="percentage2">The Gst percentage2.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator >=(Gst percentage1, Gst percentage2)
        {
            return percentage1.GreaterThanOrEqual(percentage2);
        }

        /// <summary>
        /// Lesses the than or equal.
        /// </summary>
        /// <param name="percentage">The Gst percentage.</param>
        /// <returns></returns>
        public bool LessThanOrEqual(Gst percentage)
        {
            return _percentage <= percentage._percentage;
        }

        /// <summary>
        /// Lesses the than.
        /// </summary>
        /// <param name="percentage">The Gst percentage.</param>
        /// <returns></returns>
        public bool LessThan(Gst percentage)
        {
            return _percentage < percentage._percentage;
        }

        /// <summary>
        /// Greaters the than or equal.
        /// </summary>
        /// <param name="percentage">The Gst percentage.</param>
        /// <returns></returns>
        public bool GreaterThanOrEqual(Gst percentage)
        {
            return _percentage >= percentage._percentage;
        }

        /// <summary>
        /// Indicates if the given GST is greater than this GST
        /// </summary>
        /// <param name="percentage">The Gst percentage.</param>
        /// <returns></returns>
        public bool GreaterThan(Gst percentage)
        {
            return _percentage > percentage._percentage;
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="percentage1">The percentage1.</param>
        /// <param name="percentage2">The percentage2.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(Gst percentage1, Gst percentage2)
        {
            return !percentage1.Equals(percentage2);
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="percentage1">The percentage1.</param>
        /// <param name="percentage2">The percentage2.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(Gst percentage1, Gst percentage2)
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
        public bool Equals(Gst other)
        {
            return _percentage == other._percentage;
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
            if (!(obj is Gst)) return false;
            return Equals((Gst)obj);
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>
        /// A 32-bit signed integer that is the hash code for this instance.
        /// </returns>
        public override int GetHashCode()
        {
            return _percentage.GetHashCode();
        }

        /// <summary>
        /// Gets a zero GST percentage
        /// </summary>        
        public static Gst Zero
        {
            get { return new Gst(0); }
        }

        /// <summary>
        /// Gets a GST that represents no GST
        /// </summary>
        /// <value>The zero.</value>
        public static Gst None
        {
            get
            {
                Gst gst = new Gst(0);
                gst._isNone = true;
                return gst;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance represents no GST
        /// </summary>
        public bool IsNone
        {
            get { return _isNone; }
        }



        /// <summary>
        /// Gets a value indicating whether this instance is over one hundred.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is over one hundred; otherwise, <c>false</c>.
        /// </value>
        public bool IsOverOneHundred
        {
            get { return _percentage > 100; }
        }

        /// <summary>
        ///Returns the integer representation of this percentage
        /// </summary>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "integer")]
        public int ToInteger()
        {
            return (int)Math.Round(_percentage, 0, MidpointRounding.AwayFromZero);
        }

        /// <summary>
        /// Returns the decimal representation of this percentage
        /// </summary>
        public decimal ToDecimal()
        {
            return _percentage;
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
        /// 1  if given percenatge is less than this percentage
        /// </returns>
        public int CompareTo(Gst other)
        {
            return _percentage.CompareTo(other._percentage);
        }

        /// <summary>
        /// Returns the string representation of this percentage
        /// </summary>
        public override string ToString()
        {
            return GstFormat.FormatWith(_percentage);
        }

        /// <summary>
        /// Returns a numeric string representation of the <see cref="Gst"/> object.
        /// </summary>
        /// <returns></returns>
        public string ToNumericString()
        {
            return _percentage.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Parse a numeric string representing a unit of <see cref="Gst"/>
        /// </summary>
        /// <exception cref="ArgumentException">
        /// Thrown if the string amount cannot be parsed to a <see cref="Gst"/>
        /// </exception>
        public static Gst Parse(string amount)
        {
            if (string.IsNullOrEmpty(amount))
            {
                return Zero;
            }

            string unformattedText = amount.Replace(FormatString, "");

            decimal parsedValue;

            if (unformattedText.TryParseDecimal(out parsedValue))
            {
                return new Gst(parsedValue);
            }

            throw new InvalidOperationException("The amount {0} is not a valid GST percentage".FormatWith(amount));
        }

    }
}