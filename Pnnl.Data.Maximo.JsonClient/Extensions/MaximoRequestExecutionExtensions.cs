using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Pnnl.Data.Maximo.JsonClient.QueryBuilder;

namespace Pnnl.Data.Maximo.JsonClient.Extensions
{
    /// <summary>
    /// Extensions to execute <see cref="MaximoRequest"/> queries.
    /// </summary>
    public static class MaximoRequestExecutionExtensions
    {
        #region GET and return raw JSON

        /// <summary>
        /// Performs a GET using the query.
        /// </summary>
        /// <param name="request">The query.</param>
        /// <param name="cancellationToken">The optional token to monitor for cancellation requests.</param>
        /// <returns>A <see cref="string"/> containing the raw text response from the API.</returns>
        public static async Task<string> GetAsync(this MaximoRequest request, CancellationToken cancellationToken)
        {
            var uri = request.ToUri();

            return await request.Client.SendAndGetStringResponseAsync(HttpMethod.Get, uri, null, request.Headers, request.ImpersonateIdentity, cancellationToken);
        }

        #endregion

        #region GET and return typed object from object base

        /// <summary>
        /// Performs a GET using the query.
        /// </summary>
        /// <typeparam name="T">Specifies the type of the return object.</typeparam>
        /// <param name="request">The query.</param>
        /// <param name="cancellationToken">The optional token to monitor for cancellation requests.</param>
        /// <returns>A deserialized object containing a representation of the response from the API.</returns>
        public static async Task<T> GetAsync<T>(this MaximoRequest request, CancellationToken cancellationToken)
        {
            var uri = request.ToUri();

            var response = await request.Client.SendAndGetTypedObjectResponseAsync<T>(HttpMethod.Get, uri, null, request.Headers, request.ImpersonateIdentity, cancellationToken);

            return response;
        }

        #endregion

        #region GET and return dynamic object from object base

        /// <summary>
        /// Performs a GET using the query and returns a dynamic object.
        /// </summary>
        /// <param name="request">The query.</param>
        /// <param name="cancellationToken">The optional token to monitor for cancellation requests.</param>
        /// <returns>A dynamic object containing a representation of the response from the API.</returns>
        public static async Task<dynamic> DynamicGetAsync(this MaximoRequest request, CancellationToken cancellationToken)
        {
            var uri = request.ToUri();

            var response = await request.Client.SendAndGetObjectResponseAsync(HttpMethod.Get, uri, null, request.Headers, request.ImpersonateIdentity, cancellationToken);

            return response;
        }

        #endregion

        #region POST and return raw JSON

        /// <summary>
        /// Performs a POST using the query.
        /// </summary>
        /// <param name="request">The query.</param>
        /// <param name="cancellationToken">The optional token to monitor for cancellation requests.</param>
        /// <param name="body">An object to serialize and send to the remote endpoint.</param>
        public static async Task<string> PostAsync(this MaximoRequest request, CancellationToken cancellationToken, object body = null)
        {
            var uri = request.ToUri();

            return await request.Client.SendAndGetStringResponseAsync(HttpMethod.Post, uri, body, request.Headers, request.ImpersonateIdentity, cancellationToken);
        }

        #endregion

        #region POST and return typed object from object base

        /// <summary>
        /// Performs a POST using the query.
        /// </summary>
        /// <typeparam name="T">Specifies the type of the return object.</typeparam>
        /// <param name="request">The query</param>
        /// <param name="cancellationToken">The optional token to monitor for cancellation requests.</param>
        /// <param name="body">An object to serialize and send to the remote endpoint.</param>
        /// <returns>A deserialized object containing a representation of the response from the API.</returns>
        public static async Task<T> PostAsync<T>(this MaximoRequest request, CancellationToken cancellationToken, object body = null)
        {
            var uri = request.ToUri();

            var response = await request.Client.SendAndGetTypedObjectResponseAsync<T>(HttpMethod.Post, uri, body, request.Headers, request.ImpersonateIdentity, cancellationToken);

            return response;
        }

        #endregion

        #region POST and return a dynamic object from object base

        /// <summary>
        /// Performs a POST using the query and returns a dynamic object.
        /// </summary>
        /// <param name="request">The query</param>
        /// <param name="cancellationToken">The optional token to monitor for cancellation requests.</param>
        /// <param name="body">An object to serialize and send to the remote endpoint.</param>
        /// <returns>A dynamic object containing a representation of the response from the API.</returns>
        public static async Task<dynamic> DynamicPostAsync(this MaximoRequest request, CancellationToken cancellationToken, object body = null)
        {
            var uri = request.ToUri();

            var response = await request.Client.SendAndGetObjectResponseAsync(HttpMethod.Post, uri, body, request.Headers, request.ImpersonateIdentity, cancellationToken);

            return response;
        }

        #endregion

        #region Shortcut methods

        private static void SetUpdateHeaders(this MaximoRequest request)
        {
            request.Headers["x-method-override"] = "PATCH";
            request.Headers["patchtype"] = "MERGE";
            request.Headers["properties"] = "*";
        }

        private static void SetCreateHeaders(this MaximoRequest request)
        {
            request.IsCreate = true;
            request.Headers["properties"] = "*";
        }

        /// <summary>
        /// Performs an update on a resource.
        /// </summary>
        /// <param name="request">The query.</param>
        /// <param name="cancellationToken">The optional token to monitor for cancellation requests.</param>
        /// <param name="body">An object to serialize and send to the remote endpoint.</param>
        /// <returns></returns>
        public static async Task<string> UpdateAsync(this MaximoRequest request, CancellationToken cancellationToken, object body = null)
        {
            request.SetUpdateHeaders();

            return await PostAsync(request, cancellationToken, body);
        }

        /// <summary>
        /// Performs an update on a resource.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="request">The query.</param>
        /// <param name="cancellationToken">The optional token to monitor for cancellation requests.</param>
        /// <param name="body">An object to serialize and send to the remote endpoint.</param>
        /// <returns></returns>
        public static async Task<T> UpdateAsync<T>(this MaximoRequest request, CancellationToken cancellationToken, object body = null)
        {
            request.SetUpdateHeaders();

            return await PostAsync<T>(request, cancellationToken, body);
        }

        /// <summary>
        /// Performs an update on a resource.
        /// </summary>
        /// <param name="request">The query.</param>
        /// <param name="cancellationToken">The optional token to monitor for cancellation requests.</param>
        /// <param name="body">An object to serialize and send to the remote endpoint.</param>
        /// <returns>A deserialized object containing a representation of the response from the API.</returns>
        public static async Task<dynamic> UpdateDynamicAsync(this MaximoRequest request, CancellationToken cancellationToken, object body = null)
        {
            request.SetUpdateHeaders();

            return await DynamicPostAsync(request, cancellationToken, body);
        }

        /// <summary>
        /// Creates a resource.
        /// </summary>
        /// <param name="request">The query.</param>
        /// <param name="cancellationToken">The optional token to monitor for cancellation requests.</param>
        /// <param name="body">An object to serialize and send to the remote endpoint.</param>
        /// <returns></returns>
        public static async Task<string> CreateAsync(this MaximoRequest request, CancellationToken cancellationToken, object body = null)
        {
            request.SetCreateHeaders();

            return await PostAsync(request, cancellationToken, body);
        }

        /// <summary>
        /// Creates a resource.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="request">The query.</param>
        /// <param name="cancellationToken">The optional token to monitor for cancellation requests.</param>
        /// <param name="body">An object to serialize and send to the remote endpoint.</param>
        /// <returns></returns>
        public static async Task<T> CreateAsync<T>(this MaximoRequest request, CancellationToken cancellationToken, object body = null)
        {
            request.SetCreateHeaders();

            return await PostAsync<T>(request, cancellationToken, body);
        }

        /// <summary>
        /// Creates a resource.
        /// </summary>
        /// <param name="request">The query.</param>
        /// <param name="cancellationToken">The optional token to monitor for cancellation requests.</param>
        /// <param name="body">An object to serialize and send to the remote endpoint.</param>
        /// <returns>A deserialized object containing a representation of the response from the API.</returns>
        public static async Task<dynamic> CreateDynamicAsync(this MaximoRequest request, CancellationToken cancellationToken, object body = null)
        {
            request.SetCreateHeaders();

            return await DynamicPostAsync(request, cancellationToken, body);
        }

        /// <summary>
        /// Performs a deletion on a resource.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task DeleteAsync(this MaximoRequest request, CancellationToken cancellationToken)
        {
            request.Headers["x-method-override"] = "DELETE";

            // We need a body to trigger adding the content type header.
            await PostAsync(request, cancellationToken, string.Empty);
        }

        #endregion
    }
}
