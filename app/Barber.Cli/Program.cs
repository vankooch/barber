namespace Barber.Cli
{
    using System;
    using McMaster.Extensions.CommandLineUtils;

    public class Program
    {
        public static void Main(string[] args)
        {
            var app = new CommandLineApplication(throwOnUnexpectedArg: false)
            {
                AllowArgumentSeparator = true,
            };

            app.Command("render", Commands.RenderCommand.Register);
            app.Command("mustache", Commands.RenderCommand.Register);

            app.Command("coap", (cmd) =>
            {
                // Subcommands
                cmd.Command("psk", Commands.CoAP.PskCreateCommand.Register);
                cmd.Command("discover", Commands.CoAP.DiscoverCommand.Register);
            });

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
