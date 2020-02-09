namespace Barber.Cli.Commands.OpenApi
{
    using System.Diagnostics;
    using Barber.Cli.Helper;
    using Barber.OpenApi;
    using McMaster.Extensions.CommandLineUtils;

    public class GenerateCommand
    {
        public static void Register(CommandLineApplication config)
        {
            config.HelpOption("-? | -h | --help");
            config.Description = "Generate code based on OpenAPI specification";
            config.ExtendedHelpText = "Reads OpenAPI file and generates code";

            var (fileOption, urlOption) = Helper.GetCommonOptions(config);

            var stepOption = config.Option("-s | --step",
                "Run given step only",
                CommandOptionType.SingleValue);

            var dryOption = config.Option("-d | --dry",
                "Dry run without writing to disk",
                CommandOptionType.NoValue);

            config.OnExecuteAsync(async cancellationToken =>
            {
                // Setup
                var sw = new Stopwatch();
                var settings = Helper.ReadConfiguration(fileOption, urlOption);
                var processor = Helper.GetProcessor(settings);

                // Read OpenAPI file
                await Helper.ReadOpenApi(processor, settings);

                if (settings.Steps == null || settings.Steps.Count == 0)
                {
                    return 0;
                }

                // Process Steps
                foreach (var step in settings.Steps)
                {
                    sw = Styler.TaskStart($"Convert Step: {step.Name}");
                    processor.Convert(step);
                    Styler.TaskEnd(sw, true);
                }

                // Update References
                sw = Styler.TaskStart($"Update references");
                await processor.Update();
                Styler.TaskEnd(sw, true);

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

                        sw = Styler.TaskStart($"Write Step: {step.Name}");
                        Processor.Write(step.ResultItems);
                        Styler.TaskEnd(sw, true);
                    }
                }

                return 0;
            });
        }
    }
}
