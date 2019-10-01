using System;

namespace Pnnl.Data.Maximo.JsonClient.Exceptions
{
    /// <summary>
    /// The exception that is thrown when API authentication fails.
    /// </summary>
    /// <seealso cref="System.Exception" />
    [Serializable]
    public class AuthenticationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationException"/> class.
        /// </summary>
        public AuthenticationException() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public AuthenticationException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        public AuthenticationException(string message, Exception inner) : base(message, inner) { }
    }
}
