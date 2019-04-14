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
                AllowArgumentSeparator = true
            };

            app.Command("mustache", Commands.Mustache.Register);
            app.Command("render", Commands.Mustache.Register);
            app.Command("openapi", Commands.OpenApi.Register);

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
