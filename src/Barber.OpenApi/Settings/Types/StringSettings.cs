namespace Barber.OpenApi.Settings.Types
{
    public class StringSettings : BaseModel
    {
        public StringSettings()
        {
            this.Default = "undefined";
            this.Type = "string";
        }
    }
}
