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
    /// - uuid
    /// - int
    /// - int64
    /// - string
    /// - object
    /// - enum
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

            var settings = CommonHelpers.TryGetSettings<List<MapSettings>>(options);
            if (settings == null)
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
                case TypeNames.ARRAY:
                    result = settings.FirstOrDefault(e => e.Match == TypeNames.ARRAY);
                    break;

                case TypeNames.INTEGER when propteryModel.Schema.Format == TypeNames.INT64:
                    result = settings.FirstOrDefault(e => e.Match == TypeNames.INT64);
                    break;

                case TypeNames.INTEGER when !string.IsNullOrWhiteSpace(propteryModel.TypeReference):
                    result = settings.FirstOrDefault(e => e.Match == TypeNames.ENUM);
                    break;

                case TypeNames.INTEGER:
                    result = settings.FirstOrDefault(e => e.Match == TypeNames.INT);
                    break;

                case TypeNames.BOOL:
                    result = settings.FirstOrDefault(e => e.Match == TypeNames.BOOL);
                    break;

                case TypeNames.STRING when propteryModel.Schema.Format == TypeNames.DATETIME:
                    result = settings.FirstOrDefault(e => e.Match == TypeNames.DATETIME);
                    break;

                case TypeNames.STRING when propteryModel.Schema.Format == TypeNames.UUID:
                    result = settings.FirstOrDefault(e => e.Match == TypeNames.UUID);
                    break;

                case TypeNames.STRING when !string.IsNullOrWhiteSpace(propteryModel.TypeReference):
                    result = settings.FirstOrDefault(e => e.Match == TypeNames.ENUM);
                    break;

                case TypeNames.STRING:
                    result = settings.FirstOrDefault(e => e.Match == TypeNames.STRING);
                    break;

                case TypeNames.OBJECT:
                    var match = settings.FirstOrDefault(e => e.Match == propteryModel.TypeReference);
                    if (match == null)
                    {
                        result = settings.FirstOrDefault(e => e.Match == TypeNames.OBJECT);
                    }
                    else
                    {
                        result = match;
                    }

                    break;

                default:
                    break;
            }

            // Try match exact
            if (result == null)
            {
                result = settings.FirstOrDefault(e => e.Match == propteryModel.TypeReference);
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
