#pragma warning disable CA1720 // Identifier contains type name
namespace Barber.OpenApi.Settings
{
    public class TypescriptSettingsModel
    {
        public TypescriptSettingsModel()
        {
        }

        /// <summary>
        /// String converting settings
        /// </summary>
        public Types.ArraySettings Array { get; set; } = new Types.ArraySettings();

        /// <summary>
        /// Boolean converting settings
        /// </summary>
        public Types.BooleanSettings Boolean { get; set; } = new Types.BooleanSettings();

        /// <summary>
        /// DateTime converting settings
        /// </summary>
        public Types.DateTimeSettings DateTime { get; set; } = new Types.DateTimeSettings();

        /// <summary>
        /// Integer converting settings
        /// </summary>
        public Types.IntegerSettings Integer { get; set; } = new Types.IntegerSettings();

        /// <summary>
        /// String converting settings
        /// </summary>
        public Types.StringSettings String { get; set; } = new Types.StringSettings();
    }
}
#pragma warning restore CA1720 // Identifier contains type name
