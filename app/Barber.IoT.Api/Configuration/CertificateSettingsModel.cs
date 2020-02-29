namespace Barber.IoT.Api.Configuration
{
    using System.IO;

    public class CertificateSettingsModel
    {
        /// <summary>
        /// Path to certificate.
        /// </summary>
        public string Path { get; set; } = string.Empty;

        /// <summary>
        /// Password of certificate.
        /// </summary>
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Read certificate file.
        /// </summary>
        public byte[] ReadCertificate()
        {
            if (string.IsNullOrEmpty(this.Path) || string.IsNullOrWhiteSpace(this.Path))
            {
                throw new FileNotFoundException("No path set");
            }

            if (!File.Exists(this.Path))
            {
                throw new FileNotFoundException($"Could not find Certificate in path: {this.Path}");
            }

            return File.ReadAllBytes(this.Path);
        }
    }
}
