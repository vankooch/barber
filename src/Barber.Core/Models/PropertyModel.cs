namespace Barber.Core.Models
{
    using Microsoft.OpenApi.Models;

    public class PropertyModel
    {
        public string? DefaultValue { get; set; }

        public string? Description { get; set; }

        public bool IsNullable { get; set; } = false;

        public bool IsRequired { get; set; } = false;

        public string Key { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public OpenApiSchema? Schema { get; set; }

        public string Type { get; set; } = string.Empty;

        public string TypeMatched { get; set; } = string.Empty;

        public string? TypeReference { get; set; }
    }
}
