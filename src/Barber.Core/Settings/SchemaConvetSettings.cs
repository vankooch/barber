namespace Barber.Core.Settings
{
    using System.Collections.Generic;
    using Barber.Core.Models;

    public class SchemaConvetSettings
    {
        private List<FileModel> _files = new List<FileModel>();
        private List<SchemaModel> _schemas = new List<SchemaModel>();

        public string? Destination { get; set; }

        /// <summary>
        /// Only used when IsSingleFile is set true
        /// </summary>
        public string? Filename { get; set; }

        public bool IsSingleFile { get; set; }

        public string? Name { get; set; }

        public List<string> Preset { get; set; } = new List<string>();

        public List<string> Schemas { get; set; } = new List<string>();

        public List<string> SchemasSkip { get; set; } = new List<string>();

        public List<string>? Extra { get; set; }

        public string Template { get; set; } = "interface-single.mustache";

        public string TemplateType { get; set; } = "mustache";

        public List<FileModel>? GetFiles() => this._files ?? null;

        public List<SchemaModel>? GetSchemas() => this._schemas ?? null;

        public void SetFiles(List<FileModel> files)
        {
            this._files = files;
        }

        public void SetSchema(List<SchemaModel> schema)
        {
            this._schemas = schema;
        }
    }
}
