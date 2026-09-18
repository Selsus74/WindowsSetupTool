using System.IO;

namespace WindowsSetupTool.Helpers;

public static class Logger
{
    private static readonly string LogDirectory =
        Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
            "WindowsSetupTool"
        );

    private static readonly string LogFile = Path.Combine(LogDirectory, "setup.log");

    public static void Log(string message)
    {
        try
        {
            Directory.CreateDirectory(LogDirectory);

            string line = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}";

            File.AppendAllText(LogFile, line + Environment.NewLine);
        }
        catch
        {
            //keep app running if logging fails
        }
    }
}