namespace Barber.OpenApi.Settings.Types
{
    public class BooleanSettings : BaseModel
    {
        public BooleanSettings()
        {
            this.Default = "false";
            this.Type = "boolean";
        }
    }
}
