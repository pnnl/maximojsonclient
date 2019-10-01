using System;

namespace Pnnl.Data.Maximo.JsonClient.QueryBuilder
{
    /// <summary>
    /// Represents a <see cref="string"/> based query condition.
    /// </summary>
    public class StringCondition : Condition
    {
        /// <summary>
        /// The value
        /// </summary>
        private readonly string _value;

        /// <summary>
        /// Initializes a new instance of the <see cref="StringCondition" /> class.
        /// </summary>
        /// <param name="attribute">The attribute.</param>
        /// <param name="value">The value.</param>
        /// <param name="op">The operator.</param>
        public StringCondition(string attribute, string value, ConditionComparison op = ConditionComparison.Equals) : base(attribute, op)
        {
            _value = value;
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            if (_operator == ConditionComparison.Equals)
                return $"{_attribute}=\"{_value}\"";

            if (_operator == ConditionComparison.NotEqual)
                return $"{_attribute}!=\"{_value}\"";

            if (_operator == ConditionComparison.Contains)
                return $"{_attribute}=\"%25{_value}%25\"";

            if (_operator == ConditionComparison.StartsWith)
                return $"{_attribute}=\"{_value}%25\"";

            if (_operator == ConditionComparison.EndsWith)
                return $"{_attribute}=\"%25{_value}\"";

            if (_operator == ConditionComparison.GreaterThanOrEqual)
                return $"{_attribute}>=\"{_value}\"";

            if (_operator == ConditionComparison.LessThanOrEqual)
                return $"{_attribute}<=\"{_value}\"";

            if (_operator == ConditionComparison.GreaterThan)
                return $"{_attribute}>\"{_value}\"";

            if (_operator == ConditionComparison.LessThan)
                return $"{_attribute}<\"{_value}\"";

            throw new NotImplementedException("The operator type is not valid for this condition type.");
        }
    }
}
