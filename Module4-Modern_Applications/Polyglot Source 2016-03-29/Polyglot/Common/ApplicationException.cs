using System;
using System.Runtime.Serialization;

namespace Common
{
    /// <summary>
    /// All custom exceptions should extend ApplicationException
    /// </summary>
    [Serializable]
    public abstract class ApplicationException : Exception
    {
        [NonSerialized]
        private readonly ApplicationExceptionSeverity _severity = ApplicationExceptionSeverity.Error;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationException"/> class.
        /// </summary>
        protected ApplicationException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        protected ApplicationException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        protected ApplicationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Creates a ApplicationException with the given message and severity.
        /// </summary>
        /// <param name="message">The message displayed by the exception and ultimately displayed to the end user.</param>
        /// <param name="severity">The severity of the exception. For example exceeding a credit limit may be a warning
        /// the DB or network being down may be an error.</param>
        protected ApplicationException(string message, ApplicationExceptionSeverity severity)
            : base(message)
        {
            _severity = severity;
        }

        /// <summary>
        /// Creates a ApplicationException with the given message, severity and inner exception details.
        /// </summary>
        /// <param name="message">The message displayed by the exception and ultimately displayed to the end user.</param>
        /// <param name="innerException">The inner exception that may provide additional information.</param>
        /// <param name="severity">
        /// The severity of the exception. For example exceeding a credit limit may be a warning
        /// the DB or network being down may be an error.
        /// </param>
        protected ApplicationException(string message, Exception innerException, ApplicationExceptionSeverity severity)
            : base(message, innerException)
        {
            _severity = severity;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// The <paramref name="info"/> parameter is null.
        /// </exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">
        /// The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0).
        /// </exception>
        protected ApplicationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// The severity of the exception
        /// </summary>
        public ApplicationExceptionSeverity Severity
        {
            get { return _severity; }
        }

        /// <summary>
        /// Gets a user friendly message that can be displayed.
        /// </summary>
        public virtual string UserMessage
        {
            get { return Message; }
        }
    }
}