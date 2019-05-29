namespace Barber.Core.Converter
{
    using Barber.Core.Models;

    public class SuffixConverter : IConverter
    {
        public string Name => nameof(SuffixConverter);

        public string? Convert(string? name, object? options, SchemaModel schemaModel, PropertyModel? propteryModel)
            => $"{name}{options}";

        public object GetSampleOptions() => "I";
    }
}
