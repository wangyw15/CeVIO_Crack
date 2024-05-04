using System;
using System.IO;
using Microsoft.Win32;

namespace CeVIO_crack
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
            activator.ActivateProducts();
            Console.WriteLine("Activated all packages");

            activator.GenerateLicenseSummary();
            Console.WriteLine("Authorized");

            var installFolder = GetCeVIOInstallFolder();

            Console.WriteLine("Patching CeVIO.ToolBarControl.dll");
            AssemblyPatcher.PatchFile(installFolder);

            Console.WriteLine("Deleting Ngen");
            AssemblyPatcher.DeleteNgen(installFolder);
            
            Console.WriteLine("You should reactivate CeVIO AI before " + DateTime.Now.AddDays(365).ToLongDateString());

            Console.WriteLine("Replace the file to enable offline export");
            AssemblyPatcher.ReplaceFile(installFolder);
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