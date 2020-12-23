namespace Barber.Cli
{
    using System;
    using System.Diagnostics;

    public static class LogHelper
    {
        private const int LINE_WIDTH = 80;

        public static void Error(string message)
        {
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        public static Stopwatch TaskStart(string message)
        {
            var sw = new Stopwatch();
            sw.Start();

            Console.ResetColor();
            var format = message + "...".PadRight(LINE_WIDTH - message.Length);
            Console.Write(format);

            return sw;
        }

        public static void TaskStop(Stopwatch sw, bool success, int? message = null)
        {
            sw.Stop();

            Console.ResetColor();
            if (success)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("OK");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("FAIL");
            }

            Console.ResetColor();
            var count = message.HasValue ? message.Value.ToString() + "[#]" : string.Empty;
            var format = $" - {sw.Elapsed.TotalSeconds:0.00}[sec] {count}";
            Console.Write(format);
            Console.Write(Environment.NewLine);
        }
    }
}
