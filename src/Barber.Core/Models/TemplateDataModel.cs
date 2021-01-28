namespace Barber.Core.Models
{
    using System.Collections.Generic;
    using System.Linq;
    using Barber.Core.Settings;

    public class TemplateDataModel
    {
        private List<ReferencedSchemasModel> _referencedSchemas = new List<ReferencedSchemasModel>();

        public SchemaModel Schema { get; set; } = new SchemaModel();

        public List<SchemaModel> Schemas { get; set; } = new List<SchemaModel>();

        public SchemaConvetSettings Step { get; set; } = new SchemaConvetSettings();

        public List<ReferencedSchemasModel> ReferencedSchemas => this._referencedSchemas;

        public List<string>? Extra => this.Step.Extra;

        #region Helper

        public bool HasDateTime => this.HasHelper(TypeNames.DATETIME);

        public bool HasDate => this.HasHelper(TypeNames.DATE);

        public bool HasEnum => this.HasHelper(TypeNames.ENUM);

        public bool HasInt64 => this.HasHelper(TypeNames.INT64);

        public bool HasIntDouble => this.HasHelper(TypeNames.DOUBLE);

        public bool HasObject => this.HasHelper(TypeNames.OBJECT);

        public bool HasUUID => this.HasHelper(TypeNames.UUID);

        #endregion Helper

        public List<string> GetReferencedSchemasKeys()
        {
            if (this.Schemas.Count > 0)
            {
                return this.Schemas
                    .SelectMany(e => e.ReferencedSchemas)
                    .Distinct()
                    .ToList();
            }
            else
            {
                return this.Schema
                    .ReferencedSchemas
                    .Distinct()
                    .ToList();
            }
        }

        public List<ReferencedSchemasModel> GetReferencedSchemas(ProjectSettings? project)
        {
            var list = new List<ReferencedSchemasItemModel>();
            var refKeys = this.GetReferencedSchemasKeys();
            if (refKeys?.Count > 0 && project?.SchemaJobs?.Count > 0)
            {
                list = this.AddMatchs(project, refKeys);
                this._referencedSchemas = list
                    .GroupBy(e => e.File)
                    .Select(e => new ReferencedSchemasModel()
                    {
                        FileFull = e.Key,
                        Imports = e
                            .Select(f => new ReferencedSchemasItemModel(f.Key, f.Name))
                            .ToList(),
                    })
                    .ToList();
            }

            return this._referencedSchemas;
        }

        private List<ReferencedSchemasItemModel> AddMatchs(ProjectSettings? project, List<string>? refKeys)
        {
            var result = new List<ReferencedSchemasItemModel>();
            if (refKeys?.Count > 0 && project?.SchemaJobs?.Count > 0)
            {
                var nextRun = new List<string>();
                for (var i = 0; i < refKeys.Count; i++)
                {
                    var key = refKeys[i];
                    for (var j = 0; j < project.SchemaJobs.Count; j++)
                    {
                        var schemas = project.SchemaJobs[j].GetSchemas();
                        var match = schemas?.FirstOrDefault(e => e.Name == key);
                        if (match != null)
                        {
                            result.Add(new ReferencedSchemasItemModel()
                            {
                                Key = key,
                                File = project.SchemaJobs[j].Filename ?? string.Empty,
                                Name = match.Name,
                            });

                            if (match.Name != match.Key && match.Key != key)
                            {
                                nextRun.Add(match.Key);
                            }
                        }
                    }
                }

                if (nextRun.Count > 0)
                {
                    result.AddRange(this.AddMatchs(project, nextRun));
                }
            }

            return result;
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
