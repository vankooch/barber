namespace Barber.Core.Models
{
    using System.Collections.Generic;
    using System.Linq;
    using Barber.Core.Settings;

    public class TemplateDataModel
    {
        private List<ReferencedSchemasModel> _referencedSchemas = new List<ReferencedSchemasModel>();

        public List<string>? Extra => this.Step.Extra;

        public List<ReferencedSchemasModel> ReferencedSchemas => this._referencedSchemas;

        public SchemaModel Schema { get; set; } = new SchemaModel();

        public List<SchemaModel> Schemas { get; set; } = new List<SchemaModel>();

        public SchemaConvetSettings Step { get; set; } = new SchemaConvetSettings();

        #region Helper

        public bool HasDate => this.HasHelper(TypeNames.DATE);

        public bool HasDateTime => this.HasHelper(TypeNames.DATETIME);

        public bool HasEnum => this.HasHelper(TypeNames.ENUM);

        public bool HasInt64 => this.HasHelper(TypeNames.INT64);

        public bool HasIntDouble => this.HasHelper(TypeNames.DOUBLE);

        public bool HasObject => this.HasHelper(TypeNames.OBJECT);

        public bool HasUUID => this.HasHelper(TypeNames.UUID);

        #endregion Helper

        public List<ReferencedSchemasModel> GetReferencedSchemas(ProjectSettings? project)
        {
            var list = new List<ReferencedSchemasItemModel>();
            var refKeys = this.GetReferencedSchemasKeys();
            if (refKeys?.Count > 0 && project?.SchemaJobs?.Count > 0)
            {
                list = this.AddMatchs(project, refKeys);
                this._referencedSchemas = list
                    .GroupBy(e => e.File)
                    .Select(e =>
                    {
                        var imports = new List<ReferencedSchemasItemModel>();
                        foreach (var item in e)
                        {
                            if (!imports.Any(f => f.Name == item.Name && f.Key == item.Key))
                            {
                                imports.Add(new ReferencedSchemasItemModel(item.Key, item.Name));
                            }
                        }

                        return new ReferencedSchemasModel()
                        {
                            FileFull = e.Key,
                            Imports = imports.OrderBy(e => e.Name).ToList(),
                        };
                    })
                    .OrderBy(e => e.File)
                    .ToList();
            }

            return this._referencedSchemas;
        }

        public List<string> GetReferencedSchemasKeys()
        {
            if (this.Schemas.Count > 0)
            {
                var localKeys = this.Schemas.Select(e => e.Name).ToArray();

                return this.Schemas
                    .SelectMany(e => e.ReferencedSchemas)
                    .Where(e => !localKeys.Contains(e))
                    .Distinct()
                    .ToList();
            }
            else
            {
                return this.Schema
                    .ReferencedSchemas
                    .Where(e => e != this.Schema.Name)
                    .Distinct()
                    .ToList();
            }
        }

        private List<ReferencedSchemasItemModel> AddMatchs(ProjectSettings? project, List<string>? refKeys)
        {
            var result = new List<ReferencedSchemasItemModel>();
            if (refKeys?.Count > 0 && project?.SchemaJobs?.Count > 0)
            {
                for (var i = 0; i < refKeys.Count; i++)
                {
                    var key = refKeys[i];
                    var match = this.FindRefByName(project, key);
                    if (match != null)
                    {
                        result.Add(match);
                    }
                }
            }

            return result;
        }

        private ReferencedSchemasItemModel? FindRefByName(ProjectSettings? project, string refKey)
        {
            if (!string.IsNullOrWhiteSpace(refKey) && project?.SchemaJobs?.Count > 0)
            {
                for (var j = 0; j < project.SchemaJobs.Count; j++)
                {
                    var schemas = project.SchemaJobs[j].GetSchemas();
                    var match = schemas?.FirstOrDefault(e => e.Name == refKey);
                    if (match != null)
                    {
                        return new ReferencedSchemasItemModel()
                        {
                            Key = refKey,
                            File = project.SchemaJobs[j].Filename ?? string.Empty,
                            Name = match.Name,
                        };
                    }
                }
            }

            return null;
        }

        private bool HasHelper(string match)
        {
            if (this.Schemas.Count > 0)
            {
                return this.Schemas.Any(e => e.Properties.Any(f => f.TypeMatched == match));
            }
            else
            {
                return this.Schema.Properties.Any(e => e.TypeMatched == match);
            }
        }
    }
}
