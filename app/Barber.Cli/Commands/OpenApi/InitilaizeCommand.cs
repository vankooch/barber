namespace Barber.Cli.Commands.OpenApi
{
    using System.IO;
    using Barber.OpenApi.Settings;
    using McMaster.Extensions.CommandLineUtils;

    public class InitilaizeCommand
    {
        public static void Register(CommandLineApplication config)
        {
            config.HelpOption("-? | -h | --help");
            config.Description = "Create default configuration file";
            config.ExtendedHelpText = "Run this to create a new configuration file in current directory";

            var fileOption = config.Option("-f | --file",
                "Set configuration file name",
                CommandOptionType.SingleValue);

            config.OnExecute(() =>
            {
                var file = Constants.OPENAPI_SETTINGS_NAME;
                if (fileOption.HasValue())
                {
                    file = fileOption.Value()!.Trim();
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
                                Template = "model.mustache",
                            },
                            new StepModel()
                            {
                                Name = "Services",
                                Destination = "codegen-services",
                                Generator = "TypescriptService",
                                Template = "service.mustache",
                                Resolve = "Models",
                            },
                    },
                };

                File.WriteAllText(file, SettingsModel.ToJson(settings));

                return 0;
            });
        }
    }
}
