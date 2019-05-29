namespace Barber.Core.Settings
{
    public class ConverterParameterSettings
    {
        public string Name { get; set; } = string.Empty;

        public object? Option { get; set; }

        public string PropertyFilter { get; set; } = string.Empty;

        public string SchemaFilter { get; set; } = string.Empty;
    }
}
