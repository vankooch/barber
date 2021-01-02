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
    /// - uuid
    /// - int
    /// - int64
    /// - string
    /// - object
    /// - enum
    /// </summary>
    public class TypeConverter : IConverter
    {
        private const string PATH_SCHEMA = "#/components/schemas/";

        public string Name => nameof(TypeConverter);

        public string? Convert(string? name, object? options, SchemaModel schemaModel, PropertyModel? propteryModel)
        {
            if (options == null
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
                case TypeNames.ARRAY:
                    result = GetTypeMapResult(settings, TypeNames.ARRAY);
                    if (!string.IsNullOrWhiteSpace(propteryModel.TypeReference))
                    {
                        result = result?.Replace("TYPE", propteryModel.TypeReference);
                    }
                    else
                    {
                        result = result?.Replace("TYPE", string.Empty);
                    }

                    break;

                case TypeNames.INTEGER when propteryModel.Schema.Format == TypeNames.INT64:
                    result = GetTypeMapResult(settings, TypeNames.INT64);
                    break;

                case TypeNames.INTEGER when !string.IsNullOrWhiteSpace(propteryModel.TypeReference):
                    result = GetTypeMapResult(settings, TypeNames.ENUM);
                    break;

                case TypeNames.INTEGER:
                    result = GetTypeMapResult(settings, TypeNames.INT);
                    break;

                case TypeNames.BOOL:
                    result = GetTypeMapResult(settings, TypeNames.BOOL);
                    break;

                case TypeNames.STRING when propteryModel.Schema.Format == TypeNames.DATETIME:
                    result = GetTypeMapResult(settings, TypeNames.DATETIME);
                    break;

                case TypeNames.STRING when propteryModel.Schema.Format == TypeNames.UUID:
                    result = GetTypeMapResult(settings, TypeNames.UUID);
                    break;

                case TypeNames.STRING when !string.IsNullOrWhiteSpace(propteryModel.TypeReference):
                    result = GetTypeMapResult(settings, TypeNames.ENUM);
                    result = result?.Replace("TYPE", propteryModel.TypeReference);

                    break;

                case TypeNames.STRING:
                    result = GetTypeMapResult(settings, TypeNames.STRING);

                    break;

                case TypeNames.OBJECT:
                    result = GetTypeMapResult(settings, TypeNames.OBJECT);
                    result = result?.Replace("TYPE", propteryModel.TypeReference);

                    break;

                default:
                    break;
            }

            if (result == null)
            {
                result = GetTypeMapResult(settings, "*");
            }

            // Try match exact
            if (result == null && !string.IsNullOrWhiteSpace(propteryModel.TypeReference))
            {
                result = GetTypeMapResult(settings, propteryModel.TypeReference);
            }

            return result ?? name;
        }

        public object GetSampleOptions()
            => new MapSettings[]
                {
                    new MapSettings()
                    {
                        Match = TypeNames.ARRAY,
                        Value = "TYPE[]",
                    },
                    new MapSettings()
                    {
                        Match = TypeNames.BOOL,
                        Value = TypeNames.BOOL,
                    },
                    new MapSettings()
                    {
                        Match = TypeNames.DATETIME,
                        Value = "string | Date",
                    },
                    new MapSettings()
                    {
                        Match = TypeNames.UUID,
                        Value = TypeNames.STRING,
                    },
                    new MapSettings()
                    {
                        Match = TypeNames.INT,
                        Value = "number",
                    },
                    new MapSettings()
                    {
                        Match = TypeNames.INT64,
                        Value = "number",
                    },
                    new MapSettings()
                    {
                        Match = TypeNames.STRING,
                        Value = TypeNames.STRING,
                    },
                    new MapSettings()
                    {
                        Match = TypeNames.ENUM,
                        Value = "TYPE",
                    },
                    new MapSettings()
                    {
                        Match = TypeNames.OBJECT,
                        Value = "TYPE",
                    },
                };

        private static string? GetTypeMapResult(IEnumerable<MapSettings> settings, string match)
            => settings.FirstOrDefault(e => e.Match == match)?.Value ?? null;
    }
}
