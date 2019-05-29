namespace Barber.Cli.Commands.OpenApi
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Threading.Tasks;
    using Barber.OpenApi;
    using Barber.OpenApi.Generator;
    using Barber.OpenApi.Settings;
    using McMaster.Extensions.CommandLineUtils;

    public static class Helper
    {
        public static (CommandOption fileOption, CommandOption urlOption) GetCommonOptions(CommandLineApplication config)
        {
            var fileOption = config.Option("-f | --file",
                 "Set configuration file name",
                 CommandOptionType.SingleValue);

            var urlOption = config.Option("-a | --api",
                "URL / Path to OpenAPI File",
                CommandOptionType.SingleValue);

            return (fileOption, urlOption);
        }

        public static Processor GetProcessor(SettingsModel settings)
        {
            var processor = new Processor(new Typescript(settings), settings);
            processor.Generators.Add(new TypescriptModel(settings));
            processor.Generators.Add(new TypescriptService());

            return processor;
        }

        public static SettingsModel ReadConfiguration(CommandOption fileOption, CommandOption urlOption)
        {
            var file = Constants.OPENAPI_SETTINGS_NAME;
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

            if (urlOption.HasValue())
            {
                settings.Url = urlOption.Value().Trim();
            }

            return settings;
        }

        public static async Task ReadOpenApi(Processor processor, SettingsModel settings, bool showLog = true)
        {
            var sw = new Stopwatch();
            if (showLog)
            {
                sw = LogHelper.TaskStart($"Reading from {settings.Url}");
            }

            await processor.Read(settings.Url);

            if (showLog)
            {
                LogHelper.TaskStop(sw, true);
            }
        }
    }
}
