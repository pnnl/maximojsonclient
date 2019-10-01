using System;

namespace Pnnl.Data.Maximo.JsonClient
{
    /// <summary>
    /// Describes a pool capable of creating <see cref="MaximoJsonClient"/> instances.
    /// </summary>
    public interface IMaximoJsonClientPool : IDisposable
    {
        /// <summary>
        /// Retrieves an existing client or creates a new client with the assigned <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The name of the connection to create.</param>
        /// <returns>The <see cref="MaximoJsonClient"/> with the assigned <paramref name="name"/>.</returns>
        IMaximoJsonClient GetClient(string name);

        /// <summary>
        /// Attempts to retrieve an existing client or create a new client with the assigned <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The name of the connection to create.</param>
        /// <param name="client">The configured <see cref="MaximoJsonClient"/> client.</param>
        /// <returns><see langword="true"/> if a connection with the assigned <paramref name="name"/> was created, <see langword="false"/> otherwise.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="client"/> is <see langword="null"/></exception>
        bool TryGetClient(string name, out IMaximoJsonClient client);
    }
}
