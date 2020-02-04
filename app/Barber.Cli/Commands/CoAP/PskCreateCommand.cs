namespace Barber.Cli.Commands.CoAP
{
    using System;
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
                var path = CommonHelpers.GetString(pathOption, "C:\\Projects");
                var gatewayName = CommonHelpers.GetStringRead(gatewayNameOption, "Gateway Name");
                var gatewayAddress = CommonHelpers.GetStringRead(gatewayNameOption, "Gateway Address");
                var gatewaySecret = CommonHelpers.GetStringRead(gatewayNameOption, "Gateway Secret");

                var sw = LogHelper.TaskStart("Creating PSK");
                var psk = Helper.GeneratePsk(new TradfriController(gatewayName, gatewayAddress), gatewaySecret, "barber");
                LogHelper.TaskStop(sw, true);

                sw = LogHelper.TaskStart("Write PSK");
                var fileName = Helper.PskWrite(path, gatewayName, psk, "barber");
                LogHelper.TaskStop(sw, true);

                Console.WriteLine($"PSK: {psk}");
                Console.WriteLine($"File: {fileName}");

                return 0;
            });
        }
    }
}
