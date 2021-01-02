namespace Barber.Core
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.Json;
    using Barber.Core.Models;
    using Barber.Core.Settings;

    public static class ProjectSettingsExtensions
    {
        public static ConverterOrderSettings? GetPreset(this ProjectSettings? settings, string? name)
        {
            if (settings == null || string.IsNullOrWhiteSpace(name))
            {
                return null;
            }

            var converterSet = settings.Presets.FirstOrDefault(e => e.Name == name);
            if (converterSet == null)
            {
                return null;
            }

            if (!string.IsNullOrWhiteSpace(converterSet.Extends))
            {
                var extends = settings.Presets.FirstOrDefault(e => e.Name == converterSet.Extends);
                if (extends != null)
                {
                    if (!string.IsNullOrWhiteSpace(extends.Extends))
                    {
                        extends = settings.GetPreset(extends.Name)!;
                    }

                    var result = new ConverterOrderSettings()
                    {
                        Name = converterSet.Name,
                        Extends = converterSet.Extends,
                        PropertyDefaultValueConverter = extends.PropertyDefaultValueConverter.Concat(converterSet.PropertyDefaultValueConverter).ToList(),
                        PropertyNameConverter = extends.PropertyNameConverter.Concat(converterSet.PropertyNameConverter).ToList(),
                        PropertyTypeConverter = extends.PropertyTypeConverter.Concat(converterSet.PropertyTypeConverter).ToList(),
                        SchemaNameConverter = extends.SchemaNameConverter.Concat(converterSet.SchemaNameConverter).ToList(),
                    };

                    return result;
                }
            }

            return converterSet;
        }

        public static ProjectSettings? ReadProjectSettings(string file = "barber.json")
        {
            var json = Converter.CommonHelpers.ReadFile(file);
            if (string.IsNullOrWhiteSpace(json))
            {
                return null;
            }

            var options = new JsonSerializerOptions
            {
                ReadCommentHandling = JsonCommentHandling.Skip,
            };

            return JsonSerializer.Deserialize<ProjectSettings>(json, options);
        }

        public static ProjectSettings? RunSchemaConverts(this ProjectSettings? settings, List<SchemaModel> schemas)
        {
            if (settings?.SchemaJobs?.Count > 0)
            {
                for (var i = 0; i < settings.SchemaJobs.Count; i++)
                {
                    var job = settings.SchemaJobs[i];
                    job.SetSchema(schemas.ToList());

                    for (var j = 0; j < job.Preset.Count; j++)
                    {
                        var converterSet = settings.GetPreset(job.Preset[j]);
                        if (converterSet == null)
                        {
                            continue;
                        }

                        job.SetSchema(job.ProcessJobConverters(converterSet));
                    }
                }
            }

            return settings;
        }

        public static void WriteProjectSettings(this ProjectSettings? me, string file = "barber.json")
        {
            if (me == null)
            {
                return;
            }

            if (!Path.IsPathRooted(file))
            {
                file = Path.Combine(Directory.GetCurrentDirectory(), file);
            }

            var json = JsonSerializer.Serialize(me, new JsonSerializerOptions()
            {
                WriteIndented = true,
            });
            File.WriteAllText(file, json);
        }
    }
}
