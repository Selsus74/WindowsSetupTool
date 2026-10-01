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
            new() { Name = "Git",     PaketManager = PaketManager.Winget, PaketId = "Git.Git" },
            new() { Name = "VS Code", PaketManager = PaketManager.Winget, PaketId = "Microsoft.VisualStudioCode" },
        ];
    }
}