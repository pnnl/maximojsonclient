using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pnnl.Data.Maximo.JsonClient.QueryBuilder
{
    /// <summary>
    /// Represents a structured endpoint query.
    /// </summary>
    public class MaximoResourceSet : MaximoRequest
    {
        const string SELECT_SEPERATOR = ",";
        const string WHERE_SEPERATOR = " and ";

        /// <summary>
        /// Gets or sets the where.
        /// </summary>
        /// <value>
        /// The where.
        /// </value>
        internal List<Condition> Where { get; set; }

        /// <summary>
        /// Gets or sets the where in.
        /// </summary>
        /// <value>
        /// The where in.
        /// </value>
        internal Dictionary<string, List<string>> WhereIn { get; set; }

        /// <summary>
        /// Gets or sets the saved query.
        /// </summary>
        /// <value>
        /// The saved query.
        /// </value>
        internal string SavedQuery { get; set; }

        /// <summary>
        /// Gets or sets field used for ordering.
        /// </summary>
        /// <value>The order by.</value>
        internal string OrderBy { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to [order results descending].
        /// </summary>
        /// <value><see langword="null" /> if [order by descending] contains no value, <see langword="true" /> if [order by descending]; otherwise, <see langword="false" />.</value>
        internal bool OrderByDescending { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether conditions use OR instead of AND.
        /// </summary>
        /// <value><see langword="true" /> if [conditions use or]; otherwise, <see langword="false" />.</value>
        internal bool UseOr { get; set; }

        /// <summary>
        /// Gets or sets the size of the page.
        /// </summary>
        /// <value>
        /// The size of the page.
        /// </value>
        internal int? PageSize { get; set; }

        /// <summary>
        /// Gets or sets the page number.
        /// </summary>
        /// <value>
        /// The page number.
        /// </value>
        internal int? PageNumber { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MaximoResourceSet"/> class.
        /// </summary>
        public MaximoResourceSet()
        {
            Where = new List<Condition>();
            WhereIn = new Dictionary<string, List<string>>();
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        /// <exception cref="ArgumentException">Resource</exception>
        public override string ToString()
        {
            if (string.IsNullOrEmpty(ObjectStructureName))
            {
                throw new ArgumentException($"Required paramater '{nameof(ObjectStructureName)}' was not given.");
            }

            var sb = new StringBuilder();

            sb.Append($"{ObjectStructureName}?");

            if (Select != null && Select.Any())
            {
                var selectSb = new StringBuilder("oslc.select=");
                foreach (var att in Select)
                {
                    selectSb.Append($"{att}{SELECT_SEPERATOR}");
                }

                var selectStr = selectSb.ToString();
                if (selectStr.EndsWith(SELECT_SEPERATOR))
                {
                    selectStr = selectStr.Remove(selectStr.Length - SELECT_SEPERATOR.Length);
                }

                sb.Append(selectStr);
            }
            else if (!IsCreate)
            {
                sb.Append("oslc.select=*");
            }

            if (!string.IsNullOrEmpty(SavedQuery))
            {
                sb.Append($"&savedQuery={SavedQuery}");
            }

            if (Where.Any() || WhereIn.Any())
            {
                var whereSb = new StringBuilder("&oslc.where=");
                foreach (var what in Where)
                {
                    whereSb.Append($"{what}{WHERE_SEPERATOR}");
                }

                foreach (var whatIn in WhereIn)
                {
                    var valueList = whatIn.Value.Select(v => $"\"{v}\"").ToList();
                    whereSb.Append($"{whatIn.Key} in [{string.Join(",", valueList)}]{WHERE_SEPERATOR}");
                }

                var whereStr = whereSb.ToString();
                if (whereStr.EndsWith(WHERE_SEPERATOR))
                {
                    whereStr = whereStr.Remove(whereStr.Length - WHERE_SEPERATOR.Length);
                }

                sb.Append(whereStr);
            }

            if (OrderBy != null)
            {
                sb.Append("&oslc.orderBy=");

                sb.Append(OrderByDescending ? "-" : "%2B");

                sb.Append(OrderBy);
            }

            if (PageSize != null && PageSize > 0)
            {
                sb.Append($"&oslc.pageSize={PageSize}");

                if (PageNumber != null && PageNumber > 1)
                {
                    sb.Append($"&pageno={PageNumber}");
                }
            }

            if (UseLean)
            {
                sb.Append("&lean=1");
            }

            if (UseOr)
            {
                sb.Append("&opmodeor=1");
            }

            return sb.ToString();
        }
    }
}
