namespace Barber.Cli.Commands.OpenApi
{
    using System.Diagnostics;
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

            config.OnExecute(async () =>
            {
                // Setup
                var sw = new Stopwatch();
                var settings = Helper.ReadConfiguration(fileOption, urlOption);
                var processor = Helper.GetProcessor(settings);

                // Read OpenAPI file
                await Helper.ReadOpenApi(processor, settings);

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
