namespace Barber.Core.Converter
{
    using System.Collections.Generic;
    using Barber.Core.Models;
    using Barber.Core.Settings;

    public class DefaultValueTypeConverter : IConverter
    {
        public string Name => nameof(DefaultValueTypeConverter);

        public string? Convert(string? name, object? options, SchemaModel schemaModel, PropertyModel? propteryModel)
        {
            if (options == null
                || propteryModel == null
                || propteryModel.Schema == null
                || string.IsNullOrWhiteSpace(propteryModel.TypeReference))
            {
                return string.Empty;
            }

            switch (propteryModel.Schema.Type)
            {
                case TypeNames.OBJECT:

                    return propteryModel.Schema.Reference.Id;

                default:
                    break;
            }

            return string.Empty;
        }

        public object GetSampleOptions()
            => new List<DefaultValueMapSettings>();
    }
}
