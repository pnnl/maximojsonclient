using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Pnnl.Data.Maximo.JsonClient
{
    /// <summary>
    /// Represents a pool capable of creating <see cref="MaximoJsonClient"/> instances.
    /// </summary>
    public class MaximoJsonClientPool : IMaximoJsonClientPool
    {
        private readonly IOptions<MaximoJsonClientPoolOptions> _options;
        private readonly ILogger<MaximoJsonClientPool> _logger;
        private readonly ReaderWriterLockSlim _cacheLock;
        private readonly IMemoryCache _memoryCache;
        private Dictionary<string, IMaximoJsonClient> _clientPool;

        /// <summary>
        /// Initializes a new instance of the <see cref="MaximoJsonClientPool"/> class.
        /// </summary>
        /// <param name="options">Configuration options.</param>
        /// <param name="memoryCache">The cache to use for storing and retrieving items.</param>
        /// <param name="logger">The logger to use for notifications.</param>
        /// <exception cref="System.ArgumentNullException">options</exception>
        public MaximoJsonClientPool(IOptions<MaximoJsonClientPoolOptions> options, IMemoryCache memoryCache, ILogger<MaximoJsonClientPool> logger)
        {
            if (options?.Value == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            _options = options;
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _cacheLock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
            _clientPool = new Dictionary<string, IMaximoJsonClient>(StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Retrieves an existing client or creates a new client with the assigned <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The name of the connection to create.</param>
        /// <returns>The <see cref="MaximoJsonClient"/> with the assigned <paramref name="name"/>.</returns>
        public IMaximoJsonClient GetClient(string name)
        {
            _cacheLock.EnterUpgradeableReadLock();

            try
            {
                if (_clientPool.ContainsKey(name) && _clientPool[name] != null)
                {
                    _logger.LogTrace("Retrieving existing client \"{Name}\" from the pool.", name);

                    return _clientPool[name];
                }
                else
                {
                    _cacheLock.EnterWriteLock();

                    try
                    {
                        // Check again before continuing, as another thread may have just created the instance
                        if (_clientPool.ContainsKey(name) && _clientPool[name] != null)
                        {
                            _logger.LogTrace("Retrieving newly created client \"{Name}\" from the pool.", name);

                            return _clientPool[name];
                        }

                        var config = (from e in _options.Value.Endpoints
                                      where e.Key.Equals(name, StringComparison.OrdinalIgnoreCase)
                                      select e.Value).FirstOrDefault();

                        if (config == null)
                        {
                            throw new ArgumentException(nameof(name));
                        }

                        var client = new MaximoJsonClient(_memoryCache, config);

                        _clientPool[name] = client;

                        _logger.LogTrace("Created client \"{Name}\" and added it to the pool.", name);

                        return client;
                    }
                    finally
                    {
                        _cacheLock.ExitWriteLock();
                    }
                }
            }
            finally
            {
                _cacheLock.ExitUpgradeableReadLock();
            }
        }

        /// <summary>
        /// Attempts to retrieve an existing client or create a new client with the assigned <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The name of the connection to create.</param>
        /// <param name="client">The configured <see cref="MaximoJsonClient"/> client.</param>
        /// <returns><see langword="true"/> if a connection with the assigned <paramref name="name"/> was created, <see langword="false"/> otherwise.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="client"/> is <see langword="null"/></exception>
        public bool TryGetClient(string name, out IMaximoJsonClient client)
        {
            try
            {
                client = GetClient(name);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to retrieve client \"{Name}\". Reason: {@Exception}", name, ex);

                client = null;
                return false;
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _cacheLock?.Dispose();

            _clientPool?.Clear();
        }
    }
}
