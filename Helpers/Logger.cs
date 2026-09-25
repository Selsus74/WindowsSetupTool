using System.Diagnostics;
using System.IO;

namespace WindowsSetupTool.Helpers;

//---loglevel enum---
public enum LogLevel
{
    INFO,
    WARNING,
    ERROR
}

public static class Logger
{
    //---log directory---
    private static readonly string LogDirectory = Path.Combine(AppContext.BaseDirectory, "Logs");
    //---log file path---
    private static readonly string LogFile = Path.Combine(LogDirectory, $"{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.log");

    //---log level counter---
    private static int _infoCount = 0;
    private static int _warningCount = 0;
    private static int _errorCount = 0;

    //---public getter for the log counters---
    public static int InfoCount => _infoCount;
    public static int WarningCount => _warningCount;
    public static int ErrorCount => _errorCount;

    //---logging method---
    public static void Log(LogLevel level, string message)
    {
        try
        {
            Directory.CreateDirectory(LogDirectory);

            string line = "🔷" + $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{level}] - {message}";

            File.AppendAllText(LogFile, line + Environment.NewLine);

            switch (level)
            {
                case LogLevel.INFO:
                    _infoCount++;
                    break;
                case LogLevel.WARNING:
                    _warningCount++;
                    break;
                case LogLevel.ERROR:
                    _errorCount++;
                    break;
            }
        }
        catch
        {
            // keep app running if logging fails
        }
    }

    //---short log wrapper (this will be used elsewhere)---
    public static void Info(string message) => Log(LogLevel.INFO, message);
    public static void Warning(string message) => Log(LogLevel.WARNING, message);
    public static void Error(string message) => Log(LogLevel.ERROR, message);

    //---summary method for use in mainwindow msg box---
    public static string GetSummary()
    {
        return $"{_infoCount} Info, {_warningCount} Warning(s), {_errorCount} Error(s)";
    }

    //---reset counters if app is run again without closing---
    public static void ResetCounters()
    {
        _infoCount = 0;
        _warningCount = 0;
        _errorCount = 0;
    }

    public static void OpenLatestLog()
    {
        Process.Start(new ProcessStartInfo
        {
            FileName = LogFile,
            UseShellExecute = true
        });
    }
}