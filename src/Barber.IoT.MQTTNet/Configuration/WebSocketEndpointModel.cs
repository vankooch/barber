namespace Barber.IoT.MQTTNet.Configuration
{
    using System.Collections.Generic;

    public class WebSocketEndPointModel
    {
        public List<string> AllowedOrigins { get; set; } = new List<string>();

        public bool Enabled { get; set; } = true;

        public int KeepAliveInterval { get; set; } = 120;

        public string Path { get; set; } = "/mqtt";

        public int ReceiveBufferSize { get; set; } = 4096;
    }
}
