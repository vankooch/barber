namespace Barber.IoT.Cli
{
    using System;
    using Barber.Cli.Helper;
    using Barber.IoT.Context;
    using McMaster.Extensions.CommandLineUtils;
    using Microsoft.Extensions.Configuration;
    using SimpleInjector;
    using SimpleInjector.Lifestyles;

    public class Program
    {
        internal static IConfigurationRoot? Configuration { get; private set; }

        internal static Container Container { get; private set; } = new Container();

        public static int Main(string[] args)
        {
            Console.ResetColor();

            // Settings
            var configBuilder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables();
            Configuration = configBuilder.Build();

            Container = BootstrapContainer(Container, Configuration);

            var app = new CommandLineApplication(throwOnUnexpectedArg: false)
            {
                AllowArgumentSeparator = true,
            };

            app.Command("coap", (cmd) =>
            {
                cmd.HelpOption("-? | -h | --help");
                cmd.Description = "Some simple tests with CaAP";
                cmd.ExtendedHelpText = "Some simple PoC for working with CoAP and Ikea Tradfri gateway";

                cmd.Command("psk", Commands.CoAP.PskCreateCommand.Register);
                cmd.Command("discover", Commands.CoAP.DiscoverCommand.Register);
            });

            app.Command("database", (cmd) =>
            {
                cmd.HelpOption("-? | -h | --help");
                cmd.Description = "General database commands";
                cmd.ExtendedHelpText = "Create, Update EF Core database context";

                cmd.Command("bootstrap", Commands.Database.UpdateCommand.Register);
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

        private static Container BootstrapContainer(Container container, IConfigurationRoot config)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            // Options
            container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();

            // Configure
            container.RegisterInstance(config);

            // Ef
            var contextOptions = BarberIoTContextOptionCreator.GetOptionsBuilder(Configuration, "barber-main");

            container.RegisterInstance(contextOptions);

            // Main
            container.Register<IBarberIoTContextCreator, BarberIoTContextDiCreator>(Lifestyle.Transient);
            container.Register<IBarberIoTContext>(() => new BarberIoTContext(contextOptions.Options), Lifestyle.Transient);

            return container;
        }
    }
}
