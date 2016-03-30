using System;
using System.Runtime.Serialization;

namespace Common
{
    /// <summary>
    /// Indicates if an invalid monetary operation is performed, typically if money in different currencies
    /// has a mathematical operation performed on it.
    /// </summary>
    [Serializable]
    public class InvalidMonetaryOperationException : ApplicationException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidMonetaryOperationException"/> class.
        /// </summary>
        public InvalidMonetaryOperationException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidMonetaryOperationException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public InvalidMonetaryOperationException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidMonetaryOperationException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public InvalidMonetaryOperationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidMonetaryOperationException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// The <paramref name="info"/> parameter is null.
        /// </exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">
        /// The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0).
        /// </exception>
        protected InvalidMonetaryOperationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}