namespace Barber.Cli.Helper
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using McMaster.Extensions.CommandLineUtils;

    public static class CommandOptionHelper
    {
        /// <summary>
        /// Gets full path and checks if file exists
        /// </summary>
        /// <param name="option">The option.</param>
        /// <param name="defaultPath">The default path.</param>
        /// <returns></returns>
        /// <exception cref="System.IO.FileNotFoundException">If given file could not be found</exception>
        public static string FileParameter(CommandOption option, string defaultPath)
        {
            if (option == null || !option.HasValue())
            {
                return defaultPath;
            }

            var file = option.Value();
            if (string.IsNullOrEmpty(file) || string.IsNullOrWhiteSpace(file))
            {
                file = defaultPath;
            }

            if (string.IsNullOrWhiteSpace(file) || !File.Exists(file))
            {
                Styler.Error($"Could find file in: {file}");

                throw new FileNotFoundException(file);
            }

            return file!;
        }

        /// <summary>
        /// Gets full path and checks if directory exists
        /// </summary>
        /// <param name="option">The option.</param>
        /// <param name="defaultPath">The default path.</param>
        /// <returns></returns>
        /// <exception cref="System.IO.DirectoryNotFoundException">If given file could not be found</exception>
        public static string FolderParameter(CommandOption option, string defaultPath)
        {
            if (option == null || !option.HasValue())
            {
                return defaultPath;
            }

            var file = option.Value();
            if (string.IsNullOrEmpty(file) || string.IsNullOrWhiteSpace(file))
            {
                file = defaultPath;
            }

            if (string.IsNullOrWhiteSpace(file) || !Directory.Exists(file))
            {
                Styler.Error($"Could find directory in: {file}");

                throw new DirectoryNotFoundException(file);
            }

            return file!;
        }

        /// <summary>
        /// Tries to parse input to integer
        /// </summary>
        /// <param name="option">The option.</param>
        /// <param name="defaultNumber">The default number.</param>
        /// <returns></returns>
        public static int Number(CommandOption option, int defaultNumber)
        {
            if (option != null && option.HasValue())
            {
                var input = option.Value();
                if (!string.IsNullOrEmpty(input) && !string.IsNullOrWhiteSpace(input))
                {
                    if (int.TryParse(input, out var num))
                    {
                        return num;
                    }
                }
            }

            return defaultNumber;
        }

        /// <summary>
        /// Tries to parse input to integer
        /// </summary>
        /// <param name="option">The option.</param>
        /// <param name="defaultNumber">The default number.</param>
        /// <returns></returns>
        public static long NumberLong(CommandOption option, long defaultNumber)
        {
            if (option != null && option.HasValue())
            {
                var input = option.Value();
                if (!string.IsNullOrEmpty(input) && !string.IsNullOrWhiteSpace(input))
                {
                    if (int.TryParse(input, out var num))
                    {
                        return num;
                    }
                }
            }

            return defaultNumber;
        }

        /// <summary>
        /// Tries to parse input to integer
        /// </summary>
        /// <param name="option">The option.</param>
        /// <returns></returns>
        public static IReadOnlyList<int>? Numbers(CommandOption option)
        {
            var id = Number(option, 0);
            if (id != 0)
            {
                return new int[] { id };
            }

            if (option != null && option.HasValue())
            {
                // Check if is array
                var idsValue = option.Value();
                if (!string.IsNullOrEmpty(idsValue) && !string.IsNullOrWhiteSpace(idsValue))
                {
                    var array = idsValue?.Split(',');
                    if (array?.Length > 0)
                    {
                        return GetIds(array);
                    }

                    // Check if file
                    var file = CommandOptionHelper.FileParameter(option, string.Empty);
                    if (!string.IsNullOrEmpty(file))
                    {
                        array = File.ReadAllLines(file);
                        return GetIds(array);
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Tries to parse input to integer
        /// </summary>
        /// <param name="option">The option.</param>
        /// <returns></returns>
        public static IReadOnlyList<long>? NumbersLong(CommandOption option)
        {
            var id = NumberLong(option, 0);
            if (id != 0)
            {
                return new long[] { id };
            }

            if (option != null && option.HasValue())
            {
                // Check if is array
                var idsValue = option.Value();
                if (!string.IsNullOrEmpty(idsValue) && !string.IsNullOrWhiteSpace(idsValue))
                {
                    var array = idsValue?.Split(',');
                    if (array?.Length > 0)
                    {
                        return GetIdsLong(array);
                    }

                    // Check if file
                    var file = CommandOptionHelper.FileParameter(option, string.Empty);
                    if (!string.IsNullOrEmpty(file))
                    {
                        array = File.ReadAllLines(file);
                        return GetIdsLong(array);
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Reads input string and returns a trimmed string
        /// </summary>
        /// <param name="option">The option.</param>
        /// <param name="defaultString">The default string.</param>
        /// <returns></returns>
        public static string Text(CommandOption option, string defaultString)
        {
            if (option != null && option.HasValue())
            {
                var input = option.Value();
                if (!string.IsNullOrEmpty(input) && !string.IsNullOrWhiteSpace(input))
                {
                    return input!.Trim();
                }
            }

            return defaultString;
        }

        /// <summary>
        /// Reads input string and returns a trimmed string
        /// </summary>
        /// <param name="option">The option.</param>
        /// <param name="description">Description to display if option is not set.</param>
        /// <returns></returns>
        public static string TextAskIfEmpty(CommandOption option, string description)
        {
            if (option != null && option.HasValue())
            {
                var input = option.Value();
                if (!string.IsNullOrEmpty(input) && !string.IsNullOrWhiteSpace(input))
                {
                    return input!.Trim();
                }
            }
            else
            {
                Console.WriteLine($"Please enter {description}:");
                var input = Console.ReadLine();

                return input?.Trim() ?? string.Empty;
            }

            return string.Empty;
        }

        private static IReadOnlyList<int>? GetIds(IEnumerable<string> array)
        {
            var localArray = array?.ToArray();
            if (localArray?.Length > 0)
            {
                var list = new List<int>();
                foreach (var item in localArray)
                {
                    if (string.IsNullOrEmpty(item) || string.IsNullOrWhiteSpace(item))
                    {
                        continue;
                    }

                    if (int.TryParse(item.Trim(), out var match))
                    {
                        list.Add(match);
                    }
                }

                return list;
            }

            return null;
        }

        private static IReadOnlyList<long>? GetIdsLong(IEnumerable<string> array)
        {
            var localArray = array?.ToArray();
            if (localArray?.Length > 0)
            {
                var list = new List<long>();
                foreach (var item in localArray)
                {
                    if (string.IsNullOrEmpty(item) || string.IsNullOrWhiteSpace(item))
                    {
                        continue;
                    }

                    if (int.TryParse(item.Trim(), out var match))
                    {
                        list.Add(match);
                    }
                }

                return list;
            }

            return null;
        }
    }
}
