namespace Barber.Core.Converter
{
    using System.Collections.Generic;
    using System.Linq;
    using Barber.Core.Models;
    using Barber.Core.Settings;

    /// <summary>
    /// Default value converter
    /// Known types:
    /// - array
    /// - boolean
    /// - date
    /// - date-time
    /// - uuid
    /// - int
    /// - int64
    /// - double
    /// - number
    /// - string
    /// - object
    /// - enum
    /// - *
    /// </summary>
    public class DefaultValueConverter : IConverter
    {
        public string Name => nameof(DefaultValueConverter);

        public string? Convert(string? name, object? options, SchemaModel schemaModel, PropertyModel? propteryModel)
        {
            if (options == null
                || propteryModel == null
                || propteryModel.Schema == null)
            {
                return name;
            }

            var settings = CommonHelpers.TryGetSettings<List<DefaultValueMapSettings>>(options);
            if (settings == null)
            {
                return name;
            }

            DefaultValueMapSettings? result = null;
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

                case TypeNames.NUMBER when propteryModel.Schema.Format == TypeNames.DOUBLE:
                    result = settings.FirstOrDefault(e => e.Match == TypeNames.DOUBLE);
                    break;

                case TypeNames.NUMBER:
                    result = settings.FirstOrDefault(e => e.Match == TypeNames.NUMBER);
                    if (result == null)
                    {
                        result = settings.FirstOrDefault(e => e.Match == TypeNames.INT);
                    }

                    break;

                case TypeNames.BOOL:
                    result = settings.FirstOrDefault(e => e.Match == TypeNames.BOOL);
                    break;

                case TypeNames.STRING when propteryModel.Schema.Format == TypeNames.DATETIME:
                    result = settings.FirstOrDefault(e => e.Match == TypeNames.DATETIME);
                    break;

                case TypeNames.STRING when propteryModel.Schema.Format == TypeNames.DATE:
                    result = settings.FirstOrDefault(e => e.Match == TypeNames.DATE);
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

            if (result != null)
            {
                return propteryModel.Schema.Nullable ? result.NullableValue : result.Value;
            }

            return name;
        }

        public object GetSampleOptions()
            => new List<DefaultValueMapSettings>
            {
                new DefaultValueMapSettings()
                {
                    Match = TypeNames.ARRAY,
                    Value = "new []",
                    NullableValue = "null",
                },
                new DefaultValueMapSettings()
                {
                    Match = TypeNames.BOOL,
                    Value = "false",
                    NullableValue = "undefined",
                },
                  new DefaultValueMapSettings()
                {
                    Match = TypeNames.DATE,
                    Value = "new Date()",
                    NullableValue = "null",
                },
                new DefaultValueMapSettings()
                {
                    Match = TypeNames.DATETIME,
                    Value = "new Date()",
                    NullableValue = "null",
                },
                new DefaultValueMapSettings()
                {
                    Match = TypeNames.INT,
                    Value = "0",
                    NullableValue = "null",
                },
                new DefaultValueMapSettings()
                {
                    Match = TypeNames.INT64,
                    Value = "0",
                    NullableValue = "null",
                },
                new DefaultValueMapSettings()
                {
                    Match = TypeNames.DOUBLE,
                    Value = "0",
                    NullableValue = "null",
                },
                new DefaultValueMapSettings()
                {
                    Match = TypeNames.NUMBER,
                    Value = "0",
                    NullableValue = "null",
                },
                new DefaultValueMapSettings()
                {
                    Match = TypeNames.STRING,
                    Value = "undefined",
                    NullableValue = "undefined",
                },
                new DefaultValueMapSettings()
                {
                    Match = "*",
                    Value = "undefined",
                    NullableValue = "undefined",
                },
            };
    }
}
