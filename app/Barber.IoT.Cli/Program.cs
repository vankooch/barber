namespace Barber.IoT.Cli
{
    using System;
    using Barber.Cli.Helper;
    using McMaster.Extensions.CommandLineUtils;
    using Microsoft.Extensions.Configuration;
    using SimpleInjector;

    public class Program
    {
        public static IConfigurationRoot Configuration { get; private set; }

        public static Container Container { get; private set; }

        public static int Main(string[] args)
        {
            Console.ResetColor();

            // Settings
            var configBuilder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables();
            Configuration = configBuilder.Build();

            var app = new CommandLineApplication(throwOnUnexpectedArg: false)
            {
                AllowArgumentSeparator = true,
            };

            app.Command("coap", (cmd) =>
            {
                cmd.HelpOption("-? | -h | --help");

                cmd.Command("psk", Commands.CoAP.PskCreateCommand.Register);
                cmd.Command("discover", Commands.CoAP.DiscoverCommand.Register);
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
                Styler.Error(ex);
                Console.Write(Environment.NewLine);

                return 1;
            }
        }
    }
}
