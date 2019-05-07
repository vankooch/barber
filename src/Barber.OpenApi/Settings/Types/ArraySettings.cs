namespace Barber.OpenApi.Settings.Types
{
    public class ArraySettings : BaseModel
    {
        public ArraySettings()
        {
            this.Default = "[]";
            this.Type = "TYPE[]";
        }
    }
}
