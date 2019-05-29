namespace Barber.Core.Models
{
    using System.Collections.Generic;
    using Microsoft.OpenApi.Models;

    public class SchemaModel
    {
        public string Key { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public List<PropertyModel> Properties { get; set; } = new List<PropertyModel>();

        public List<string> ReferencedSchemas { get; set; } = new List<string>();

        public OpenApiSchema? Schema { get; set; }
    }
}
