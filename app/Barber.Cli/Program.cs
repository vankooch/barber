namespace Barber.Cli
{
    using System;
    using System.IO;
    using Barber.Core;
    using Barber.Core.Renderer;
    using McMaster.Extensions.CommandLineUtils;
    using Newtonsoft.Json;

    public class Program
    {
        public static void Main(string[] args)
        {
            var app = new CommandLineApplication(throwOnUnexpectedArg: false)
            {
                AllowArgumentSeparator = true
            };

            app.Command("mustache", config =>
            {
                config.HelpOption("-? | -h | --help");
                config.AllowArgumentSeparator = true;
                config.Description = "Run mustache renderer";
                config.ExtendedHelpText = "Reads mustache template and proccess it.";

                var templateOption = config.Option("-i | --in",
                    "Path to mustache template",
                    CommandOptionType.SingleValue);

                var destinationOption = config.Option("-o | --out",
                    "Path to output file (optional)",
                    CommandOptionType.SingleValue);

                var verboseOption = config.Option("-v | --verbose",
                    "Path to output file (optional)",
                    CommandOptionType.NoValue);

                config.OnExecute(async () =>
                {
                    if (!templateOption.HasValue())
                    {
                        LogHelper.Error("Please set a template file");

                        return 1;
                    }

                    var template = templateOption.Value().Trim();
                    if (!File.Exists(template))
                    {
                        template = Path.Combine(Directory.GetCurrentDirectory(), template);

                        if (!File.Exists(template))
                        {
                            LogHelper.Error($"Could not find tenplate file in: {template}");

                            return 1;
                        }
                    }

                    if (verboseOption.HasValue())
                    {
                        Console.WriteLine("Data Model:");
                        var model = JsonConvert.SerializeObject(ArgumentHelper.Parse(config.RemainingArguments), Formatting.Indented);
                        Console.WriteLine(model);
                    }

                    var sw = LogHelper.TaskStart("Running renderer");
                    var renderer = new Mustache();
                    var result = await renderer.Render(template, ArgumentHelper.Parse(config.RemainingArguments));
                    LogHelper.TaskStop(sw, true);

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

                    sw = LogHelper.TaskStart("Writing file");
                    await File.WriteAllTextAsync(destination, result);
                    LogHelper.TaskStop(sw, true);

                    return 0;
                });
            });

            try
            {
                app.HelpOption("-? | -h | --help");
                app.Execute(args);
            }
            catch (Exception ex)
            {
                Console.Write(Environment.NewLine);
                LogHelper.Error(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
                Console.Write(Environment.NewLine);
            }
        }
    }
}
