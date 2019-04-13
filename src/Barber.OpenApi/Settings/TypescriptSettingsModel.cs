namespace Barber.OpenApi.Settings
{
    public class TypescriptSettingsModel
    {
        public TypescriptSettingsModel()
        {
        }

        /// <summary>
        /// Starting with Typescript 3.2 there is support for BigInt
        /// </summary>
        public bool UseBigInt { get; set; } = false;
    }
}
