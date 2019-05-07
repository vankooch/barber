namespace Barber.OpenApi.Generator
{
    using System.IO;
    using Microsoft.OpenApi.Models;

    /// <summary>
    /// Common converter for generics and types
    /// </summary>
    public class Typescript : IGeneratorTypes
    {
        private readonly Settings.TypescriptSettingsModel _settings;

        public Typescript() => this._settings = new Settings.TypescriptSettingsModel();

        public Typescript(Settings.SettingsModel settings) => this._settings = settings.Typescript;

        /// <inheritdoc />
        public string ConvertType(OpenApiSchema property, bool setNull = true)
        {
            var restult = string.Empty;
            var isNullType = true;
            var hasNull = false;
            switch (property.Type)
            {
                case "array":
                    if (!string.IsNullOrEmpty(property.Items?.Reference?.Id))
                    {
                        restult = this._settings.Array.Type.Replace("TYPE", property.Items.Reference.Id + "[]");
                    }
                    else
                    {
                        restult = this._settings.Array.Type.Replace("TYPE", this.ConvertType(property.Items) + "[]");
                    }

                    hasNull = true;
                    break;

                case "integer" when property.Format == "int64" && this._settings.Integer.UseBigInt:
                    restult = "bigint";
                    break;

                case "integer":
                    restult = this._settings.Integer.Type;
                    break;

                case "object":
                    restult = property.Reference?.Id;
                    hasNull = true;
                    break;

                case "string" when property.Format == "date-time":
                    restult = this._settings.DateTime.Type;
                    break;

                case "string":
                    restult = this._settings.String.Type;
                    isNullType = false;
                    break;

                default:
                    restult = property.Type;
                    isNullType = false;
                    break;
            }

            if (setNull && (property.Nullable || hasNull))
            {
                restult += " | " + (isNullType ? "null" : "undefined");
            }

            return restult;
        }

        /// <inheritdoc />
        public string GetDefaultValue(OpenApiSchema property)
        {
            if (property.Nullable)
            {
                return "null";
            }

            switch (property.Type)
            {
                case "array":
                    return this._settings.Array.Default.Replace("TYPE", string.Empty);

                case "integer" when property.Format == "int64" && this._settings.Integer.UseBigInt:
                    return "0n";

                case "integer":
                    return this._settings.Integer.Default;

                case "object":
                    return $"new {property.Reference?.Id}()";

                case "string" when property.Format == "date-time":
                    return this._settings.DateTime.Default;

                case "string":
                    return this._settings.String.Default;

                default:
                    return "undefined";
            }
        }

        /// <inheritdoc />
        public string GetReference(OpenApiSchema property)
        {
            switch (property?.Type)
            {
                case "array" when !string.IsNullOrEmpty(property.Items?.Reference?.Id):

                    return property.Items.Reference?.Id;

                case "object":
                    return property.Reference?.Id;

                default:
                    return string.Empty;
            }
        }

        /// <inheritdoc />
        public string GetReferencePath(string relativePath)
        {
            var ext = Path.GetExtension(relativePath);
            var file = relativePath.Substring(0, relativePath.Length - ext.Length);

            return $"./{file.Replace("\\", "/")}";
        }

        /// <inheritdoc />
        public bool IsNullable(OpenApiSchema property)
        {
            if (property.Nullable)
            {
                return true;
            }

            switch (property.Type)
            {
                case "array":
                case "integer":
                case "object":
                case "string":
                    return false;

                default:
                    return true;
            }
        }
    }
}
