namespace Barber.OpenApi.Settings.Types
{
    public class IntegerSettings : BaseModel
    {
        public IntegerSettings()
        {
            this.Default = "0";
            this.Type = "number";
        }

        /// <summary>
        /// Starting with Typescript 3.2 there is support for BigInt
        /// </summary>
        public bool UseBigInt { get; set; } = false;
    }
}
