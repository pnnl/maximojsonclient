using System.Linq;
using Pnnl.Data.Maximo.JsonClient.QueryBuilder;

namespace Pnnl.Data.Maximo.JsonClient.Extensions
{
    /// <summary>
    /// Extensions to build queries for <see cref="MaximoResourceSet"/> objects.
    /// </summary>
    public static class MaximoQueryBuildingExtensions
    {
        /// <summary>
        /// Selects the saved query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="queryName">Name of the saved query.</param>
        public static MaximoResourceSet SavedQuery(this MaximoResourceSet query, string queryName)
        {
            query.SavedQuery = queryName;

            return query;
        }

        /// <summary>
        /// Specifies a query condition.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="condition">The condition.</param>
        public static MaximoResourceSet Where(this MaximoResourceSet query, Condition condition)
        {
            query.Where.Add(condition);

            return query;
        }

        /// <summary>
        /// Specifies a query condition.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="attribute">The name of the attribute.</param>
        /// <param name="condition">The relationship type.</param>
        /// <param name="value">The query value.</param>
        public static MaximoResourceSet Where(this MaximoResourceSet query, string attribute, ConditionComparison condition, string value)
        {
            query.Where.Add(new StringCondition(attribute, value, condition));

            return query;
        }

        /// <summary>
        /// Specifies a query condition.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="attribute">The name of the attribute.</param>
        /// <param name="condition">The relationship type.</param>
        /// <param name="value">The query value.</param>
        public static MaximoResourceSet Where(this MaximoResourceSet query, string attribute, ConditionComparison condition, int value)
        {
            query.Where.Add(new IntegerCondition(attribute, value, condition));

            return query;
        }

        /// <summary>
        /// Specifies a value IN list condition.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="attribute"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static MaximoResourceSet WhereIn(this MaximoResourceSet query, string attribute, params string[] values)
        {
            if (query.WhereIn.ContainsKey(attribute))
            {
                query.WhereIn[attribute] = values.ToList();
            }
            else
            {
                query.WhereIn.Add(attribute, values.ToList());
            }

            return query;
        }

        /// <summary>
        /// Specifies the field name used to sort results.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="field">The field.</param>
        public static MaximoResourceSet OrderBy(this MaximoResourceSet query, string field)
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
        public static MaximoResourceSet OrderByDescending(this MaximoResourceSet query, string field)
        {
            query.OrderBy = field;
            query.OrderByDescending = true;

            return query;
        }

        /// <summary>
        /// Uses Or when performing a query with multiple conditions.
        /// </summary>
        /// <param name="query">The query.</param>
        public static MaximoResourceSet UseOrForConditions(this MaximoResourceSet query)
        {
            query.UseOr = true;

            return query;
        }
    }
}
