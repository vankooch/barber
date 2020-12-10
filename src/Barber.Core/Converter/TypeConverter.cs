namespace Barber.Core.Converter
{
    using System.Collections.Generic;
    using System.Linq;
    using Barber.Core.Models;
    using Barber.Core.Settings;

    /// <summary>
    /// Type converter
    /// Known types:
    /// - array
    /// - boolean
    /// - date-time
    /// - int
    /// - int64
    /// - string
    /// </summary>
    public class TypeConverter : IConverter
    {
        private const string PATH_SCHEMA = "#/components/schemas/";

        public string Name => nameof(TypeConverter);

        public string? Convert(string? name, object? options, SchemaModel schemaModel, PropertyModel? propteryModel)
        {
            if (options == null
                || name == null
                || propteryModel == null
                || propteryModel.Schema == null)
            {
                return name;
            }

            var settings = CommonHelpers.TryGetSettings<IEnumerable<MapSettings>>(options);
            if (settings == null)
            {
                return name;
            }

            string? result = null;
            switch (propteryModel.Schema.Type)
            {
                case "array":
                    result = GetTypeMapResult(settings, "array");
                    if (!string.IsNullOrWhiteSpace(propteryModel.TypeReference))
                    {
                        result = result?.Replace("TYPE", propteryModel.TypeReference);
                    }

                    break;

                case "integer" when propteryModel.Schema.Format == "int64":
                    result = GetTypeMapResult(settings, "int64");
                    break;

                case "integer":
                    result = GetTypeMapResult(settings, "int");
                    break;

                case "boolean":
                    result = GetTypeMapResult(settings, "boolean");
                    break;

                case "string" when propteryModel.Schema.Format == "date-time":
                    result = GetTypeMapResult(settings, "date-time");
                    break;

                case "string":
                    result = GetTypeMapResult(settings, "string");
                    break;

                case "object":
                    result = propteryModel.TypeReference;

                    break;

                default:
                    break;
            }

            return result ?? name;
        }

        public object GetSampleOptions()
            => new MapSettings[]
                {
                    new MapSettings()
                    {
                        Match = "array",
                        Value = "TYPE[]",
                    },
                    new MapSettings()
                    {
                        Match = "boolean",
                        Value = "boolean",
                    },
                    new MapSettings()
                    {
                        Match = "data-time",
                        Value = "string | Date",
                    },
                    new MapSettings()
                    {
                        Match = "int",
                        Value = "number",
                    },
                    new MapSettings()
                    {
                        Match = "int64",
                        Value = "number",
                    },
                    new MapSettings()
                    {
                        Match = "string",
                        Value = "string",
                    },
                };

        private static string? GetTypeMapResult(IEnumerable<MapSettings> settings, string match)
            => settings.FirstOrDefault(e => e.Match == match)?.Value ?? null;
    }
}
