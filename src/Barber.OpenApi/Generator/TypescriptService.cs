namespace Barber.OpenApi.Generator
{
    using System.Collections.Generic;
    using System.Linq;
    using Barber.OpenApi.Models.Template;
    using Microsoft.OpenApi.Models;

    /// <inheritdoc />
    public class TypescriptService : IGenerator, IGeneratorPaths
    {
        private readonly IGeneratorTypes _generator;

        public TypescriptService() => this._generator = new Typescript();

        /// <inheritdoc />
        public string Name => nameof(TypescriptService);

        /// <inheritdoc />
        public GeneratorType Type => GeneratorType.Path;

        /// <inheritdoc />
        public string? GetMediaType(KeyValuePair<string, OpenApiMediaType>? mediaType) => this._generator.ConvertType(mediaType?.Value.Schema, false) ?? string.Empty;

        /// <inheritdoc />
        public PropertyModel? GetParameter(OpenApiParameter? parameter)
        {
            if (parameter == null)
            {
                return null;
            }

            return new PropertyModel()
            {
                Name = parameter?.Name ?? string.Empty,
                Type = this._generator.ConvertType(parameter?.Schema) ?? string.Empty,
                Required = parameter?.Required ?? false,
            };
        }

        /// <inheritdoc />
        public PathModel? GetPath(
            KeyValuePair<string, OpenApiPathItem>? path,
            KeyValuePair<OperationType, OpenApiOperation>? operation)
        {
            if (path == null
                || operation == null
                || !path.HasValue
                || !operation.HasValue)
            {
                return null;
            }

            // Name
            var name = operation.Value.Value.OperationId.Replace(operation.Value.Value.Tags.First().Name, string.Empty);
            name = name.Substring(0, 1).ToLowerInvariant() + name.Substring(1);

            return new PathModel()
            {
                Name = name,
                Path = path.Value.Key,
                Tags = operation.Value.Value.Tags.Select(e => e.Name).ToArray(),
                Type = operation.Value.Key.ToString().ToLowerInvariant(),
            };
        }

        /// <inheritdoc />
        public ServiceModel? GetService(string? tag, IEnumerable<PathModel>? paths)
        {
            var match = paths?.Where(e => e.Tags.Any(a => a == tag));
            if (match?.Count() > 0)
            {
                return new ServiceModel()
                {
                    Name = tag ?? string.Empty,
                    File = new Models.FileModel()
                    {
                        Name = $"{tag?.Substring(0, 1).ToUpperInvariant()}{tag?.Substring(1)}Service.ts",
                        Path = string.Empty,
                    },
                    Paths = match.ToArray(),
                };
            }

            return null;
        }
    }
}
