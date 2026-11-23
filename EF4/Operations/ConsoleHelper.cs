using System.Globalization;

namespace EF4.Operations
{
    public static class ConsoleHelper
    {
        public static string ReadString(string label, Func<string, string?>? validate = null)
        {
            while (true)
            {
                Console.Write(label);
                string input = (Console.ReadLine() ?? string.Empty).Trim();

                if (string.IsNullOrWhiteSpace(input))
                {
                    Error("  Value is required. Please try again.");
                    continue;
                }

                string? error = validate?.Invoke(input);
                if (error is null)
                    return input;

                Error($"  {error} Please try again.");
            }
        }

        public static int ReadInt(string label, int min = int.MinValue, int max = int.MaxValue)
        {
            while (true)
            {
                Console.Write(label);
                string input = (Console.ReadLine() ?? string.Empty).Trim();

                if (!int.TryParse(input, out int value))
                {
                    Error("  Please enter a valid whole number.");
                    continue;
                }

                if (value < min || value > max)
                {
                    Error($"  Please enter a number between {min} and {max}.");
                    continue;
                }

                return value;
            }
        }

        public static DateTime ReadDate(string label, Func<DateTime, string?>? validate = null)
        {
            while (true)
            {
                Console.Write(label);
                string input = (Console.ReadLine() ?? string.Empty).Trim();

                if (!DateTime.TryParseExact(input, "yyyy-MM-dd", CultureInfo.InvariantCulture,
                                            DateTimeStyles.None, out DateTime value))
                {
                    Error("  Invalid date. Use the format yyyy-MM-dd (e.g. 2000-01-25).");
                    continue;
                }

                string? error = validate?.Invoke(value);
                if (error is null)
                    return value;

                Error($"  {error} Please try again.");
            }
        }

        public static TEnum ReadEnumChoice<TEnum>(string title) where TEnum : struct, Enum
        {
            TEnum[] values = Enum.GetValues<TEnum>();

            Console.WriteLine(title);
            for (int i = 0; i < values.Length; i++)
                Console.WriteLine($"    {i + 1}) {values[i]}");

            int choice = ReadInt("  Choice: ", 1, values.Length);
            return values[choice - 1];
        }

        public static void Success(string message) => WriteColored(message, ConsoleColor.Green);
        public static void Error(string message)   => WriteColored(message, ConsoleColor.Red);
        public static void Info(string message)    => WriteColored(message, ConsoleColor.Cyan);

        public static void Pause()
        {
            Console.WriteLine();
            Console.Write("Press any key to return to the menu...");
            try
            {
                Console.ReadKey(intercept: true);
            }
            catch (InvalidOperationException)
            {
                // Input is redirected (no real console) - fall back to ReadLine
                Console.ReadLine();
            }
        }

        public static void ClearScreen()
        {
            try { Console.Clear(); }
            catch (IOException) { /* no real console attached */ }
        }

        private static void WriteColored(string message, ConsoleColor color)
        {
            ConsoleColor previous = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ForegroundColor = previous;
        }
    }
}
