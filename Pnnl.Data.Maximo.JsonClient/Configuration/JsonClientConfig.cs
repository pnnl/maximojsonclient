using System;

namespace Pnnl.Data.Maximo.JsonClient.Configuration
{
    /// <summary>
    /// Represents a description of how to connect to a Maximo JSON API.
    /// </summary>
    public class JsonClientConfig
    {
        /// <summary>
        /// The endpoint configuration name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The base URI for the API.
        /// </summary>
        public string BaseUri { get; set; }

        /// <summary>
        /// Gets or sets the number of seconds in a connection lifetime.
        /// </summary>
        /// <remarks>Default value is 1440</remarks>
        public int SessionLifetime { get; set; } = 1440;

        /// <summary>
        /// Gets or sets the timeout length in seconds for an HTTP request.
        /// </summary>
        /// <remarks>Default value is 300</remarks>
        public int RequestTimeSpan { get; set; } = 300;

        /// <summary>
        /// Gets or sets a value indicating whether to authenticate with lean=1.
        /// </summary>
        /// <value><see langword="true" /> if [use lean]; otherwise, <see langword="false" />.</value>
        public bool UseLean { get; set; } = true;

        /// <summary>
        /// Gets or sets the name of the user for authentication.
        /// </summary>
        /// <remarks>Leave <see langword="null"/> to use default credentials.</remarks>
        public string User { get; set; }

        /// <summary>
        /// Gets or sets the password of the user used for authentication.
        /// </summary>
        /// <remarks>This is only used when <see cref="User"/> is not <see langword="null"/>.</remarks>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the domain of the user for authentication.
        /// </summary>
        /// <remarks>This is only used when <see cref="User"/> is not <see langword="null"/>.</remarks>
        public string Domain { get; set; }

        /// <summary>
        /// Gets or sets the fully qualified type to instantiate for the serializer contract resolver.  Leave null to use the default contract resolver.
        /// </summary>
        public string SerializerContractResolverTypeName { get; set; }

        /// <summary>
        /// Gets or sets the fully qualified type to instantiate for the deserializer contract resolver.  Leave null to use the default contract resolver.
        /// </summary>
        public string DeserializerContractResolverTypeName { get; set; }
    }
}
