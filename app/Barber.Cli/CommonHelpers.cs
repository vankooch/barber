namespace Barber.Cli
{
    using System;
    using McMaster.Extensions.CommandLineUtils;

    public static class CommonHelpers
    {
        public static string GetString(CommandOption option, string defaultString = "")
        {
            if (!option.HasValue())
            {
                return defaultString;
            }

            return option.Value();
        }

        public static string GetStringRead(CommandOption option, string name)
        {
            if (!option.HasValue())
            {
                Console.WriteLine($"Please enter {name}:");
                var input = Console.ReadLine();

                return input?.Trim();
            }

            return option.Value();
        }
    }
}
