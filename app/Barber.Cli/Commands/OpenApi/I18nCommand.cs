namespace Barber.Cli.Commands.OpenApi
{
    using McMaster.Extensions.CommandLineUtils;

    public class I18nCommand
    {
        public static void Schema(CommandLineApplication config)
        {
            config.HelpOption("-? | -h | --help");
            config.Description = "Paths";
            config.ExtendedHelpText = "List paths";

            var (fileOption, urlOption) = Helper.GetCommonOptions(config);

            config.OnExecute(async () =>
            {
                var settings = Helper.ReadConfiguration(fileOption, urlOption);
                var processor = Helper.GetProcessor(settings);

                LogHelper.AlignCenter("I18next Resource Generator");
                LogHelper.DivisionLine('-');

                System.Console.WriteLine($"Languages: {settings.I18n.Length}");

                // Generate translations
                var sw = LogHelper.TaskStart($"Generating resources");
                await processor.I18n();
                LogHelper.TaskStop(sw, true);

                return 0;
            });
        }
    }
}
