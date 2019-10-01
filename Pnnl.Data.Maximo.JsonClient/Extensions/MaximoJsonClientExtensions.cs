using System.Linq;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Pnnl.Data.Maximo.JsonClient.QueryBuilder;

namespace Pnnl.Data.Maximo.JsonClient.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="IMaximoJsonClient"/> objects.
    /// </summary>
    public static class MaximoJsonClientExtensions
    {
        /// <summary>
        /// Specifies the type of object structure to access.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="objectStructureName">The name of the Maximo object structure.</param>
        /// <returns>A <see cref="MaximoResourceSet"/> instance.</returns>
        public static MaximoResourceSet ForResourceSet(this IMaximoJsonClient client, string objectStructureName)
        {
            return new MaximoResourceSet
            {
                Client = client,
                ObjectStructureName = objectStructureName
            };
        }

        /// <summary>
        /// Specifies the type of resource to access.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="objectStructureName">The name of the Maximo object structure.</param>
        /// <param name="id">The unique Id of the specific resource.</param>
        /// <returns>A <see cref="MaximoResourceSet"/> instance.</returns>
        public static MaximoResource ForResource(this IMaximoJsonClient client, string objectStructureName, string id)
        {
            return new MaximoResource
            {
                Client = client,
                ObjectStructureName = objectStructureName,
                Id = id
            };
        }

        /// <summary>
        /// Specifies the type of object structure to search.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="objectStructureName">The name of the Maximo object structure.</param>
        /// <returns> A <see cref="MaximoResourceSet" /> instance. </returns>
        public static MaximoResourceSearch ForResourceSearch(this IMaximoJsonClient client, string objectStructureName)
        {
            return new MaximoResourceSearch
            {
                Client = client,
                ObjectStructureName = objectStructureName
            };
        }
    }
}
