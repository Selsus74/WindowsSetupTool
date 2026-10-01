//---imports---

//---namespace---
namespace WindowsSetupTool.Models
{
    //---internal enum for paketmanger type---
    internal enum PaketManager { Winget, Chocolatey, Direkt }

    //---class init---
    internal class Software
    {
        //---fields---
        public string Name { get; set; } = string.Empty;
        public PaketManager PaketManager { get; set; } = PaketManager.Winget;
        public string PaketId { get; set; } = string.Empty;
    }
}