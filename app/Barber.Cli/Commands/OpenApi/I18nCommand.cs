namespace Barber.Cli.Commands.OpenApi
{
    using Barber.Cli.Helper;
    using McMaster.Extensions.CommandLineUtils;

    public class I18nCommand
    {
        public static void Schema(CommandLineApplication config)
        {
            config.HelpOption("-? | -h | --help");
            config.Description = "Paths";
            config.ExtendedHelpText = "List paths";

            var (fileOption, urlOption) = Helper.GetCommonOptions(config);

            config.OnExecuteAsync(async cancellationToken =>
            {
                var settings = Helper.ReadConfiguration(fileOption, urlOption);
                var processor = Helper.GetProcessor(settings);

                Styler.AlignCenter("I18next Resource Generator");
                Styler.DivisionLine('-');

                System.Console.WriteLine($"Languages: {settings.I18n.Count}");

                // Generate translations
                var sw = Styler.TaskStart($"Generating resources");
                await processor.I18n();
                Styler.TaskEnd(sw, true);

                return 0;
            });
        }
    }
}
