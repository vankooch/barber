namespace Barber.Core
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Barber.Core.Converter;
    using Barber.Core.Models;
    using Barber.Core.Renderer;
    using Barber.Core.Settings;

    public static class SchemaExtensions
    {
        private static readonly IConverter[] _converters =
            {
             new PrefixConverter(),
             new SuffixConverter(),
             new ReplaceConverter(),
             new TypeConverter(),
             new DefaultValueConverter(),
             new NullableConverter(),
            };

        public static List<SchemaModel> ProcessJobConverters(this SchemaConvetSettings job, ConverterOrderSettings options)
        {
            var schemas = job.GetSchemas();
            if (schemas == null
               || schemas.Count == 0
               || options == null
               )
            {
                return schemas ?? new List<SchemaModel>();
            }

            var typeMatcher = new TypeInternConverter();
            Action<SchemaModel> schemaAction = (schema) =>
            {
                schema.Name = options.SchemaNameConverter.RunConverters(schema.Name, schema, null) ?? schema.Key;
            };

            Action<SchemaModel, PropertyModel> propertyAction = (schema, property) =>
            {
                property.Name = options.PropertyNameConverter.RunConverters(property.Name, schema, property) ?? property.Key;
                property.TypeMatched = typeMatcher.Convert(property.Type, property);
            };

            // First process names
            schemas = LoopShim(schemas, job, options, schemaAction, propertyAction);

            schemaAction = (_) => { };
            propertyAction = (schema, property) =>
            {
                property = schemas.ReferenceNameConvert(property);
                property.Type = options.PropertyTypeConverter.RunConverters(property.Type, schema, property) ?? string.Empty;
                property.DefaultValue = options.PropertyDefaultValueConverter.RunConverters(property.DefaultValue, schema, property);
            };

            // Now process rest
            schemas = LoopShim(schemas, job, options, schemaAction, propertyAction);

            return schemas;
        }

        public static async Task<SchemaConvetSettings> RenderFileContent(this SchemaConvetSettings job, ProjectSettings? project)
        {
            var result = new List<FileModel>();
            var schemas = job.GetSchemas();
            if (schemas == null
              || schemas.Count == 0
              || job == null
              )
            {
                return job ?? new SchemaConvetSettings();
            }

            var template = CommonHelpers.UpdatePath(job.Template);
            if (string.IsNullOrWhiteSpace(template))
            {
                throw new ArgumentNullException($"Could not find template file in: {job.Template}");
            }

            var render = new MustacheRenderer();
            var data = new TemplateDataModel()
            {
                Step = job,
            };
            if (job.IsSingleFile)
            {
                data.Schemas = schemas;
                data.GetReferencedSchemas(project);
                var content = await render.Render(template, data);
                result.Add(new FileModel()
                {
                    Key = job.Name ?? "SingleFile",
                    Content = content,
                });
            }
            else
            {
                foreach (var item in schemas)
                {
                    data.Schema = item;
                    data.GetReferencedSchemas(project);
                    result.Add(new FileModel()
                    {
                        Key = item.Key,
                        Content = await render.Render(template, data),
                    });
                }
            }

            job.SetFiles(result);

            return job;
        }

        public static void WriteFiles(this SchemaConvetSettings job)
        {
            if (job == null)
            {
                return;
            }

            var files = job.GetFiles();
            var schemas = job.GetSchemas();
            if (files == null)
            {
                return;
            }

            for (var i = 0; i < files.Count; i++)
            {
                var path = job.Destination ?? string.Empty;
                if (job.IsSingleFile)
                {
                    path = Path.Combine(path, job.Filename ?? string.Empty);
                }
                else
                {
                    var schema = schemas?.FirstOrDefault(e => e.Key == files[i].Key);
                    if (schema == null)
                    {
                        path = null;
                        continue;
                    }

                    path = Path.Combine(path, schema.Name);
                }

                if (!string.IsNullOrWhiteSpace(files[i].Content)
                    && !string.IsNullOrWhiteSpace(path))
                {
                    // Check if path exits
                    var directory = Path.GetDirectoryName(path);
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }

                    File.WriteAllText(path, files[i].Content);
                }
            }
        }

        private static List<SchemaModel> LoopShim(
            List<SchemaModel> schemas,
            SchemaConvetSettings job,
            ConverterOrderSettings options,
            Action<SchemaModel> schemaAction,
            Action<SchemaModel, PropertyModel> propertyAction)
        {
            if (schemas == null
                || schemas.Count == 0
                || options == null
                )
            {
                return schemas ?? new List<SchemaModel>();
            }

            var newSchemas = new List<SchemaModel>();
            foreach (var schema in schemas)
            {
                if (job?.Schemas?.Count > 0
                    && !job
                    .Schemas
                    .Any(e => !string.IsNullOrWhiteSpace(e) && e.Trim() == schema.Key))
                {
                    continue;
                }

                if (job?.SchemasSkip?.Count > 0
                    && job
                    .SchemasSkip
                    .Any(e => !string.IsNullOrWhiteSpace(e) && e.Trim() == schema.Key))
                {
                    continue;
                }

                // Action
                var local = new SchemaModel()
                {
                    Key = schema.Key,
                    Name = schema.Name,
                    Properties = schema.Properties,
                    Schema = schema.Schema,
                };
                schemaAction(local);

                if (local.Properties != null
                    && local.Properties.Count > 0)
                {
                    foreach (var property in local.Properties)
                    {
                        // Action
                        propertyAction(local, property);
                    }
                }

                newSchemas.Add(local);
            }

            return newSchemas;
        }

        private static string? RunConverters(
            this List<ConverterParameterSettings> converters,
            string? name,
            SchemaModel schemaModel,
            PropertyModel? propteryModel)
        {
            if (converters == null
                || converters.Count == 0)
            {
                return name;
            }

            for (var i = 0; i < converters.Count; i++)
            {
                var options = converters[i];
                if (!string.IsNullOrWhiteSpace(options.SchemaFilter)
                    && options.SchemaFilter != schemaModel.Key)
                {
                    continue;
                }

                if (!string.IsNullOrWhiteSpace(options.PropertyFilter)
                    && options.PropertyFilter != propteryModel?.Key)
                {
                    continue;
                }

                var converter = _converters
                    .FirstOrDefault(e => e.Name == options.Name);
                if (converter == null)
                {
                    continue;
                }

                name = converter.Convert(name, options.Option, schemaModel, propteryModel);
            }

            return name;
        }
    }
}
