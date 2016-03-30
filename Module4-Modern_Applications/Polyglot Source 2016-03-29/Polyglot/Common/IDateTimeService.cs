using System;

namespace Common
{
    ///<summary>
    /// Provides date and time functionality for a <see cref="Clock"/>.
    ///</summary>
    public interface IDateTimeService
    {
        /// <summary>
        /// Gets a <see cref="DateTime"/> object that represents the current date and time in UTC
        ///</summary>
        DateTime Now { get; }
    }
}