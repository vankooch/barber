namespace Barber.OpenApi.Extensions.SchemaFilter
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using Microsoft.OpenApi.Models;
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
            if (schema.Properties?.Count > 0)
            {
                var propertyInfos = context.Type.GetProperties();

                foreach (var item in schema.Properties)
                {
                    var name = item.Key[0].ToString().ToUpper() + item.Key.Substring(1);
                    var propertyMatch = propertyInfos.FirstOrDefault(e => e.Name == name);
                    if (propertyMatch != null)
                    {
                        var attibutes = propertyMatch.GetCustomAttributes(typeof(DisplayNameAttribute), true);
                        if (attibutes?.Count() > 0)
                        {
                            var attibute = attibutes.First() as DisplayNameAttribute;
                            item.Value.Title = attibute?.DisplayName;
                        }

                        attibutes = propertyMatch.GetCustomAttributes(typeof(DisplayAttribute), true);
                        if (attibutes?.Count() > 0)
                        {
                            if (!(attibutes.First() is DisplayAttribute attibute))
                            {
                                continue;
                            }

                            if (attibute.ResourceType != null)
                            {
                                item.Value.Title = attibute.GetName();
                                var description = attibute.GetDescription();
                                if (!string.IsNullOrEmpty(description) && !string.IsNullOrWhiteSpace(description))
                                {
                                    item.Value.Description = description;
                                }
                            }
                            else
                            {
                                item.Value.Title = attibute.Name;
                            }
                        }
                    }
                }
            }

            return;
        }
    }
}
