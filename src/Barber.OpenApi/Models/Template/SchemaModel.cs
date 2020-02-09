namespace Barber.OpenApi.Models.Template
{
    using System.Collections.Generic;
    using Barber.OpenApi.Models;
    using Microsoft.OpenApi.Models;

    public class SchemaModel : BaseModel, IRenderModel
    {
        public SchemaModel()
        {
        }

        public FileModel? File { get; set; } = null;

        public bool HasForm => this.PropertiesFiltered?.Count > 0;

        public IReadOnlyList<PropertyModel>? Properties { get; set; } = null;

        public IReadOnlyList<PropertyModel>? PropertiesFiltered { get; set; } = null;

        public OpenApiSchema? Schema { get; set; }
    }
}
