namespace Barber.OpenApi.Generator
{
    using System.Collections.Generic;
    using System.Linq;
    using Barber.OpenApi.Models.Template;
    using Microsoft.OpenApi.Models;

    /// <inheritdoc />
    public class TypescriptModel : IGenerator, IGeneratorSchema
    {
        private readonly IGeneratorTypes _generator;
        private readonly Settings.TypescriptSettingsModel _settings;

        public TypescriptModel()
        {
            this._settings = new Settings.TypescriptSettingsModel();
            this._generator = new Typescript();
        }

        public TypescriptModel(Settings.SettingsModel settings)
        {
            if (settings != null)
            {
                this._settings = settings.Typescript;
                this._generator = new Typescript(settings);
            }
        }

        /// <inheritdoc />
        public string Name => nameof(TypescriptModel);

        /// <inheritdoc />
        public GeneratorType Type => GeneratorType.Schema;

        /// <inheritdoc />
        public PropertyModel GetProperty(
            KeyValuePair<string, OpenApiSchema> schema,
            KeyValuePair<string, OpenApiSchema> property) => new PropertyModel()
            {
                Name = property.Key,
                Type = this._generator.ConvertType(property.Value),
                DefaultValue = this._generator.GetDefaultValue(property.Value),
                Description = property.Value.Description,
                Nullable = this._generator.IsNullable(property.Value),
                Reference = this._generator.GetReference(property.Value),
                Required = schema.Value.Required.Any(e => e == property.Key),
                RootSchema = schema.Key,
                Schema = property.Value,
                Title = property.Value.Title,
            };

        /// <inheritdoc />
        public SchemaModel GetSchema(
            KeyValuePair<string, OpenApiSchema> schema) => new SchemaModel()
            {
                Name = schema.Key,
                File = new Models.FileModel()
                {
                    Name = $"{schema.Key}.ts",
                    Path = string.Empty,
                },
            };
    }
}
