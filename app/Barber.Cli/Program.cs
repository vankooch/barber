namespace Barber.Cli
{
    using System;
    using Barber.Cli.Helper;
    using McMaster.Extensions.CommandLineUtils;

    public class Program
    {
        public static int Main(string[] args)
        {
            Console.ResetColor();
            var app = new CommandLineApplication(throwOnUnexpectedArg: false)
            {
                AllowArgumentSeparator = true,
            };

            app.Command("render", Commands.RenderCommand.Register);
            app.Command("mustache", Commands.RenderCommand.Register);

            app.Command("openapi", (cmd) =>
            {
                // Subcommands
                cmd.Command("init", Commands.OpenApi.InitilaizeCommand.Register);
                cmd.Command("list", Commands.OpenApi.ListCommand.Register);
                cmd.Command("i18n", Commands.OpenApi.I18nCommand.Schema);

                // Main
                Commands.OpenApi.GenerateCommand.Register(cmd);
            });

            app.Command("o", (cmd) =>
            {
                // Subcommands
                cmd.Command("l", Commands.OpenApi.ListCommand.Register);
                cmd.Command("i", Commands.OpenApi.I18nCommand.Schema);

                // Main
                Commands.OpenApi.GenerateCommand.Register(cmd);
            });

            try
            {
                app.HelpOption("-? | -h | --help");
                app.Execute(args);

                return 0;
            }
            catch (Exception ex)
            {
                Console.Write(Environment.NewLine);
                Styler.Error(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
                Console.Write(Environment.NewLine);

                return 1;
            }
        }
    }
}
