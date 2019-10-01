using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pnnl.Data.Maximo.JsonClient.QueryBuilder
{
    public class MaximoResourceSearch : MaximoRequest
    {
        const string SELECT_SEPERATOR = ",";

        /// <summary>
        /// Gets or sets the attributes to be searched.
        /// </summary>
        /// <value>
        /// The search attributes.
        /// </value>
        internal List<string> SearchAttributes { get; set; }
        
        /// <summary>
        /// Gets or sets the search term.
        /// </summary>
        /// <value>
        /// The search term.
        /// </value>
        internal string SearchTerm { get; set; }

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
        /// Gets or sets field used for ordering.
        /// </summary>
        /// <value>The order by.</value>
        internal string OrderBy { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to [order results descending].
        /// </summary>
        /// <value><see langword="null" /> if [order by descending] contains no value, <see langword="true" /> if [order by descending]; otherwise, <see langword="false" />.</value>
        internal bool OrderByDescending { get; set; }

        public MaximoResourceSearch()
        {
            SearchAttributes = new List<string>();
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        /// <exception cref="NotImplementedException"></exception>
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
            else
            {
                sb.Append("oslc.select=*");
            }

            if (SearchAttributes.Any() && !string.IsNullOrEmpty(SearchTerm))
            {
                var searchSb = new StringBuilder("&searchAttributes=");
                foreach (var attribute in SearchAttributes)
                {
                    searchSb.Append($"{attribute}{SELECT_SEPERATOR}");
                }
                
                var searchStr = searchSb.ToString();
                if (searchStr.EndsWith(SELECT_SEPERATOR))
                {
                    searchStr = searchStr.Remove(searchStr.Length - SELECT_SEPERATOR.Length);
                }

                sb.Append(searchStr);
                sb.Append($"&oslc.searchTerms=\"{SearchTerm}\"");
            }

            if (OrderBy != null)
            {
                sb.Append("&orderBy=");

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

            return sb.ToString();
        }
    }
}
