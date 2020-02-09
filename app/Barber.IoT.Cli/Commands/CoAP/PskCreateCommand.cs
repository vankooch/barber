namespace Barber.IoT.Cli.Commands.CoAP
{
    using System;
    using Barber.Cli.Helper;
    using McMaster.Extensions.CommandLineUtils;
    using Tomidix.NetStandard.Tradfri;

    public class PskCreateCommand
    {
        public static void Register(CommandLineApplication config)
        {
            config.HelpOption("-? | -h | --help");
            config.Description = "Generate new PSK key for talking with Tradfri Gateway";
            config.ExtendedHelpText = "Create a new PSK certificate for talking to Ikea Tradfri Gateway";

            var pathOption = config.Option("-p | --path",
                "Path to write PSK file",
                CommandOptionType.SingleValue);

            var gatewayNameOption = config.Option("-n | --name",
                "Gateway Name",
                CommandOptionType.SingleValue);

            var gatewayAddressOption = config.Option("-a | --address",
                "Gateway Address",
                CommandOptionType.SingleValue);

            var gatewaySecretOption = config.Option("-s | --secret",
                "Gateway Secret",
                CommandOptionType.SingleValue);

            config.OnExecute(() =>
            {
                var path = CommandOptionHelper.Text(pathOption, "C:\\Projects");
                var gatewayName = CommandOptionHelper.TextAskIfEmpty(gatewayNameOption, "Gateway Name");
                var gatewayAddress = CommandOptionHelper.TextAskIfEmpty(gatewayNameOption, "Gateway Address");
                var gatewaySecret = CommandOptionHelper.TextAskIfEmpty(gatewayNameOption, "Gateway Secret");

                var sw = Styler.TaskStart("Creating PSK");
                var psk = Helper.GeneratePsk(new TradfriController(gatewayName, gatewayAddress), gatewaySecret, "barber");
                Styler.TaskEnd(sw, true);

                sw = Styler.TaskStart("Write PSK");
                var fileName = Helper.PskWrite(path, gatewayName, psk, "barber");
                Styler.TaskEnd(sw, true);

                Console.WriteLine($"PSK: {psk}");
                Console.WriteLine($"File: {fileName}");

                return 0;
            });
        }
    }
}
