namespace Barber.OpenApi.Models.Template
{
    using Barber.OpenApi.Models;
    using Microsoft.OpenApi.Models;

    public class SchemaModel : BaseModel, IRenderModel
    {
        public SchemaModel()
        {
        }

        public FileModel File { get; set; } = null;

        public PropertyModel[] Properties { get; set; } = null;

        public PropertyModel[] PropertiesFiltered { get; set; } = null;

        public OpenApiSchema Schema { get; set; }
    }
}
