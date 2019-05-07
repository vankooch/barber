namespace Barber.Cli
{
    using System;
    using System.Diagnostics;

    public static class LogHelper
    {
        private const int LINE_WIDTH = 80;

        public static void AlignCenter(string text)
        {
            if (text.Length > Console.WindowWidth)
            {
                Console.WriteLine(text);
                return;
            }

            decimal size = Console.WindowWidth - 1 - text.Length;
            var rightSize = (int)Math.Round(size / 2);
            var leftSize = (int)(size - rightSize);
            var leftMargin = new string(' ', leftSize);
            var rightMargin = new string(' ', rightSize);

            Console.Write(leftMargin);
            Console.Write(text);
            Console.WriteLine(rightMargin);
        }

        public static void DivisionLine(char character, bool color = true)
        {
            var text = new string(character, Console.WindowWidth - 1);
            if (color)
            {
                Console.BackgroundColor = ConsoleColor.DarkCyan;
                Console.ForegroundColor = ConsoleColor.White;
            }

            Console.Write(text);
            Console.ResetColor();
            Console.Write(Environment.NewLine);
        }

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
            var format = $" - {sw.Elapsed.TotalSeconds.ToString("0.00")}[sec] {count}";
            Console.Write(format);
            Console.Write(Environment.NewLine);
        }
    }
}
