using System;
using Common;

namespace MSCorp.AdventureWorks.Core.Domain
{
    /// <summary>
    /// The rating of a particular product
    /// </summary>
    public class ProductRating
    {
        private int _numberOfStars;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductRating"/> class.
        /// </summary>
        public ProductRating(int numberOfStars)
        {
            NumberOfStars = numberOfStars;
        }

        /// <summary>
        /// The maximum star rating.
        /// </summary>
        public const int MaximumStarRating = 5;

        /// <summary>
        /// Gets the number of stars.
        /// </summary>
        public int NumberOfStars
        {
            get { return _numberOfStars; }
            private set
            {
                if (value > MaximumStarRating || value < 0)
                {
                    throw new ArgumentException("rating can only be between 0 and {1}".FormatWith(MaximumStarRating), "{0}".FormatWith(value));
                } 
                _numberOfStars = value;
            }
        }
    }
}