namespace Barber.Cli.Commands
{
    using System;
    using System.IO;
    using Barber.Cli.Helper;
    using Barber.Core;
    using McMaster.Extensions.CommandLineUtils;
    using Newtonsoft.Json;

    public static class RenderCommand
    {
        public static void Register(CommandLineApplication config)
        {
            config.HelpOption("-? | -h | --help");
            config.AllowArgumentSeparator = true;
            config.Description = "Render template";
            config.ExtendedHelpText = "Reads mustache template and process it.";

            var templateOption = config.Option("-i | --in",
                "Path to mustache template",
                CommandOptionType.SingleValue);

            var destinationOption = config.Option("-o | --out",
                "Path to output file (optional)",
                CommandOptionType.SingleValue);

            var jsonOption = config.Option("-j | --json",
                "Path to JSON file",
                CommandOptionType.SingleValue);

            var verboseOption = config.Option("-v | --verbose",
                "Display verbose output",
                CommandOptionType.NoValue);

            config.OnExecuteAsync(async cancellationToken =>
            {
                if (!templateOption.HasValue())
                {
                    Styler.Error("Please set a template file");

                    return 1;
                }

                var template = templateOption.Value().Trim();
                if (!File.Exists(template))
                {
                    template = Path.Combine(Directory.GetCurrentDirectory(), template);

                    if (!File.Exists(template))
                    {
                        Styler.Error($"Could not find template file in: {template}");

                        return 1;
                    }
                }

                object model = null;
                if (jsonOption.HasValue())
                {
                    var file = jsonOption.Value().Trim();
                    if (!File.Exists(file))
                    {
                        Styler.Error($"Could not find JSON file in: {template}");

                        return 1;
                    }

                    model = JsonConvert.DeserializeObject(file);
                }
                else
                {
                    model = ArgumentHelper.Parse(config.RemainingArguments);
                }

                if (verboseOption.HasValue())
                {
                    Console.WriteLine("Data Model:");
                    Console.WriteLine(model);
                }

                var sw = Styler.TaskStart("Processing templates");
                var renderer = new Core.Renderer.Mustache();
                var result = await renderer.Render(template, model);
                Styler.TaskEnd(sw, true);

                var destination = string.Empty;
                if (!destinationOption.HasValue())
                {
                    destination = template;
                }
                else
                {
                    destination = destinationOption.Value().Trim();
                }

                var destinationPath = Path.GetDirectoryName(destination);
                if (!string.IsNullOrEmpty(destinationPath) && !Directory.Exists(destinationPath))
                {
                    Directory.CreateDirectory(destinationPath);
                }

                sw = Styler.TaskStart("Writing file");
                await File.WriteAllTextAsync(destination, result, cancellationToken);
                Styler.TaskEnd(sw, true);

                return 0;
            });
        }
    }
}
