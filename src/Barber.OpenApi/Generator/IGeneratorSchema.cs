namespace Barber.OpenApi.Generator
{
    using System.Collections.Generic;
    using Barber.OpenApi.Models.Template;
    using Microsoft.OpenApi.Models;

    /// <summary>
    /// Generator for handling OpenApi Schema section
    /// </summary>
    public interface IGeneratorSchema
    {
        /// <summary>
        /// Convert a OpenApi schema property to intermediate Property Model
        /// </summary>
        /// <param name="schema">Root OpenApi schema</param>
        /// <param name="propertyItem">OpenApi property</param>
        /// <returns></returns>
        PropertyModel GetProperty(KeyValuePair<string, OpenApiSchema> schema, KeyValuePair<string, OpenApiSchema> propertyItem);

        /// <summary>
        /// Convert a OpenApi schema to intermediate Schema Model
        /// </summary>
        /// <param name="schema">OpenApi schema</param>
        /// <returns></returns>
        SchemaModel GetSchema(KeyValuePair<string, OpenApiSchema> schema);
    }
}
