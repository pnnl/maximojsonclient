using System;
using System.Linq;
using System.Security.Principal;
using Pnnl.Data.Maximo.JsonClient.QueryBuilder;

namespace Pnnl.Data.Maximo.JsonClient.Extensions
{
    /// <summary>
    /// Extensions to build queries for <see cref="MaximoRequest"/> objects.
    /// </summary>
    public static class MaximoRequestBuildingExtensions
    {
        /// <summary>
        /// Specifies a user to impersonate when executing the request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="identity">The identity.</param>
        public static T AsUser<T>(this T request, WindowsIdentity identity) where T : MaximoRequest
        {
            request.ImpersonateIdentity = identity;

            return request;
        }

        /// <summary>
        /// Specifies a user to impersonate when executing the request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="principal">The principal that contains a <see cref="WindowsIdentity"/>.</param>
        public static T AsUser<T>(this T request, WindowsPrincipal principal) where T : MaximoRequest
        {
            var identity = principal.Identities.FirstOrDefault(i => i is WindowsIdentity);

            if (identity == null)
            {
                throw new InvalidOperationException("The principal does not contain a Windows identity.");
            }

            request.ImpersonateIdentity = identity as WindowsIdentity;

            return request;
        }

        /// <summary>
        /// Specifies if the request should omit OSLC namespaces in JSON data.
        /// </summary>
        /// <param name="request">The query.</param>
        public static T UseLean<T>(this T request) where T : MaximoRequest
        {
            request.UseLean = true;

            return request;
        }

        /// <summary>
        /// Adds a custom request header.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="name">The name of the header.</param>
        /// <param name="value">The value of the header.</param>
        public static T AddHeader<T>(this T request, string name, string value) where T : MaximoRequest
        {
            request.Headers.Add(name, value);

            return request;
        }

        /// <summary>
        /// Selects the specified field.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="attribute">The attribute name.</param>
        public static T Select<T>(this T request, string attribute) where T : MaximoRequest
        {
            request.Select.Add(attribute);

            return request;
        }

        /// <summary>
        /// Selects the specified fields.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="attributes">The names of the attributes.</param>
        public static T Select<T>(this T request, params string[] attributes) where T : MaximoRequest
        {
            if (attributes != null && attributes.Length > 0)
            {
                foreach (var attribute in attributes)
                {
                    if (attribute != null && !request.Select.Contains(attribute))
                    {
                        request.Select.Add(attribute);
                    }
                }
            }

            return request;
        }
    }
}
