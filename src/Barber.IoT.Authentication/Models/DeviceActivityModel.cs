#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.

namespace Barber.IoT.Authentication.Models
{
    using System;

    public class DeviceActivityModel<TKey> where TKey : IEquatable<TKey>
    {
        public DeviceActivityModel()
        {
        }

        public DeviceActivityModel(TKey deviceId, int state, int code, string payload)
        {
            this.DeviceId = deviceId;
            this.State = state;
            this.Code = code;
            this.Payload = payload;
        }

        /// <summary>
        /// Message code
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// Message date
        /// </summary>
        public DateTime Date { get; set; } = DateTime.Now;

        /// <summary>
        /// Device Id
        /// </summary>
        public TKey DeviceId { get; set; }

        public long Id { get; set; }

        /// <summary>
        /// Payload data
        /// </summary>
        public string? Payload { get; set; }

        /// <summary>
        /// State indicator
        /// </summary>
        public int State { get; set; }
    }
}

#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
