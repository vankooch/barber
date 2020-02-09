namespace Barber.IoT.Cli.Commands.CoAP
{
    using System;
    using System.IO;
    using Tomidix.NetStandard.Tradfri;

    public static class Helper
    {
        public static string GeneratePsk(
            TradfriController controller,
            string gatewaySecrect,
            string applicationName)
        {
            if (string.IsNullOrWhiteSpace(gatewaySecrect))
            {
                throw new ArgumentNullException("The secret is missing");
            }

            if (string.IsNullOrWhiteSpace(applicationName))
            {
                applicationName = "Barber.IoT.Cli";
            }

            var psk = controller.GenerateAppSecret(gatewaySecrect, applicationName);

            return psk.PSK;
        }

        public static string PskRead(string path, string gatewayName, string applicationName)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentNullException("Base path missing");
            }

            if (string.IsNullOrWhiteSpace(gatewayName))
            {
                throw new ArgumentNullException("Gateway name is missing");
            }

            if (string.IsNullOrWhiteSpace(applicationName))
            {
                applicationName = "Barber.Cli";
            }

            if (!Directory.Exists(path))
            {
                throw new DirectoryNotFoundException($"Could not find path: {path}");
            }

            var fileName = $"tradfri-{gatewayName}-{applicationName}.psk";
            var psk = File.ReadAllText(Path.Combine(path, fileName));
            return psk;
        }

        public static string PskWrite(string path, string gatewayName, string psk, string applicationName)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentNullException("Base path missing");
            }

            if (string.IsNullOrWhiteSpace(gatewayName))
            {
                throw new ArgumentNullException("Gateway name is missing");
            }

            if (string.IsNullOrWhiteSpace(applicationName))
            {
                applicationName = "Barber.IoT.Cli";
            }

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            var fileName = $"tradfri-{gatewayName}-{applicationName}.psk";
            var fullPath = Path.Combine(path, fileName);
            File.WriteAllText(fullPath, psk);

            return fullPath;
        }
    }
}
