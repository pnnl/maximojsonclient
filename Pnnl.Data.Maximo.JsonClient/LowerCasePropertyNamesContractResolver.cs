using Newtonsoft.Json.Serialization;

namespace Pnnl.Data.Maximo.JsonClient
{
    /// <summary>
    /// Resolves member mappings for a type, lower casing property names.
    /// </summary>
    public class LowerCasePropertyNamesContractResolver : DefaultContractResolver
    {
        /// <summary>
        /// Resolves the name of the property.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>Resolved name of the property.</returns>
        protected override string ResolvePropertyName(string propertyName)
        {
            return propertyName.ToLower();
        }
    }
}
