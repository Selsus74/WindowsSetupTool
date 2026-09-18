using System.Management;

namespace WindowsSetupTool.Setup;

public static class SystemInformation
{
    public static string GetComputerName()
    {
        return Environment.MachineName;
    }

    public static string GetUserName()
    {
        return Environment.UserName;
    }

    public static string GetWindowsVersion()
    {
        return Environment.OSVersion.VersionString;
    }

    public static string GetProcessor()
    {
        using ManagementObjectSearcher searcher =
            new("SELECT Name FROM Win32_Processor");

        foreach (ManagementObject obj in searcher.Get())
        {
            return obj["Name"]?.ToString() ?? "Unbekannt";
        }

        return "Unbekannt";
    }
}