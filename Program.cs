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
            var path = GetCeVIOExecutable();
            if (string.IsNullOrEmpty(path))
            {
                Console.Write("CeVIO AI.exe not found, please specify the file: ");
                path = (Console.ReadLine() ?? "").Replace("\"", "");
            }
            var activator = new Activator(path);

            Console.WriteLine("Loading...");

            activator.ActivateProducts();
            Console.WriteLine("Activated all packages");

            activator.GenerateLicenseSummary();
            Console.WriteLine("Authorized");

            Console.WriteLine("Completed");
            Console.WriteLine("You should reactivate CeVIO AI before " + DateTime.Now.AddDays(365).ToLongDateString());
            Console.ReadLine();
        }

        private static string GetCeVIOExecutable()
        {
            var folder = "";
            using (var reg = Registry.LocalMachine.OpenSubKey("SOFTWARE\\CeVIO_NV\\Subject\\Editor\\x64"))
            {
                if (reg == null)
                {
                    return null;
                }
                folder = reg.GetValue("InstallFolder") as string;
            }

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