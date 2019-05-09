namespace Barber.Cli.Commands.OpenApi
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Barber.Cli.Models;
    using Barber.OpenApi.Generator;
    using Barber.OpenApi.I18next;
    using Barber.OpenApi.Models.Template;
    using ConsoleTables;
    using McMaster.Extensions.CommandLineUtils;
    using Newtonsoft.Json;

    public class ListCommand
    {
        public static void Path(CommandLineApplication config)
        {
            config.HelpOption("-? | -h | --help");
            config.Description = "Paths";
            config.ExtendedHelpText = "List paths";

            var (fileOption, urlOption) = Helper.GetCommonOptions(config);

            config.OnExecute(async () =>
            {
                var settings = Helper.ReadConfiguration(fileOption, urlOption);
                var processor = Helper.GetProcessor(settings);

                // Read OpenAPI file
                await Helper.ReadOpenApi(processor, settings, false);

                // List steps
                var table = new ConsoleTable("#", "Path");
                var count = 0;
                foreach (var item in processor.Api.Paths.OrderBy(e => e.Key))
                {
                    table.AddRow(count, item.Key);
                    count++;
                }

                Helper.WriteTable("Paths", table);

                return 0;
            });
        }

        public static void Register(CommandLineApplication config)
        {
            config.HelpOption("-? | -h | --help");
            config.Description = "Lists";
            config.ExtendedHelpText = "List various elements";
            config.Command("step", Step);
            config.Command("schema", Schema);
            config.Command("property", SchemaProperty);
            config.Command("path", Path);

            config.OnExecute(() =>
            {
                config.ShowHelp();
                return 0;
            });
        }

        public static void Schema(CommandLineApplication config)
        {
            config.HelpOption("-? | -h | --help");
            config.Description = "Schema's";
            config.ExtendedHelpText = "List schema's";

            var (fileOption, urlOption) = Helper.GetCommonOptions(config);

            config.OnExecute(async () =>
            {
                var settings = Helper.ReadConfiguration(fileOption, urlOption);
                var processor = Helper.GetProcessor(settings);

                // Read OpenAPI file
                await Helper.ReadOpenApi(processor, settings, false);

                // List steps
                var table = new ConsoleTable("#", "Name", "Description");
                var count = 0;
                foreach (var item in processor.Api.Components.Schemas.OrderBy(e => e.Key))
                {
                    table.AddRow(count, item.Key, item.Value.Description);
                    count++;
                }

                Helper.WriteTable("Schema's", table);

                return 0;
            });
        }

        public static void SchemaProperty(CommandLineApplication config)
        {
            config.HelpOption("-? | -h | --help");
            config.Description = "Schema properties list for translations";
            config.ExtendedHelpText = "List schema's";

            var (fileOption, urlOption) = Helper.GetCommonOptions(config);

            config.OnExecute(async () =>
            {
                var settings = Helper.ReadConfiguration(fileOption, urlOption);
                var processor = Helper.GetProcessor(settings);

                // Read OpenAPI file
                await Helper.ReadOpenApi(processor, settings, false);

                // Read
                processor.Convert();

                var models = processor.Models;
                var properties = new List<ItemDataModel<List<string>>>();
                foreach (var model in models)
                {
                    if (model.Value.Properties == null || model.Value.Properties.Length <= 0)
                    {
                        continue;
                    }

                    foreach (var property in model.Value.Properties)
                    {
                        var match = properties
                            .FirstOrDefault(e => e.Key == property.Name);
                        if (match != null)
                        {
                            if (!match.Data.Any(e => e == model.Key))
                            {
                                match.Data.Add(model.Key);
                            }
                        }
                        else
                        {
                            if (property.Name.ToLower() == "id")
                            {
                                continue;
                            }

                            if (property.Name.Length > 2)
                            {
                                var lastPart = property.Name.Substring(property.Name.Length - 2);
                                if (lastPart.ToLower() == "id")
                                {
                                    continue;
                                }
                            }

                            properties.Add(new ItemDataModel<List<string>>(property.Name, new List<string>() { model.Key }));
                        }
                    }
                }

                var ns = new NamespaceModel();
                var table = new ConsoleTable("#", "Name", "Schema");
                var count = 0;
                foreach (var item in properties.OrderBy(e => e.Key))
                {
                    table.AddRow(count, item.Key, string.Join(", ", item.Data));
                    ns.Items.Add(new ItemModel(item.Key, item.Key.ToUpper())
                    {
                        ContextFemale = "Female",
                        ContextMale = "Male",
                    });

                    count++;
                }

                Helper.WriteTable("Schema Property", table);

                return 0;
            });
        }

        public static void Step(CommandLineApplication config)
        {
            config.HelpOption("-? | -h | --help");
            config.Description = "Lists";
            config.ExtendedHelpText = "List steps";

            var (fileOption, urlOption) = Helper.GetCommonOptions(config);

            config.OnExecute(async () =>
            {
                var settings = Helper.ReadConfiguration(fileOption, urlOption);
                var processor = Helper.GetProcessor(settings);

                // Read OpenAPI file
                await Helper.ReadOpenApi(processor, settings, false);

                // List steps
                var table = new ConsoleTable("#", "Name", "Generator");
                var count = 0;
                foreach (var item in settings.Steps.OrderBy(e => e.Name))
                {
                    table.AddRow(count, item.Name, item.Generator);
                    count++;
                }

                Helper.WriteTable("Steps", table);

                return 0;
            });
        }
    }
}
