namespace Barber.Core.Converter
{
    using Barber.Core.Models;

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
    /// - unknown
    /// </summary>
    public class TypeInternConverter
    {
        public string Name => nameof(TypeInternConverter);

        public string Convert(string? name, PropertyModel? propteryModel)
        {
            var result = TypeNames.UNKNOWN;
            if (name == null
                || propteryModel == null
                || propteryModel.Schema == null)
            {
                return result;
            }

            switch (propteryModel.Schema.Type)
            {
                case TypeNames.ARRAY:
                    result = TypeNames.ARRAY;
                    break;

                case TypeNames.INTEGER when propteryModel.Schema.Format == TypeNames.INT64:
                    result = TypeNames.INT64;
                    break;

                case TypeNames.INTEGER when !string.IsNullOrWhiteSpace(propteryModel.TypeReference):
                    result = TypeNames.ENUM;
                    break;

                case TypeNames.INTEGER:
                    result = TypeNames.INT;
                    break;

                case TypeNames.BOOL:
                    result = TypeNames.BOOL;
                    break;

                case TypeNames.STRING when propteryModel.Schema.Format == TypeNames.DATETIME:
                    result = TypeNames.DATETIME;
                    break;

                case TypeNames.STRING when propteryModel.Schema.Format == TypeNames.UUID:
                    result = TypeNames.UUID;
                    break;

                case TypeNames.STRING when !string.IsNullOrWhiteSpace(propteryModel.TypeReference):
                    result = TypeNames.ENUM;
                    break;

                case TypeNames.STRING:
                    result = TypeNames.STRING;
                    break;

                case TypeNames.OBJECT:
                    result = TypeNames.OBJECT;

                    break;

                default:
                    break;
            }

            return result ?? TypeNames.UNKNOWN;
        }
    }
}
