using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;

namespace Pnnl.Data.Maximo.JsonClient
{
    /// <summary>
    /// Represents a client capable of retrieving information from Maximo using JSON HTTP services.
    /// </summary>
    public interface IMaximoJsonClient
    {
        /// <summary>
        /// Gets or sets the serializer settings.
        /// </summary>
        JsonSerializerSettings SerializerSettings { get; set; }

        /// <summary>
        /// Gets or sets the deserializer settings.
        /// </summary>
        JsonSerializerSettings DeserializerSettings { get; set; }

        /// <summary>
        /// Asynchronously performs a command on the resource with the specified <paramref name="uri" />.
        /// </summary>
        /// <param name="verb">The HTTP verb of the request.</param>
        /// <param name="uri">The URI to get.</param>
        /// <param name="body">The body.</param>
        /// <param name="headers">Headers to set in the outgoing request.</param>
        /// <param name="identity">The identity of the user.</param>
        /// <param name="cancellationToken">The optional token to monitor for cancellation requests.</param>
        /// <returns>A <see cref="Task{TResult}"/> whose result yields the resource of the specific <paramref name="uri"/>.</returns>
        Task<string> SendAndGetStringResponseAsync(HttpMethod verb, Uri uri, object body, IDictionary<string, string> headers, WindowsIdentity identity = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Asynchronously performs a command on the resource with the specified <paramref name="uri" />.
        /// </summary>
        /// <param name="verb">The HTTP verb of the request.</param>
        /// <param name="uri">The URI to get.</param>
        /// <param name="body">The body.</param>
        /// <param name="headers">Headers to set in the outgoing request.</param>
        /// <param name="identity">The identity of the user.</param>
        /// <param name="cancellationToken">The optional token to monitor for cancellation requests.</param>
        /// <returns>A <see cref="Task{TResult}"/> whose result yields the resource of the specific <paramref name="uri"/>.</returns>
        Task<object> SendAndGetObjectResponseAsync(HttpMethod verb, Uri uri, object body, IDictionary<string, string> headers, WindowsIdentity identity = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Asynchronously performs a command on the resource with the specified <paramref name="uri" />.
        /// </summary>
        /// <param name="verb">The HTTP verb of the request.</param>
        /// <param name="uri">The URI to get.</param>
        /// <param name="body">The body.</param>
        /// <param name="headers">Headers to set in the outgoing request.</param>
        /// <param name="identity">The identity of the user.</param>
        /// <param name="cancellationToken">The optional token to monitor for cancellation requests.</param>
        /// <returns>A <see cref="Task{TResult}"/> whose result yields the resource of the specific <paramref name="uri"/>.</returns>
        Task<T> SendAndGetTypedObjectResponseAsync<T>(HttpMethod verb, Uri uri, object body, IDictionary<string, string> headers, WindowsIdentity identity = null, CancellationToken cancellationToken = default(CancellationToken));
    }
}
