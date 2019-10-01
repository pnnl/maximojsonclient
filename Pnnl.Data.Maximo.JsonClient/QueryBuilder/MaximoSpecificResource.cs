using System;
using System.Linq;
using System.Text;

namespace Pnnl.Data.Maximo.JsonClient.QueryBuilder
{
    /// <summary>
    /// Represents a request for a specific Maximo object.
    /// </summary>
    public class MaximoResource : MaximoRequest
    {
        /// <summary>
        /// Gets or sets the unique Id of the resource.
        /// </summary>
        internal string Id { get; set; }

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

            if (string.IsNullOrEmpty(Id))
            {
                throw new ArgumentException($"Required parameter '{nameof(Id)} was not given.");
            }

            var sb = new StringBuilder();

            sb.Append($"{ObjectStructureName}/{Id}?");

            if (Select != null && Select.Any())
            {
                var selectSb = new StringBuilder("oslc.select=");
                foreach (var att in Select)
                {
                    selectSb.Append($"{att},");
                }

                var selectStr = selectSb.ToString();
                if (selectStr.EndsWith(","))
                {
                    selectStr = selectStr.Remove(selectStr.Length - 1);
                }

                sb.Append(selectStr);
            }
            else
            {
                sb.Append("oslc.select=*");
            }

            if (UseLean)
            {
                sb.Append("&lean=1");
            }

            return sb.ToString();
        }
    }
}
