namespace Pnnl.Data.Maximo.JsonClient.QueryBuilder
{
    /// <summary>
    /// Specifies the comparison rules to be used by <see cref="Condition"/> comparisons.
    /// </summary>
    public enum ConditionComparison
    {
        /// <summary>
        /// The equals
        /// </summary>
        Equals,
        /// <summary>
        /// The notequal
        /// </summary>
        NotEqual,
        /// <summary>
        /// The like
        /// </summary>
        Contains,
        /// <summary>
        /// The starts with
        /// </summary>
        StartsWith,
        /// <summary>
        /// The ends with
        /// </summary>
        EndsWith,
        /// <summary>
        ///  The greater than or equal
        /// </summary>
        GreaterThanOrEqual,
        /// <summary>
        /// The less than or equal
        /// </summary>
        LessThanOrEqual,
        /// <summary>
        /// The greater than
        /// </summary>
        GreaterThan,
        /// <summary>
        /// The less than
        /// </summary>
        LessThan
    }
}
