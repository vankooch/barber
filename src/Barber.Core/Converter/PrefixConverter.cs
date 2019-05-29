namespace Barber.Core.Converter
{
    using Barber.Core.Models;

    public class PrefixConverter : IConverter
    {
        public string Name => nameof(PrefixConverter);

        public string? Convert(string? name, object? options, SchemaModel schemaModel, PropertyModel? propteryModel)
            => $"{options}{name}";

        public object GetSampleOptions() => "I";
    }
}
