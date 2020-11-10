namespace Barber.IoT.Data.Model
{
    using Barber.IoT.Authentication.Models;
    using Barber.IoT.Data.Enums;

    public class DeviceActivity : DeviceActivityModel<string>
    {
        public DeviceActivity()
            : base()
        {
        }

        public DeviceActivity(string deviceId, int state, int code, string payload)
            : base(deviceId, state, code, payload)
        {
        }

        public DeviceActivity(string deviceId, DeviceActivityStateType state, int code, string payload)
        {
            this.DeviceId = deviceId;
            this.State = (int)state;
            this.Code = code;
            this.Payload = payload;
        }
    }
}
