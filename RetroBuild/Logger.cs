using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace RetroBuild
{
    internal class Logger
    {
        private static readonly string logFilePath;

        static Logger()
        {
            string exePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string exeDir = Path.GetDirectoryName(exePath);
            logFilePath = Path.Combine(exeDir, "build.log");

            // Optional: delete existing log on startup
            if (File.Exists(logFilePath))
            {
                File.Delete(logFilePath);
            }
        }

        public static void Log(string message)
        {
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            string formatted = $"{timestamp} {message}";

            Console.WriteLine(formatted);
            File.AppendAllText(logFilePath, formatted + Environment.NewLine);
        }

        public static void LogLabel(string label)
        {
            Log($"[LABEL] :{label}");
        }

        public static void LogInfo(string message)
        {
            Log($"[INFO] {message}");
        }

        public static void LogExit(int code)
        {
            Log($"[EXIT] {code}");
        }

        public static void LogStart(string scriptName)
        {
            Log($"[START] Run: {scriptName}");
        }
    }
}
