using System.Diagnostics;
using System.IO;
using System;

public static class Logger
{
    private static readonly string logFilePath;

    static Logger()
    {
        string exeFolder = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
        logFilePath = Path.Combine(exeFolder, "retrobat-install.log");
    }

    public static void Log(string message)
    {
        try
        {
            string logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}";
            File.AppendAllText(logFilePath, logEntry + Environment.NewLine);
        }
        catch { }
    }
}