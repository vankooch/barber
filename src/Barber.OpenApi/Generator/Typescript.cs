namespace Barber.OpenApi.Generator
{
    using System;
    using System.IO;
    using Microsoft.OpenApi.Models;

    /// <summary>
    /// Common converter for generics and types
    /// </summary>
    public class Typescript : IGeneratorTypes
    {
        private readonly Settings.TypescriptSettingsModel _settings = new Settings.TypescriptSettingsModel();

        public Typescript()
        {
        }

        public Typescript(Settings.SettingsModel settings)
        {
            if (settings != null)
            {
                this._settings = settings.Typescript;
            }
        }

        /// <inheritdoc />
        public string? ConvertType(OpenApiSchema? propertyItem, bool setNull = true)
        {
            if (propertyItem == null)
            {
                throw new ArgumentNullException(nameof(propertyItem));
            }

            string restult;
            var isNullType = true;
            var hasNull = false;
            switch (propertyItem.Type)
            {
                case "array":
                    if (!string.IsNullOrEmpty(propertyItem.Items?.Reference?.Id))
                    {
                        restult = this._settings.Array.Type.Replace("TYPE", propertyItem.Items?.Reference?.Id);
                    }
                    else
                    {
                        restult = this._settings.Array.Type.Replace("TYPE", this.ConvertType(propertyItem.Items));
                    }

                    hasNull = true;
                    break;

                case "integer" when propertyItem.Format == "int64" && this._settings.Integer.UseBigInt:
                    restult = "bigint";
                    break;

                case "integer":
                    restult = this._settings.Integer.Type;
                    break;

                case "boolean":
                    restult = this._settings.Boolean.Type;
                    isNullType = false;
                    break;

                case "string" when propertyItem.Format == "date-time":
                    restult = this._settings.DateTime.Type;
                    break;

                case "string":
                    restult = this._settings.String.Type;
                    isNullType = false;
                    break;

                case "object":
                    restult = propertyItem.Reference.Id;
                    hasNull = true;
                    break;

                default:
                    restult = propertyItem.Type;
                    isNullType = false;
                    break;
            }

            if (setNull && (propertyItem.Nullable || hasNull))
            {
                restult += " | " + (isNullType ? "null" : "undefined");
            }

            return restult;
        }

        /// <inheritdoc />
        public string? GetDefaultValue(OpenApiSchema? propertyItem)
        {
            if (propertyItem == null)
            {
                throw new ArgumentNullException(nameof(propertyItem));
            }

            if (propertyItem.Nullable)
            {
                return "null";
            }

            return propertyItem.Type switch
            {
                "array" => this._settings.Array.Default.Replace("TYPE", string.Empty),
                "integer" when propertyItem.Format == "int64" && this._settings.Integer.UseBigInt => "0n",
                "integer" => this._settings.Integer.Default,
                "boolean" => this._settings.Boolean.Default,
                "string" when propertyItem.Format == "date-time" => this._settings.DateTime.Default,
                "string" => this._settings.String.Default,
                "object" => $"new {propertyItem.Reference?.Id}()",
                _ => "undefined",
            };
        }

        /// <inheritdoc />
        public string? GetReference(OpenApiSchema? propertyItem)
        {
            if (propertyItem == null)
            {
                throw new ArgumentNullException(nameof(propertyItem));
            }

            return propertyItem.Type switch
            {
                "array" when !string.IsNullOrEmpty(propertyItem.Items?.Reference?.Id) => propertyItem.Items?.Reference?.Id,
                "object" => propertyItem.Reference?.Id,
                _ => string.Empty,
            };
        }

        /// <inheritdoc />
        public string? GetReferencePath(string? relativePath)
        {
            var ext = Path.GetExtension(relativePath);
            var file = relativePath?.Substring(0, relativePath.Length - ext.Length);

            return $"./{file?.Replace("\\", "/")}";
        }

        /// <inheritdoc />
        public bool IsNullable(OpenApiSchema? propertyItem)
        {
            if (propertyItem == null)
            {
                throw new ArgumentNullException(nameof(propertyItem));
            }

            if (propertyItem.Nullable)
            {
                return true;
            }

            switch (propertyItem.Type)
            {
                case "array":
                case "boolean":
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
