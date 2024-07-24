using System;
using System.IO;
using Microsoft.Win32;

namespace CeVIOActivator
{
    class Program
    {
        // N54KC-7U2ZL-PQZBM-SPF8H suzuki trial key

        static void Main(string[] args)
        {
            var executablePath = GetCeVIOExecutable();
            if (string.IsNullOrEmpty(executablePath))
            {
                Console.Write("CeVIO AI.exe not found, please specify the file: ");
                executablePath = (Console.ReadLine() ?? "").Replace("\"", "");
            }
            
            Console.WriteLine("Loading...");
            var activator = new Activator(executablePath);
            activator.ActivateProducts(activator.OfflineAcceptablePeriod);
            Console.WriteLine("Activated all packages");

            activator.GenerateLicenseSummary();
            Console.WriteLine("Authorized");

            var installFolder = GetCeVIOInstallFolder();

            var thisTimePatched = AssemblyPatcher.PatchFile(installFolder);

            if (thisTimePatched)
            {
                Console.WriteLine("Patching CeVIO.ToolBarControl.dll");
                Console.WriteLine("Deleting Ngen");
                AssemblyPatcher.DeleteNgen(installFolder);

                activator.Dispose(); // release the assembly

                Console.WriteLine("Replacing the file to enable offline export");
                AssemblyPatcher.ReplaceFile(installFolder);
            }
            else
            {
                Console.WriteLine("CeVIO.ToolBarControl.dll already patched, skip");
            }
            
            Console.WriteLine("You should reactivate CeVIO AI before " + (DateTime.Now + activator.OfflineAcceptablePeriod).ToLongDateString());

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        private static string GetCeVIOInstallFolder()
        {
            using (var reg = Registry.LocalMachine.OpenSubKey("SOFTWARE\\CeVIO_NV\\Subject\\Editor\\x64"))
            {
                if (reg == null)
                {
                    return null;
                }
                return reg.GetValue("InstallFolder") as string;
            }
        }

        private static string GetCeVIOExecutable()
        {
            var folder = GetCeVIOInstallFolder();

            if (string.IsNullOrEmpty(folder))
            {
                return null;
            }

            foreach (var file in Directory.GetFiles(folder))
            {
                if (Path.GetExtension(file).ToLower() == ".exe" && Path.GetFileName(file).StartsWith("CeVIO"))
                {
                    return file;
                }
            }

            return null;
        }
    }
}