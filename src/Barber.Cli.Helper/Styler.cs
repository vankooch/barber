namespace Barber.Cli.Helper
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;

    public static class Styler
    {
        public const string FORMAT_JOB = "{0, -55}...";
        public const string FORMAT_KEY_VAL = " - {0, -55}{1}";

        /// <summary>
        /// Aligns input string
        /// </summary>
        /// <param name="text">The text.</param>
        public static void AlignCenter(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return;
            }

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

        /// <summary>
        /// Creates a simple bar chart
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="typeName">Name of the type.</param>
        public static void BarChart(IList<KeyValuePair<string, long>> list, string typeName = "[ms]")
        {
            if (list == null || !list.Any())
            {
                return;
            }

            const int MAX_WIDTH = 100;
            var lineTemplate = "| - {0, -20} | {1, -105} | {2} " + typeName;
            var order = list.OrderByDescending(e => e.Value).ToArray();
            var max = order[0].Value;
            for (var i = 0; i < order.Length; i++)
            {
                var pad = Math.Round(MAX_WIDTH / (decimal)max * order[i].Value);
                Console.WriteLine(lineTemplate, order[i].Key, string.Empty.PadRight((int)pad, '#'), order[i].Value);
            }
        }

        /// <summary>
        /// Division line
        /// </summary>
        /// <param name="character">The character.</param>
        /// <param name="color">if set to <c>true</c> [color].</param>
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

        /// <summary>
        /// Prints exception
        /// </summary>
        /// <param name="exception">The exception.</param>
        public static void Error(Exception exception)
        {
            Console.WriteLine();
            DivisionLine('-');
            Error("Operation Failed");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(exception);
            Console.ResetColor();
        }

        /// <summary>
        /// Errors the specified MSG.
        /// </summary>
        /// <param name="msg">The MSG.</param>
        public static void Error(string msg)
        {
            Console.BackgroundColor = ConsoleColor.DarkRed;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(msg);
            Console.ResetColor();
            Console.Write(Environment.NewLine);
        }

        /// <summary>
        /// Informations the specified MSG.
        /// </summary>
        /// <param name="msg">The MSG.</param>
        public static void Info(string msg)
        {
            Console.BackgroundColor = ConsoleColor.DarkCyan;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(msg);
            Console.ResetColor();
            Console.Write(Environment.NewLine);
        }

        /// <summary>
        /// Informations the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public static void Info(string key, string value)
        {
            Console.BackgroundColor = ConsoleColor.DarkCyan;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(FORMAT_KEY_VAL, key, value);
            Console.ResetColor();
            Console.Write(Environment.NewLine);
        }

        /// <summary>
        /// Reads user console input as bool.
        /// </summary>
        /// <param name="msg">The MSG.</param>
        /// <returns></returns>
        public static bool ReadInputAsBool(string msg)
        {
            Console.WriteLine($@"{msg} (Yes / No)");
            var result = Console.ReadLine();
            if (string.IsNullOrEmpty(result) || string.IsNullOrWhiteSpace(result))
            {
                return false;
            }

            result = result.Trim().ToUpperInvariant();

            return result == "YES" || result == "Y" || result == "JA" || result == "J";
        }

        /// <summary>
        /// Successes the specified MSG.
        /// </summary>
        /// <param name="msg">The MSG.</param>
        /// <param name="newLine">if set to <c>true</c> [new line].</param>
        public static void Success(string msg, bool newLine = true)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(msg);
            Console.ResetColor();

            if (newLine)
            {
                Console.Write(Environment.NewLine);
            }
        }

        /// <summary>
        /// Prints task end message
        /// </summary>
        /// <param name="ts">The ts.</param>
        /// <param name="success">if set to <c>true</c> [success].</param>
        public static void TaskEnd(Stopwatch ts, bool success = true)
        {
            if (ts == null)
            {
                return;
            }

            ts.Stop();
            var time = ts.Elapsed.TotalSeconds.ToString("0.000", CultureInfo.InvariantCulture) + " [sec]";
            if (success)
            {
                Success($"done in {time}");
            }
            else
            {
                Error($"fail in {time}");
            }
        }

        /// <summary>
        /// Prints task end message
        /// </summary>
        /// <param name="ts">The ts.</param>
        /// <param name="success">if set to <c>true</c> [success].</param>
        public static void TaskEnd(TimeSpan ts, bool success = true)
        {
            if (ts == null)
            {
                return;
            }

            var time = ts.TotalSeconds.ToString("0.000", CultureInfo.InvariantCulture) + " [sec]";
            if (success)
            {
                Success($"done in {time}");
            }
            else
            {
                Error($"fail in {time}");
            }
        }

        /// <summary>
        /// Prints task start message
        /// </summary>
        /// <param name="msg">The MSG.</param>
        /// <returns></returns>
        public static Stopwatch TaskStart(string msg)
        {
            Console.Write(FORMAT_JOB, msg + @"...");
            var ts = new Stopwatch();
            ts.Start();

            return ts;
        }

        /// <summary>
        /// Verboses the specified MSG.
        /// </summary>
        /// <param name="msg">The MSG.</param>
        public static void Verbose(string msg)
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write(msg);
            Console.ResetColor();
            Console.Write(Environment.NewLine);
        }

        /// <summary>
        /// Warnings the specified MSG.
        /// </summary>
        /// <param name="msg">The MSG.</param>
        public static void Warning(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(msg);
            Console.ResetColor();
            Console.Write(Environment.NewLine);
        }
    }
}
