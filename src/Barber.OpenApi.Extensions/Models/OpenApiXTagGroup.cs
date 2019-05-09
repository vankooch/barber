namespace Barber.OpenApi.Extensions.Models
{
    using System.Collections.Generic;
    using Microsoft.OpenApi;
    using Microsoft.OpenApi.Interfaces;
    using Microsoft.OpenApi.Writers;

    /// <summary>
    /// ReDoc Vendor extensions
    /// </summary>
    public class OpenApiXTagGroup : IOpenApiExtension
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public OpenApiXTagGroup()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Group Name</param>
        public OpenApiXTagGroup(string name) => this.Name = name;

        /// <summary>
        /// Group Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Tags names to group
        /// </summary>
        public List<string> Tags { get; set; } = new List<string>();

        /// <inheritdoc />
        public void Write(IOpenApiWriter writer, OpenApiSpecVersion specVersion)
        {
            writer.WriteStartObject();
            writer.WriteProperty("name", this.Name);
            writer.WritePropertyName("tags");
            writer.WriteStartArray();
            foreach (var item in this.Tags)
            {
                writer.WriteValue(item);
            }

            writer.WriteEndArray();
            writer.WriteEndObject();
        }
    }
}
