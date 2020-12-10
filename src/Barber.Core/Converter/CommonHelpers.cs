namespace Barber.Core.Converter
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.Json;
    using Barber.Core.Models;

    public static class CommonHelpers
    {
        private const string PATH_SCHEMA = "#/components/schemas/";

        public static T? TryGetSettings<T>(object options)
        {
            var json = JsonSerializer.Serialize(options);
            return JsonSerializer.Deserialize<T>(json);
        }

        public static string? GetReferenceName(this List<SchemaModel> schemas, string? key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return key;
            }

            var match = schemas?
                .FirstOrDefault(e => e.Key == key)?
                .Name;

            return match ?? key;
        }

        public static string? UpdatePath(string file)
        {
            if (string.IsNullOrWhiteSpace(file))
            {
                return null;
            }

            if (!File.Exists(file))
            {
                file = Path.Combine(Directory.GetCurrentDirectory(), file);
                if (!File.Exists(file))
                {
                    return null;
                }
            }

            return file;
        }

        public static string? ReadFile(string file)
        {
            if (string.IsNullOrWhiteSpace(file))
            {
                return null;
            }

            if (!File.Exists(file))
            {
                file = Path.Combine(Directory.GetCurrentDirectory(), file);
                if (!File.Exists(file))
                {
                    return null;
                }
            }

            return File.ReadAllText(file);
        }

        public static PropertyModel ReferenceNameConvert(this List<SchemaModel> schemas, PropertyModel propteryModel)
        {
            if (propteryModel.Schema == null)
            {
                return propteryModel;
            }

            switch (propteryModel.Schema.Type)
            {
                case "array":
                    propteryModel.TypeReference = propteryModel.TypeReference?.Replace(PATH_SCHEMA, string.Empty);
                    propteryModel.TypeReference = schemas.GetReferenceName(propteryModel.TypeReference);

                    break;

                case "object":
                    propteryModel.TypeReference = propteryModel.TypeReference?.Replace(PATH_SCHEMA, string.Empty);
                    propteryModel.TypeReference = schemas.GetReferenceName(propteryModel.TypeReference);

                    break;

                default:
                    break;
            }

            return propteryModel;
        }
    }
}
