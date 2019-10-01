using System;
using System.Collections.Generic;
using System.Security.Principal;

namespace Pnnl.Data.Maximo.JsonClient.QueryBuilder
{
    /// <summary>
    /// Represents the base set of information needed to begin a request for resources from Maximo.
    /// </summary>
    public abstract class MaximoRequest
    {
        /// <summary>
        /// Gets or sets the client.
        /// </summary>
        internal IMaximoJsonClient Client { get; set; }

        /// <summary>
        /// Gets or sets the name of the object structure.
        /// </summary>
        internal string ObjectStructureName { get; set; }

        /// <summary>
        /// Gets or sets the headers.
        /// </summary>
        internal Dictionary<string, string> Headers { get; set; }

        /// <summary>
        /// Gets or sets the identity to impersonate.
        /// </summary>
        internal WindowsIdentity ImpersonateIdentity { get; set; }

        /// <summary>
        /// Gets or sets the select.
        /// </summary>
        /// <value>
        /// The select.
        /// </value>
        internal List<string> Select { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [use lean].
        /// </summary>
        /// <value><see langword="true" /> if [use lean]; otherwise, <see langword="false" />.</value>
        internal bool UseLean { get; set; }


        /// <summary>
        /// Gets or sets a value indicating whether this instance is create.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is create; otherwise, <c>false</c>.
        /// </value>
        internal bool IsCreate { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MaximoRequest"/> class.
        /// </summary>
        protected MaximoRequest()
        {
            Headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            Select = new List<string>();
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public abstract override string ToString();

        /// <summary>
        /// Returns a <see cref="Uri" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="Uri" /> that represents this instance.
        /// </returns>
        /// <exception cref="System.ArgumentException">Resource</exception>
        internal Uri ToUri(string prefix = "os/")
        {
            var uriString = this.ToString();

            if (prefix != null)
            {
                uriString = prefix + uriString;
            }

            return new Uri(uriString, UriKind.Relative);
        }
    }
}
