using Pnnl.Data.Maximo.JsonClient.QueryBuilder;

namespace Pnnl.Data.Maximo.JsonClient.Extensions
{
    /// <summary>
    /// Extensions to build queries for <see cref="MaximoResourceSearch"/> objects.
    /// </summary>
    public static class MaximoTermSearchBuildingExtensions
    {
        /// <summary>
        /// Searches the specified field.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="attribute">The attribute name.</param>
        public static MaximoResourceSearch SearchAttributes(this MaximoResourceSearch query, string attribute)
        {
            query.SearchAttributes.Add(attribute);

            return query;
        }

        /// <summary>
        /// Searches the specified fields.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="attributes">The names of the attributes.</param>
        public static MaximoResourceSearch SearchAttributes(this MaximoResourceSearch query, params string[] attributes)
        {
            if (attributes != null && attributes.Length > 0)
            {
                foreach (var attribute in attributes)
                {
                    if (attribute != null && !query.SearchAttributes.Contains(attribute))
                    {
                        query.SearchAttributes.Add(attribute);
                    }
                }
            }

            return query;
        }

        /// <summary>
        /// Specifies the term to search for.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="term">The term.</param>
        public static MaximoResourceSearch SearchTerm(this MaximoResourceSearch query, string term)
        {
            query.SearchTerm = term;

            return query;
        }

        /// <summary>
        /// Specifies the field name used to sort results.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="field">The field.</param>
        public static MaximoResourceSearch OrderBy(this MaximoResourceSearch query, string field)
        {
            query.OrderBy = field;
            query.OrderByDescending = false;

            return query;
        }

        /// <summary>
        /// Specifies the field names used to sort results descending.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="field">The field.</param>
        public static MaximoResourceSearch OrderByDescending(this MaximoResourceSearch query, string field)
        {
            query.OrderBy = field;
            query.OrderByDescending = true;

            return query;
        }
    }
}
