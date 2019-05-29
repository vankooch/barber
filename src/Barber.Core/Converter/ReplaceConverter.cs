namespace Barber.Core.Converter
{
    using Barber.Core.Models;
    using Barber.Core.Settings;

    /// <summary>
    /// Replace text
    /// </summary>
    public class ReplaceConverter : IConverter
    {
        public string Name => nameof(ReplaceConverter);

        public string? Convert(string? name, object? options, SchemaModel schemaModel, PropertyModel? propteryModel)
        {
            if (options == null || name == null)
            {
                return name;
            }

            if (options is not MapSettings settings)
            {
                return name;
            }

            return name.Replace(settings.Match, settings.Value);
        }

        public object GetSampleOptions()
            => new MapSettings()
            {
                Match = "ABC",
                Value = "123",
            };
    }
}
