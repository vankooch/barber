﻿namespace Barber.IoT.MQTTNet.Configuration
{
    public class RetainedApplicationMessagesModel
    {
        public bool Persist { get; set; } = false;

        public int WriteInterval { get; set; } = 10;

        public string Path { get; set; } = string.Empty;
    }
}
