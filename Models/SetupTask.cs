namespace WindowsSetupTool.Models;

public class SetupTask
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsSelected { get; set; }
    public Action? Execute { get; set; }
}