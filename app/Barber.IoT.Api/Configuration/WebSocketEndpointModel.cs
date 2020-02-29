namespace Barber.IoT.Api.Configuration
{
    using System.Collections.Generic;

    public class WebSocketEndPointModel
    {
        public bool Enabled { get; set; } = true;

        public string Path { get; set; } = "/mqtt";

        public int ReceiveBufferSize { get; set; } = 4096;

        public int KeepAliveInterval { get; set; } = 120;

        public List<string> AllowedOrigins { get; set; } = new List<string>();
    }
}
