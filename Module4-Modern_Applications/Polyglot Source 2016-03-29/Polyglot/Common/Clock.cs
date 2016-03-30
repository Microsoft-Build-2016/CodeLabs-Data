using System;

namespace Common
{
    /// <summary>
    /// Represents a clock that can be configured to represent different dates and times.
    /// </summary>
    public static class Clock
    {
        private static IDateTimeService _dateTimeService = new SystemTimeService();

        /// <summary>
        /// Configures the specified date time service.
        /// </summary>
        /// <param name="dateTimeService">The date time service.</param>
        public static void Configure(IDateTimeService dateTimeService)
        {
            Argument.CheckIfNull(dateTimeService, "dateTimeService");
            _dateTimeService = dateTimeService;
        }

        /// <summary>
        /// Gets the current time
        /// </summary>
        /// <value>The now.</value>
        public static DateTime Now
        {
            get { return _dateTimeService.Now; }
        }
    }
}