using System.Diagnostics;

var exeFolder = Path.GetDirectoryName(Process.GetCurrentProcess()!.MainModule!.FileName);
Console.WriteLine("Working Dir {0}", exeFolder);
void Run(ProcessStartInfo info)
{
    var process = new Process { StartInfo = info };
    process.Start();
    process.WaitForExit();
}

var clean = new ProcessStartInfo();
clean.FileName = "dotnet";
clean.Arguments = "clean ../../../../Hud1/Hud1.csproj";

var publish = new ProcessStartInfo();
publish.FileName = "dotnet";
publish.Arguments = "publish -p:PublishProfile=Hud1/Properties/PublishProfiles/FastProfile ../../../../Hud1/Hud1.csproj";

var inno = new ProcessStartInfo();
inno.FileName = "C:/Program Files (x86)/Inno Setup 6/ISCC.exe";
inno.Arguments = "../../../app.installer.iss";

var binDir = "../../../../Hud1/bin";
Run(clean);
if (Directory.Exists(binDir))
{
    Console.WriteLine("Delete Folder");
    Directory.Delete(binDir, true);
}
Run(publish);

var bak = binDir + "/Publish/hostfxr.dll.bak";
if (File.Exists(bak))
{
    Console.WriteLine("Delete Folder");
    File.Delete(bak);
}
Run(inno);

Process.Start("explorer.exe", "C:\\Workspaces\\nico_2024\\hud1\\Hud1\\Hud1\\bin\\Installer");