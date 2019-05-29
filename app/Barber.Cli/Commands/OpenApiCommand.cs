namespace Barber.Cli.Commands
{
    using System.Collections.Generic;
    using System.Linq;
    using Barber.Core;
    using Barber.Core.Models;
    using Barber.Core.Settings;
    using ConsoleTables;
    using McMaster.Extensions.CommandLineUtils;

    public static class OpenApiCommand
    {
        public static void CreateProjectSettings(CommandLineApplication config)
        {
            config.HelpOption("-? | -h | --help");
            config.Description = "Generates initial barber.json";
            config.ExtendedHelpText = "Create a project settings file";

            var urlOption = config.Option("-a | --api",
              "URL / Path to OpenAPI File",
              CommandOptionType.SingleValue);

            config.OnExecute(() =>
            {
                var project = ProjectSettings.Default();
                if (urlOption.HasValue())
                {
                    project.Api = urlOption.Value();
                }

                project.WriteProjectSettings();

                return 0;
            });
        }

        public static void List(CommandLineApplication config)
        {
            config.HelpOption("-? | -h | --help");
            config.Description = "Schema's";
            config.ExtendedHelpText = "List schema's";

            var projectFile = config.Option("-p | --project",
               "Project settings file location",
               CommandOptionType.SingleValue);

            var urlOption = config.Option("-a | --api",
               "URL / Path to OpenAPI File",
               CommandOptionType.SingleValue);

            var stepName = config.Option("-s | --step",
               "Run only given schema job",
               CommandOptionType.SingleValue);

            var schemaOnly = config.Option("--schema",
               "Only list schema",
               CommandOptionType.NoValue);

            config.OnExecuteAsync(async cancellationToken =>
            {
                var project = ProjectSettingsExtensions.ReadProjectSettings(projectFile.HasValue() ? projectFile.Value() : "barber.json");
                if (project == null)
                {
                    System.Console.WriteLine("Could not read project file");
                    return 1;
                }

                if (string.IsNullOrWhiteSpace(project.Api) && !urlOption.HasValue())
                {
                    System.Console.WriteLine("No path to OpenApi spec found.");
                    return 1;
                }

                // Read OpenAPI file
                var document = await DocumentExtensions.ReadDocument(urlOption.Value() ?? project.Api, "en-US", cancellationToken);
                var schemas = document.ReadSchemas();

                // Convert
                var job = project.RunSchemaConverts(schemas, stepName.Value());

                // List steps
                if (schemaOnly.HasValue())
                {
                    ListSchemas(schemas);
                }
                else
                {
                    ListProperties(schemas);
                }

                return 0;
            });
        }

        public static void Run(CommandLineApplication config)
        {
            config.HelpOption("-? | -h | --help");
            config.Description = "Schema's";
            config.ExtendedHelpText = "List schema's";

            var projectFile = config.Option("-p | --project",
               "Project settings file location",
               CommandOptionType.SingleValue);

            var urlOption = config.Option("-a | --api",
               "URL / Path to OpenAPI File",
               CommandOptionType.SingleValue);

            var stepName = config.Option("-s | --step",
               "Run only given schema job",
               CommandOptionType.SingleValue);

            config.OnExecuteAsync(async cancellationToken =>
            {
                var project = ProjectSettingsExtensions.ReadProjectSettings(projectFile.HasValue() ? projectFile.Value() : "barber.json");
                if (project == null)
                {
                    System.Console.WriteLine("Could not read project file");
                    return 1;
                }

                if (string.IsNullOrWhiteSpace(project.Api) && !urlOption.HasValue())
                {
                    System.Console.WriteLine("No path to OpenApi spec found.");
                    return 1;
                }

                // Read OpenAPI file
                var document = await DocumentExtensions.ReadDocument(urlOption.Value() ?? project.Api, "en-US", cancellationToken);
                var schemas = document.ReadSchemas();

                // Convert
                var job = project.RunSchemaConverts(schemas, stepName.Value());

                // List
                schemas.ListProperties();

                job = await job.RenderFileContent();
                job.WriteFiles();

                return 0;
            });
        }

        private static void ListProperties(this IList<SchemaModel> schemas)
        {
            var table = new ConsoleTable("#", "Schema Key", "Schema Name", "Property Key", "Property Name", "Type", "Default Value");
            var count = 0;
            foreach (var item in schemas.OrderBy(e => e.Key))
            {
                count++;
                table.AddRow(count, item.Key, item.Name, string.Empty, string.Empty, string.Empty, string.Empty);
                foreach (var prop in item.Properties)
                {
                    table.AddRow(count, item.Key, item.Name, prop.Key, prop.Name, prop.Type, prop.DefaultValue);
                }
            }

            table.WriteTableToConsole();
        }

        private static void ListSchemas(this IList<SchemaModel> schemas)
        {
            var table = new ConsoleTable("#", "Key", "Name", "Description");
            var count = 0;
            foreach (var item in schemas.OrderBy(e => e.Key))
            {
                table.AddRow(count, item.Key, item.Name, item.Schema.Description);
                count++;
            }

            table.WriteTableToConsole();
        }
    }
}
