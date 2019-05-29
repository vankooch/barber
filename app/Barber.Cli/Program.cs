namespace Barber.Cli
{
    using System;
    using McMaster.Extensions.CommandLineUtils;

    public class Program
    {
        public static void Main(string[] args)
        {
            var app = new CommandLineApplication()
            {
                AllowArgumentSeparator = true,
            };

            app.Command("render", Commands.RenderCommand.Register);
            app.Command("openapi", (cmd) =>
            {
                // Subcommands
                cmd.Command("init", Commands.OpenApiCommand.CreateProjectSettings);
                cmd.Command("list", Commands.OpenApiCommand.List);

                Commands.OpenApiCommand.Run(cmd);
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
