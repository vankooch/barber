namespace Barber.Cli.Commands
{
    using System;
    using System.IO;
    using Barber.OpenApi;
    using Barber.OpenApi.Generator;
    using Barber.OpenApi.Settings;
    using McMaster.Extensions.CommandLineUtils;

    public static class OpenApi
    {
        private const string SETTINGS_NAME = "barber-openapi.json";

        public static void ConfigurationCreate(CommandLineApplication config)
        {
            config.HelpOption("-? | -h | --help");
            config.Description = "Create default configuration file";
            config.ExtendedHelpText = "Run this to create a new configuration file in current directory";

            var fileOption = config.Option("-f | --file",
                "Set configuration file name",
                CommandOptionType.SingleValue);

            config.OnExecute(() =>
            {
                var file = SETTINGS_NAME;
                if (fileOption.HasValue())
                {
                    file = fileOption.Value().Trim();
                }

                file = Path.Combine(Directory.GetCurrentDirectory(), file);
                var path = Path.GetDirectoryName(file);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                var settings = new SettingsModel
                {
                    Steps = new StepModel[]
                    {
                            new StepModel()
                            {
                                Name = "Models",
                                Destination = "codegen-models",
                                Generator = "TypescriptModel",
                                Template = "model.mustache"
                            },
                            new StepModel()
                            {
                                Name = "Services",
                                Destination = "codegen-services",
                                Generator = "TypescriptService",
                                Template = "service.mustache",
                                Resolve = "Models"
                            }
                    }
                };

                File.WriteAllText(file, SettingsModel.ToJson(settings));

                return 0;
            });
        }

        public static void Register(CommandLineApplication config)
        {
            config.HelpOption("-? | -h | --help");
            config.Description = "Generate code based on OpenAPI specification";
            config.ExtendedHelpText = "Reads OpenAPI file and generates code";
            config.Command("init", ConfigurationCreate);

            var fileOption = config.Option("-f | --file",
                "Set configuration file name",
                CommandOptionType.SingleValue);

            var stepOption = config.Option("-s | --step",
                "Run given step only",
                CommandOptionType.SingleValue);

            var openApiOption = config.Option("-a | --api",
                "URL / Path to OpenAPI File",
                CommandOptionType.SingleValue);

            var dryOption = config.Option("-d | --dry",
                "Dry run without writing to disk",
                CommandOptionType.NoValue);

            config.OnExecute(async () =>
            {
                var file = SETTINGS_NAME;
                if (fileOption.HasValue())
                {
                    file = fileOption.Value();
                }

                file = Path.Combine(Directory.GetCurrentDirectory(), file);
                if (!File.Exists(file))
                {
                    throw new Exception("Could not find Settings File in: " + file);
                }

                var settings = SettingsModel.FromJson(File.ReadAllText(file));
                var api = settings.Url;
                if (openApiOption.HasValue())
                {
                    api = openApiOption.Value();
                }

                // Setup
                var processor = new Processor(new Typescript(settings), settings);
                processor.Generators.Add(new TypescriptModel(settings));
                processor.Generators.Add(new TypescriptService(settings));

                // Read OpenAPI file
                var sw = LogHelper.TaskStart($"Reading from {api}");
                await processor.Read(api);
                LogHelper.TaskStop(sw, true);

                if (settings.Steps == null || settings.Steps.Length == 0)
                {
                    return 0;
                }

                // Process Steps
                foreach (var step in settings.Steps)
                {
                    sw = LogHelper.TaskStart($"Convert Step: {step.Name}");
                    processor.Convert(step);
                    LogHelper.TaskStop(sw, true, step.Result?.Count);
                }

                // Update References
                sw = LogHelper.TaskStart($"Update references");
                await processor.Update();
                LogHelper.TaskStop(sw, true);

                if (!dryOption.HasValue())
                {
                    var singleStep = stepOption.HasValue();
                    var singleStepName = singleStep ? stepOption.Value().Trim().ToLower() : string.Empty;

                    // Write Steps
                    foreach (var step in settings.Steps)
                    {
                        if (singleStep && singleStepName != step.Name.Trim().ToLower())
                        {
                            continue;
                        }

                        sw = LogHelper.TaskStart($"Write Step: {step.Name}");
                        processor.Write(step.Result);
                        LogHelper.TaskStop(sw, true, step.Result.Count);
                    }
                }

                return 0;
            });
        }
    }
}
