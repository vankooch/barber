namespace Barber.OpenApi.Extensions.Models
{
    using Microsoft.OpenApi;
    using Microsoft.OpenApi.Interfaces;
    using Microsoft.OpenApi.Writers;

    /// <summary>
    /// ReDoc Vendor extensions
    /// </summary>
    public class OpenApiXDisplayName : IOpenApiExtension
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public OpenApiXDisplayName()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Display Name</param>
        public OpenApiXDisplayName(string name) => this.Name = name;

        /// <summary>
        /// Logo Uri
        /// </summary>
        public string Name { get; set; }

        /// <inheritdoc />
        public void Write(IOpenApiWriter writer, OpenApiSpecVersion specVersion)
        {
            writer.WriteValue(this.Name);
        }
    }
}
