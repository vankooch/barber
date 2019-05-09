namespace Barber.OpenApi.Extensions.Models
{
    using System.Collections.Generic;
    using Microsoft.OpenApi;
    using Microsoft.OpenApi.Interfaces;
    using Microsoft.OpenApi.Writers;

    /// <summary>
    /// ReDoc Vendor extensions
    /// </summary>
    public class OpenApiXTagGroups : IOpenApiExtension
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public OpenApiXTagGroups()
        {
        }

        /// <summary>
        /// Tags names to group
        /// </summary>
        public List<OpenApiXTagGroup> List { get; set; } = new List<OpenApiXTagGroup>();

        /// <inheritdoc />
        public void Write(IOpenApiWriter writer, OpenApiSpecVersion specVersion)
        {
            writer.WriteStartArray();
            foreach (var item in this.List)
            {
                item.Write(writer, specVersion);
            }

            writer.WriteEndArray();
        }
    }
}
