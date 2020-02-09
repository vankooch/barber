namespace Barber.OpenApi.Extensions.SchemaFilter
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Linq;
    using Microsoft.OpenApi.Models;
    using Newtonsoft.Json;
    using Swashbuckle.AspNetCore.SwaggerGen;

    /// <summary>
    /// Add display names to schema definitions
    /// </summary>
    public class DisplaySchemaFilter : ISchemaFilter
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public DisplaySchemaFilter()
        {
        }

        /// <summary>
        /// Apply filter
        /// </summary>
        /// <param name="schema">OpenApiSchema</param>
        /// <param name="context">SchemaFilterContext</param>
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (schema == null || schema.Properties == null || context == null)
            {
                return;
            }

            if (schema.Properties?.Count > 0)
            {
                var propertyInfos = context.Type.GetProperties();
                var dict = propertyInfos
                        .ToLookup(
                            e => (JsonPropertyAttribute)e.GetCustomAttributes(typeof(JsonPropertyAttribute), true).FirstOrDefault(),
                            e => e.Name)
                        .Where(e => e.Key != null && e.Key != default)
                        .ToLookup(e => e?.Key?.PropertyName, e => e.FirstOrDefault());

                foreach (var item in schema.Properties)
                {
                    var name = item.Key[0].ToString(CultureInfo.CurrentCulture).ToUpperInvariant() + item.Key.Substring(1);
                    var propertyMatch = propertyInfos.FirstOrDefault(e => e.Name == name);
                    if (propertyMatch == null)
                    {
                        var match = dict.FirstOrDefault(e => e.Key == item.Key)?.FirstOrDefault();
                        if (!string.IsNullOrEmpty(match))
                        {
                            propertyMatch = propertyInfos.FirstOrDefault(e => e.Name == match);
                        }
                    }

                    if (propertyMatch != null)
                    {
                        var attibutes = propertyMatch.GetCustomAttributes(typeof(DisplayNameAttribute), true);
                        if (attibutes?.Length > 0)
                        {
                            var attibute = attibutes.First() as DisplayNameAttribute;
                            item.Value.Title = attibute?.DisplayName;
                        }

                        attibutes = propertyMatch.GetCustomAttributes(typeof(DisplayAttribute), true);
                        if (attibutes?.Length > 0)
                        {
                            if (!(attibutes.First() is DisplayAttribute attibute))
                            {
                                continue;
                            }

                            var title = attibute.GetName() ?? attibute.Name;
                            if (!string.IsNullOrEmpty(title) && !string.IsNullOrWhiteSpace(title))
                            {
                                item.Value.Title = title;
                            }

                            var description = attibute.GetDescription() ?? attibute.Description;
                            if (!string.IsNullOrEmpty(description) && !string.IsNullOrWhiteSpace(description))
                            {
                                item.Value.Description = description;
                            }
                        }
                    }
                }
            }

            return;
        }
    }
}
