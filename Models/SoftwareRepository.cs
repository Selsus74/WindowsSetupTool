//---imports---

//---namespace---
namespace WindowsSetupTool.Models
{
    //---class init---
    internal class SoftwareRepository
    {
        //---public list with software---
        public IReadOnlyList<Software> GetAllSoftwares { get; } =
        [
            new() { Name = "Firefox", PaketManager = PaketManager.Winget, PaketId = "Mozilla.Firefox" },
            new() { Name = "7-Zip",     PaketManager = PaketManager.Winget, PaketId = "7zip.7zip" },
            new() { Name = "Thunderbird", PaketManager = PaketManager.Winget, PaketId = "Mozilla.Thunderbird" },
            new() { Name = "LibreOffice", PaketManager = PaketManager.Winget, PaketId = "TheDocumentFoundation.LibreOffice" },
            new() { Name = "PDF24", PaketManager = PaketManager.Winget, PaketId = "geeksoftwareGmbH.PDF24Creator" },
            new() { Name = "TeamViewer Full", PaketManager = PaketManager.Winget, PaketId = "TeamViewer.TeamViewer" },
            new() { Name = "TeamViewer Host", PaketManager = PaketManager.Winget, PaketId = "TeamViewer.TeamViewer.Host" },
        ];
    }
}