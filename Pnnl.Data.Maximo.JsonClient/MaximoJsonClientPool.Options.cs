using Pnnl.Data.Maximo.JsonClient.Configuration;
using System;
using System.Collections.Generic;

namespace Pnnl.Data.Maximo.JsonClient
{
    /// <summary>
    /// Defines the configuration options for <see cref="MaximoJsonClientPool"/> instances.
    /// </summary>
    public class MaximoJsonClientPoolOptions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MaximoJsonClientPoolOptions"/> class.
        /// </summary>
        public MaximoJsonClientPoolOptions()
        {
            Endpoints = new Dictionary<string, JsonClientConfig>(StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// A collection of named client configuration options.
        /// </summary>
        public Dictionary<string, JsonClientConfig> Endpoints { get; }
    }
}
