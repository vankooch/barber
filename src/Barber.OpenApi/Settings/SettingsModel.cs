﻿namespace Barber.OpenApi.Settings
{
    using Newtonsoft.Json;

    public class SettingsModel
    {
        public SettingsModel()
        {
        }

        /// <summary>
        /// Which body Media Type is used for setting the Parameter Type
        /// </summary>
        /// <remarks>
        /// Since a API can have multiple allowed request types,
        /// we need to set which one we what to use for code generation.
        /// If no match could be find we use the first one in definition.
        /// </remarks>
        public string BodyMediaType { get; set; } = "application/json";

        /// <summary>
        /// Is API Versioning used
        /// </summary>
        /// <remarks>
        /// In case you have URI like this
        /// "api/v{version}/controller/action/{id}"
        /// you need to set this option true
        /// </remarks>
        public bool HasApiVersion { get; set; } = true;

        /// <summary>
        /// Which Response Codes are use to set Return Types
        /// </summary>
        /// <remarks>
        /// Codes are matched in given order. First match will win.
        /// </remarks>
        public string[] ResponseCodes { get; set; } = new string[] { "200", "201" };

        /// <summary>
        /// Which body Media Type is used for setting the Return Type
        /// </summary>
        /// <remarks>
        /// Since a API can have multiple allowed response types,
        /// we need to set which one we what to use for code generation.
        /// If no match could be find we use the first one in definition.
        /// </remarks>
        public string ResponseMediaType { get; set; } = "application/json";

        /// <summary>
        /// Schema specific configuration
        /// </summary>
        public SchemaItemModel[] SchemaConfig { get; set; }

        /// <summary>
        /// Properties which are filtered out
        /// </summary>
        public string[] SkipProperties { get; set; }

        /// <summary>
        /// Schema's to Skip
        /// </summary>
        public string[] SkipSchemas { get; set; }

        /// <summary>
        /// Tag to Skip
        /// </summary>
        public string[] SkipTags { get; set; }

        /// <summary>
        /// Steps
        /// </summary>
        public StepModel[] Steps { get; set; }

        /// <summary>
        /// I18next
        /// </summary>
        public I18nModel[] I18n { get; set; }

        /// <summary>
        /// Template Root Path to templates, can be relative from assembly root
        /// </summary>
        public string TemplateRoot { get; set; } = "Templates";

        /// <summary>
        /// Typescript Generator Settings
        /// </summary>
        public TypescriptSettingsModel Typescript { get; set; } = new TypescriptSettingsModel();

        /// <summary>
        /// URL / Path to OpenAPI 3 File
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Create model from settings
        /// </summary>
        /// <param name="json">JSON Text</param>
        /// <returns></returns>
        public static SettingsModel FromJson(string json) => JsonConvert.DeserializeObject<SettingsModel>(json);

        /// <summary>
        /// Create JSON Object
        /// </summary>
        /// <param name="model">Settings Model</param>
        /// <returns></returns>
        public static string ToJson(SettingsModel model) => JsonConvert.SerializeObject(model, Formatting.Indented);
    }
}
