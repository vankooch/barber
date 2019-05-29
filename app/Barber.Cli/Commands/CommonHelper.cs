namespace Barber.Cli.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Linq;
    using ConsoleTables;

    public static class CommonHelper
    {
        public static object? Parse(IEnumerable<string> args)
        {
            if (args == null || args?.Count() == 0)
            {
                return null;
            }

            dynamic model = new ExpandoObject();
            var dictionary = (IDictionary<string, object>)model;
            foreach (var item in args)
            {
                var parts = item
                    .TrimStart('-')
                    .TrimStart('-')
                    .Split('=');

                if (parts.Length == 2)
                {
                    dictionary.Add(parts[0].Trim(), parts[1].Trim());
                }
            }

            return model;
        }

        public static void WriteTableToConsole(this ConsoleTable table)
        {
            table.Options.EnableCount = true;
            table.Write(Format.MarkDown);
            Console.WriteLine();
        }
    }
}
