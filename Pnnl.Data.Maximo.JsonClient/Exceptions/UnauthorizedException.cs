using System;

namespace Pnnl.Data.Maximo.JsonClient.Exceptions
{
    /// <summary>
    /// The exception that is thrown when access to a resource is forbidden.
    /// </summary>
    /// <seealso cref="System.Exception" />
    [Serializable]
    public class UnauthorizedException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnauthorizedException"/> class.
        /// </summary>
        public UnauthorizedException() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnauthorizedException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public UnauthorizedException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnauthorizedException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        public UnauthorizedException(string message, Exception inner) : base(message, inner) { }
    }
}
