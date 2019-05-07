namespace Barber.OpenApi.Settings.Types
{
    public class DateTimeSettings : BaseModel
    {
        public DateTimeSettings()
        {
            this.Default = "new Date()";
            this.Type = "string | Date";
        }
    }
}
