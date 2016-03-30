namespace Common
{
    /// <summary>
    /// The severity of the exception. For example exceeding a credit limit may be a warning
    /// the DB or network being down may be an error.
    /// </summary>
    public enum ApplicationExceptionSeverity
    {
        /// <summary>
        /// Indicates the exception shuld be displayed to the user as a warning
        /// </summary>
        Warning,
        /// <summary>
        /// Indicates the exception should be displayed to the user as an error.
        /// </summary>
        Error
    }
}