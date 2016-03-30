using System;

namespace Common
{
    ///<summary>
    /// Provides dates and times using the local system time.
    ///</summary>
    public class SystemTimeService : IDateTimeService
    {
        /// <summary>
        /// Gets a <see cref="DateTime"/> object that represents the current date and time in UTC
        ///</summary>
        public DateTime Now
        {
            get { return DateTime.UtcNow; }
        }
    }
}