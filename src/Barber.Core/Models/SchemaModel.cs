namespace Barber.Core.Models
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OpenApi.Models;

    public class SchemaModel
    {
        public string Key { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public List<PropertyModel> Properties { get; set; } = new List<PropertyModel>();

        public List<string> ReferencedSchemas => this.GetReferencedSchemas();

        public OpenApiSchema? Schema { get; set; }

        #region Helper

        public bool HasDateTime => this.HasHelper(TypeNames.DATETIME);

        public bool HasDate => this.HasHelper(TypeNames.DATE);

        public bool HasEnum => this.HasHelper(TypeNames.ENUM);

        public bool HasInt64 => this.HasHelper(TypeNames.INT64);

        public bool HasIntDouble => this.HasHelper(TypeNames.DOUBLE);

        public bool HasObject => this.HasHelper(TypeNames.OBJECT);

        public bool HasUUID => this.HasHelper(TypeNames.UUID);

        #endregion Helper

        private bool HasHelper(string match)
        {
            return this.Properties.Any(e => e.TypeMatched == match);
        }

        private List<string> GetReferencedSchemas()
        {
            return this
                .Properties
                .Where(e => !string.IsNullOrWhiteSpace(e.TypeReference))
                .Select(e => e.TypeReference!)
                .Distinct()
                .ToList();
        }
    }
}
