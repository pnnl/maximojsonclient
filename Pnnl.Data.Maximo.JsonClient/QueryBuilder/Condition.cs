namespace Pnnl.Data.Maximo.JsonClient.QueryBuilder
{
    /// <summary>
    /// Base class for query conditions.
    /// </summary>
    public abstract class Condition
    {
        /// <summary>
        /// The name of the attribute.
        /// </summary>
        protected readonly string _attribute;

        /// <summary>
        /// The condition.
        /// </summary>
        protected readonly ConditionComparison _operator;

        /// <summary>
        /// Initializes a new instance of the <see cref="Condition"/> class.
        /// </summary>
        /// <param name="attribute">The name of the attribute.</param>
        /// <param name="compareOperator">The type of comparison to be performed.</param>
        protected Condition(string attribute, ConditionComparison compareOperator)
        {
            _attribute = attribute;
            _operator = compareOperator;
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public abstract override string ToString();
    }
}
