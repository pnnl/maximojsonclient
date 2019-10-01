using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Pnnl.Data.Maximo.JsonClient.QueryBuilder;

namespace Pnnl.Data.Maximo.JsonClient.Extensions
{
    /// <summary>
    /// Extensions to execute <see cref="MaximoResourceSearch"/> searches.
    /// </summary>
    public static class MaximoTermSearchExecutionExtensions
    {
        /// <summary>
        /// Performs a GET using the search query.
        /// </summary>
        /// <typeparam name="T">Specifies the type of the return object.</typeparam>
        /// <param name="query">The search query.</param>
        /// <param name="cancellationToken">The optional token to monitor for cancellation requests.</param>
        /// <param name="pageSize">The maximum number of items to return.</param>
        /// <param name="page">The page number to retrieve.</param>
        /// <returns>An <see cref="IList{TResult}"/> containing a collection of items from the API.</returns>
        public static async Task<IList<T>> GetListAsync<T>(this MaximoResourceSearch query, CancellationToken cancellationToken, int? pageSize = null, int? page = null)
        {
            query.PageSize = pageSize;
            query.PageNumber = page;

            var uri = query.ToUri();

            var response = await query.Client.SendAndGetStringResponseAsync(HttpMethod.Get, uri, null, query.Headers, query.ImpersonateIdentity, cancellationToken);

            var jObj = JObject.Parse(response);

            if (jObj["member"] == null)
            {
                throw new KeyNotFoundException(@"The ""member"" attribute was not found.");
            }

            return jObj["member"].ToObject<List<T>>();
        }
    }
}
