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
        public static ProjectSettings? ReadProjectSettings(string file = "barber.json")
        {
            var json = Converter.CommonHelpers.ReadFile(file);
            if (string.IsNullOrWhiteSpace(json))
            {
                return null;
            }

            return JsonSerializer.Deserialize<ProjectSettings>(json);
        }

        public static SchemaConvetSettings? RunSchemaConverts(this ProjectSettings? settings, List<SchemaModel> schemas, string? jobName = null)
        {
            SchemaConvetSettings? job = null;
            if (settings?.SchemaJobs?.Count > 0)
            {
                if (settings.SchemaJobs.Count == 1 || string.IsNullOrWhiteSpace(jobName))
                {
                    job = settings.SchemaJobs[0];
                }
                else
                {
                    job = settings.SchemaJobs.FirstOrDefault(e => e.Name == jobName);
                }

                if (job == null)
                {
                    return null;
                }

                job.SetSchema(schemas.ToList());
                if (job.Preset == null
                    || job.Preset.Count == 0)
                {
                    return job;
                }

                foreach (var item in job.Preset)
                {
                    var converterSet = settings.Presets.FirstOrDefault(e => e.Name == item);
                    if (converterSet == null)
                    {
                        continue;
                    }

                    job.SetSchema(job.ProcessJobConverters(converterSet));
                }
            }

            return job;
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
