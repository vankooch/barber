namespace Barber.Core.Settings
{
    using System.Collections.Generic;

    public class ConverterOrderSettings
    {
        public string? Extends { get; set; }

        public string Name { get; set; } = string.Empty;

        public List<ConverterParameterSettings> PropertyDefaultValueConverter { get; set; } = new List<ConverterParameterSettings>();

        public List<ConverterParameterSettings> PropertyDefaultValueTypeConverter { get; set; } = new List<ConverterParameterSettings>();

        public List<ConverterParameterSettings> PropertyNameConverter { get; set; } = new List<ConverterParameterSettings>();

        public List<ConverterParameterSettings> PropertyTypeConverter { get; set; } = new List<ConverterParameterSettings>();

        public List<ConverterParameterSettings> SchemaNameConverter { get; set; } = new List<ConverterParameterSettings>();

        public List<BlockPropertiesModel> Skips { get; set; } = new List<BlockPropertiesModel>();
    }
}
