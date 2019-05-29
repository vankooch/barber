namespace Barber.Core.Converter
{
    using System.Collections.Generic;
    using System.Linq;
    using Barber.Core.Models;
    using Barber.Core.Settings;

    /// <summary>
    /// Nullable converter
    /// Known types:
    /// - array
    /// - boolean
    /// - date-time
    /// - int
    /// - int64
    /// - string
    /// - *
    /// </summary>
    public class NullableConverter : IConverter
    {
        public string Name => nameof(NullableConverter);

        public string? Convert(string? name, object? options, SchemaModel schemaModel, PropertyModel? propteryModel)
        {
            if (options == null
                || propteryModel == null
                || propteryModel.Schema == null)
            {
                return name;
            }

            if (options is not List<MapSettings> settings)
            {
                return name;
            }

            if (!propteryModel.IsNullable)
            {
                return name;
            }

            MapSettings? result = null;
            switch (propteryModel.Schema.Type)
            {
                case "array":
                    result = settings.FirstOrDefault(e => e.Match == "array");
                    break;

                case "integer" when propteryModel.Schema.Format == "int64":
                    result = settings.FirstOrDefault(e => e.Match == "int64");
                    break;

                case "integer":
                    result = settings.FirstOrDefault(e => e.Match == "int");
                    break;

                case "boolean":
                    result = settings.FirstOrDefault(e => e.Match == "boolean");
                    break;

                case "string" when propteryModel.Schema.Format == "date-time":
                    result = settings.FirstOrDefault(e => e.Match == "date-time");
                    break;

                case "string":
                    result = settings.FirstOrDefault(e => e.Match == "string");
                    break;

                default:
                    break;
            }

            if (result == null)
            {
                result = settings.FirstOrDefault(e => e.Match == "*");
            }

            if (result != null && !string.IsNullOrWhiteSpace(result.Value))
            {
                return $"{name}{result.Value}";
            }

            return name;
        }

        public object GetSampleOptions()
            => new List<MapSettings>
            {
                new MapSettings()
                {
                    Match = "*",
                    Value = "?",
                },
            };
    }
}
