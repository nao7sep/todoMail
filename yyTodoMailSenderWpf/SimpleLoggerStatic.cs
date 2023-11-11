using System;
using System.IO;
using System.Text;
using yyLib;

namespace yyTodoMailSenderWpf
{
    public static class SimpleLogger
    {
        private static Lazy <string> _logFilePath = new (() => yyApplicationDirectory.MapPath ("Logs.txt"));

        public static string LogFilePath => _logFilePath.Value;

        private static string Borderline => new ('-', 80);

        public static void Log (string key, string value)
        {
            try
            {
                StringBuilder xBuilder = new ();

                if (File.Exists (LogFilePath))
                    xBuilder.AppendLine (Borderline);

                xBuilder.AppendLine ($"Time: {DateTime.UtcNow.ToString ("O")}"); // Roundtrip format.
                xBuilder.AppendLine ($"{key}: {value.TrimEnd ()}");

                File.AppendAllText (LogFilePath, xBuilder.ToString (), Encoding.UTF8);
            }

            catch
            {
                // Do nothing.
                // Often called in a catch block.
            }
        }

        public static void LogException (Exception exception) => Log ("Exception", exception.ToString ());
    }
}
