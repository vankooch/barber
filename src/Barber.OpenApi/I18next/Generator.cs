namespace Barber.OpenApi.I18next
{
    using Microsoft.OpenApi.Models;

    public static class Generator
    {
        public static NamespaceModel ReadSchemas(OpenApiDocument api)
        {
            var ns = new NamespaceModel();
            if (api?.Components?.Schemas?.Count > 0)
            {
                foreach (var schema in api.Components.Schemas)
                {
                    if (schema.Value?.Properties?.Count > 0)
                    {
                        foreach (var property in schema.Value.Properties)
                        {
                            if (!string.IsNullOrEmpty(property.Value.Title) && !string.IsNullOrEmpty(property.Value.Title))
                            {
                                ns.Add(property.Key, property.Value.Title);
                            }

                            if (!string.IsNullOrEmpty(property.Value.Description) && !string.IsNullOrEmpty(property.Value.Description))
                            {
                                ns.Add(property.Key + "_description", property.Value.Description);
                            }
                        }
                    }
                }
            }

            return ns;
        }
    }
}
