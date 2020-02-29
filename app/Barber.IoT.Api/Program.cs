namespace Barber.IoT.Api
{
    using System;
    using System.Reflection;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Hosting;

    public class Program
    {
        public static int Main(string[] args)
        {
            try
            {
                PrintLogo();
                Host.CreateDefaultBuilder(args)
                    .ConfigureWebHostDefaults(webBuilder =>
                    {
                        webBuilder.UseStartup<Startup>();
                    })
                    .Build()
                    .Run();

                return 0;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);

                return -1;
            }
        }

        private static void PrintLogo()
        {
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Red;
            const string LOGO_TEXT =
@"

███╗   ███╗ ██████╗ ████████╗████████╗███╗   ██╗███████╗████████╗    ███████╗███████╗██████╗ ██╗   ██╗███████╗██████╗
████╗ ████║██╔═══██╗╚══██╔══╝╚══██╔══╝████╗  ██║██╔════╝╚══██╔══╝    ██╔════╝██╔════╝██╔══██╗██║   ██║██╔════╝██╔══██╗
██╔████╔██║██║   ██║   ██║      ██║   ██╔██╗ ██║█████╗     ██║       ███████╗█████╗  ██████╔╝██║   ██║█████╗  ██████╔╝
██║╚██╔╝██║██║▄▄ ██║   ██║      ██║   ██║╚██╗██║██╔══╝     ██║       ╚════██║██╔══╝  ██╔══██╗╚██╗ ██╔╝██╔══╝  ██╔══██╗
██║ ╚═╝ ██║╚██████╔╝   ██║      ██║   ██║ ╚████║███████╗   ██║       ███████║███████╗██║  ██║ ╚████╔╝ ███████╗██║  ██║
╚═╝     ╚═╝ ╚══▀▀═╝    ╚═╝      ╚═╝   ╚═╝  ╚═══╝╚══════╝   ╚═╝       ╚══════╝╚══════╝╚═╝  ╚═╝  ╚═══╝  ╚══════╝╚═╝  ╚═╝

";

            Console.WriteLine(LOGO_TEXT);
            Console.ResetColor();
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("The official MQTT server implementation of MQTTnet");
            Console.WriteLine("Copyright (c) 2017-2019 The MQTTnet Team");
            Console.WriteLine(@"https://github.com/chkr1011/MQTTnet");

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($@"
Version:    {Assembly.GetExecutingAssembly()?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion}
License:    MIT (read LICENSE file)
Sponsoring: https://opencollective.com/mqttnet
Support:    https://github.com/chkr1011/MQTTnet/issues
Docs:       https://github.com/chkr1011/MQTTnet/wiki/MQTTnetServer
");

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(" ! THIS IS AN ALPHA VERSION! IT IS NOT RECOMMENDED TO USE IT FOR ANY DIFFERENT PURPOSE THAN TESTING OR EVALUATING!");
            Console.ResetColor();
            Console.WriteLine();
        }
    }
}
