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
        public string GetMediaType(KeyValuePair<string, OpenApiMediaType> mediaType)
        {
            return this._generator.ConvertType(mediaType.Value.Schema, false);
        }

        /// <inheritdoc />
        public PropertyModel GetParameter(OpenApiParameter parameter) => new PropertyModel()
        {
            Name = parameter.Name,
            Type = this._generator.ConvertType(parameter.Schema),
            Required = parameter.Required,
        };

        /// <inheritdoc />
        public PathModel GetPath(
            KeyValuePair<string, OpenApiPathItem> path,
            KeyValuePair<OperationType, OpenApiOperation> operation)
        {
            // Name
            var name = operation.Value.OperationId.Replace(operation.Value.Tags.First().Name, string.Empty);
            name = name.Substring(0, 1).ToLower() + name.Substring(1);

            return new PathModel()
            {
                Name = name,
                Path = path.Key,
                Tags = operation.Value.Tags.Select(e => e.Name).ToArray(),
                Type = operation.Key.ToString().ToLower(),
            };
        }

        /// <inheritdoc />
        public ServiceModel GetService(string tag, IEnumerable<PathModel> paths)
        {
            var match = paths.Where(e => e.Tags.Any(a => a == tag));
            if (match?.Count() > 0)
            {
                return new ServiceModel()
                {
                    Name = tag,
                    File = new Models.FileModel()
                    {
                        Name = $"{tag.Substring(0, 1).ToUpper()}{tag.Substring(1)}Service.ts",
                        Path = string.Empty,
                    },
                    Paths = match.ToArray(),
                };
            }

            return null;
        }
    }
}
