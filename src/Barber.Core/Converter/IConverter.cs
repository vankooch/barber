namespace Barber.Core.Converter
{
    using Barber.Core.Models;

    public interface IConverter
    {
        string Name { get; }

        string? Convert(string? name, object? options, SchemaModel schemaModel, PropertyModel? propteryModel);

        object GetSampleOptions();
    }
}
