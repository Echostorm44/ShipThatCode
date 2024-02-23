using System.Diagnostics;
using System.IO.Compression;
using System.Reflection;
// Icon <a href="https://www.flaticon.com/free-icons/ship" title="ship icons">Ship icons created by Freepik - Flaticon</a>
//dotnet publish -c Release -r win-x64 -p:PublishReadyToRun=true -f net8.0 -o ./publish/ --sc true
//   -- Not sure about this, not needed  /p:IncludeNativeLibrariesForSelfExtract=true

class Program
{
    static void Main(string[] args)
    {
        try
        {
            string tempFolder = Path.Combine(Path.GetTempPath(), "ShipThatInstaller");
            if(Directory.Exists(tempFolder))
            {
                Console.WriteLine($"Deleting Temp Folder: {tempFolder}");
                Directory.Delete(tempFolder, true);
            }
            Console.WriteLine($"Creating Temp Folder: {tempFolder}");
            Directory.CreateDirectory(tempFolder);
            ExtractResource("ShipThatInstaller.Payload.zip", tempFolder);
            string setupFile = Path.Combine(tempFolder, "setup.exe");

            if(File.Exists(setupFile))
            {
                Console.WriteLine($"Executing: {setupFile}");
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = setupFile,
                    UseShellExecute = false,
                };
                using Process process = new Process { StartInfo = startInfo };
                process.Start();
                process.WaitForExit();
            }
            else
            {
                Console.WriteLine("Setup file not found.");
            }
            Console.WriteLine("Cleaning Up Temp Data...");
            Directory.Delete(tempFolder, true);
        }
        catch(Exception ex)
        {
            Console.WriteLine(ex.ToString());
            Console.ReadKey();
        }
    }

    // Extracts an embedded resource to a specified directory
    static void ExtractResource(string resourceName, string targetDirectory)
    {
        using Stream resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
        if(resourceStream != null)
        {
            using var zipStream = new ZipArchive(resourceStream);
            zipStream.ExtractToDirectory(targetDirectory, true);
        }
    }
}