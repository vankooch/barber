namespace Barber.Cli.Commands.CoAP
{
    using System;
    using McMaster.Extensions.CommandLineUtils;
    using Newtonsoft.Json;
    using Tomidix.NetStandard.Tradfri;

    public class DiscoverCommand
    {
        public static void Register(CommandLineApplication config)
        {
            config.HelpOption("-? | -h | --help");
            config.Description = "Discover devices listed on Tradfri Gateway";
            config.ExtendedHelpText = "CoAP Discovery";

            var pathOption = config.Option("-p | --path",
                "Path to write PSK file",
                CommandOptionType.SingleValue);

            var gatewayNameOption = config.Option("-n | --name",
                "Gateway Name",
                CommandOptionType.SingleValue);

            var gatewayAddressOption = config.Option("-a | --address",
                "Gateway Address",
                CommandOptionType.SingleValue);

            config.OnExecuteAsync(async cancellationToken =>
            {
                var path = CommonHelpers.GetString(pathOption, "C:\\Projects");
                var gatewayName = CommonHelpers.GetStringRead(gatewayNameOption, "Gateway Name");
                var gatewayAddress = CommonHelpers.GetStringRead(gatewayNameOption, "Gateway Address");

                var sw = LogHelper.TaskStart("Read PSK");
                var psk = Helper.PskRead(path, gatewayName, "barber");
                LogHelper.TaskStop(sw, true);

                sw = LogHelper.TaskStart("Connect to gateway");
                var controller = new TradfriController(gatewayName, gatewayAddress);
                controller.ConnectAppKey(psk, "barber");
                var devices = await controller.GatewayController.GetDeviceObjects();
                LogHelper.TaskStop(sw, true);

                if (devices?.Count > 0)
                {
                    foreach (var item in devices)
                    {
                        Console.WriteLine($"{item.ID}-{item.Name}-{item.Info.Battery}-{item.DeviceType}");
                    }

                    Console.WriteLine(JsonConvert.SerializeObject(devices));
                }

                return 0;
            });
        }
    }
}
