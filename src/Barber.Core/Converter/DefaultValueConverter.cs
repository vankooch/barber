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
    /// - date-time
    /// - int
    /// - int64
    /// - string
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
                    Match = "array",
                    Value = "new []",
                    NullableValue = "null",
                },
                new DefaultValueMapSettings()
                {
                    Match = "boolean",
                    Value = "false",
                    NullableValue = "undefined",
                },
                new DefaultValueMapSettings()
                {
                    Match = "date-time",
                    Value = "new Date()",
                    NullableValue = "null",
                },
                new DefaultValueMapSettings()
                {
                    Match = "int",
                    Value = "0",
                    NullableValue = "null",
                },
                new DefaultValueMapSettings()
                {
                    Match = "int64",
                    Value = "0",
                    NullableValue = "null",
                },
                new DefaultValueMapSettings()
                {
                    Match = "string",
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
