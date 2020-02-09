namespace Barber.OpenApi.Extensions.Models
{
    using Microsoft.OpenApi;
    using Microsoft.OpenApi.Interfaces;
    using Microsoft.OpenApi.Writers;

    /// <summary>
    /// ReDoc Vendor extensions
    /// </summary>
    public class OpenApiXLogo : IOpenApiExtension
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public OpenApiXLogo()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="url">Group Name</param>
        public OpenApiXLogo(string url) => this.Url = url;

        /// <summary>
        /// Background Color. MUST be RGB color in [hexadecimal format]
        /// </summary>
        public string? BackgroundColor { get; set; }

        /// <summary>
        /// Optional URL pointing to the contact page
        /// </summary>
        public string? Link { get; set; }

        /// <summary>
        /// Alternative text
        /// </summary>
        public string? Text { get; set; }

        /// <summary>
        /// Logo Uri
        /// </summary>
        public string? Url { get; set; }

        /// <inheritdoc />
        public void Write(IOpenApiWriter writer, OpenApiSpecVersion specVersion)
        {
            if (writer == null)
            {
                return;
            }

            writer.WriteStartObject();
            writer.WriteProperty("url", this.Url);

            if (!string.IsNullOrEmpty(this.BackgroundColor))
            {
                writer.WriteProperty("backgroundColor", this.Url);
            }

            if (!string.IsNullOrEmpty(this.Text))
            {
                writer.WriteProperty("altText", this.Url);
            }

            if (!string.IsNullOrEmpty(this.Link))
            {
                writer.WriteProperty("href", this.Url);
            }

            writer.WriteEndObject();
        }
    }
}
