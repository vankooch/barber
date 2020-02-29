namespace Barber.IoT.Api.Configuration
{
    using System.Net;

    /// <summary>
    /// Listen Entry Settings Model
    /// </summary>
    public class TcpEndPointModel
    {
        /// <summary>
        /// Certificate settings.
        /// </summary>
        public CertificateSettingsModel? Certificate { get; set; }

        /// <summary>
        /// Enabled / Disable
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// Listen Address
        /// </summary>
        public string IPv4 { get; set; } = string.Empty;

        /// <summary>
        /// Listen Address
        /// </summary>
        public string IPv6 { get; set; } = string.Empty;

        /// <summary>
        /// Listen Port
        /// </summary>
        public int Port { get; set; } = 1883;

        /// <summary>
        /// Read IPv4
        /// </summary>
        /// <returns></returns>
        public bool TryReadIPv4(out IPAddress address)
        {
            if (this.IPv4 == "*")
            {
                address = IPAddress.Any;
                return true;
            }

            if (this.IPv4 == "localhost")
            {
                address = IPAddress.Loopback;
                return true;
            }

            if (this.IPv4 == "disable")
            {
                address = IPAddress.None;
                return true;
            }

            if (IPAddress.TryParse(this.IPv4, out var ip))
            {
                address = ip;
                return true;
            }

            throw new System.Exception($"Could not parse IPv4 address: {this.IPv4}");
        }

        /// <summary>
        /// Read IPv6
        /// </summary>
        /// <returns></returns>
        public bool TryReadIPv6(out IPAddress address)
        {
            if (this.IPv6 == "*")
            {
                address = IPAddress.IPv6Any;
                return true;
            }

            if (this.IPv6 == "localhost")
            {
                address = IPAddress.IPv6Loopback;
                return true;
            }

            if (this.IPv6 == "disable")
            {
                address = IPAddress.None;
                return true;
            }

            if (IPAddress.TryParse(this.IPv6, out var ip))
            {
                address = ip;
                return true;
            }

            throw new System.Exception($"Could not parse IPv6 address: {this.IPv6}");
        }
    }
}
