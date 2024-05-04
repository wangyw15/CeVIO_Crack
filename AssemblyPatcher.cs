using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace CeVIO_crack
{
    public static class AssemblyPatcher
    {
        private const string TARGET_FILE = "CeVIO.ToolBarControl.dll";
        public static void PatchFile(string cevioInstallPath)
        {
            // System.Void CeVIO.ToolBarControl.ToolBarControl::.cctor()
            // System.Reflection.Assembly.GetEntryAssembly().GetType("CeVIO.Editor.MissionAssistant.Authorizer").GetProperty("HasAuthorized").SetValue(null, true);

            var modulePath = Path.Combine(cevioInstallPath, TARGET_FILE);
            if (!File.Exists(modulePath))
            {
                throw new FileNotFoundException($"{TARGET_FILE} not found");
            }

            // find method
            var module = ModuleDefinition.ReadModule(modulePath);
            var type = module.GetType("CeVIO.ToolBarControl.ToolBarControl");
            var method = type.Methods.First(m => m.Name == ".cctor");

            // patch
            var processor = method.Body.GetILProcessor();
            var instructions = new Instruction[]
            {
                processor.Create(OpCodes.Call, module.ImportReference(typeof(Assembly).GetMethod("GetEntryAssembly"))),
                processor.Create(OpCodes.Ldstr, "CeVIO.Editor.MissionAssistant.Authorizer"),
                processor.Create(OpCodes.Callvirt, module.ImportReference(typeof(Assembly).GetMethod("GetType", new Type[] { typeof(string) }))),
                processor.Create(OpCodes.Ldstr, "HasAuthorized"),
                processor.Create(OpCodes.Callvirt, module.ImportReference(typeof(Type).GetMethod("GetProperty", new Type[] { typeof(string) }))),
                processor.Create(OpCodes.Ldnull),
                processor.Create(OpCodes.Ldc_I4_1),
                processor.Create(OpCodes.Box, module.ImportReference(typeof(bool))),
                processor.Create(OpCodes.Callvirt, module.ImportReference(typeof(PropertyInfo).GetMethod("SetValue", new Type[] { typeof(object), typeof(object) })))
            };

            for (var i = instructions.Length - 1; i >= 0; i--)
            {
                processor.InsertBefore(method.Body.Instructions[0], instructions[i]);
            }
            
            // write
            module.Write(TARGET_FILE);
        }

        public static void DeleteNgen(string cevioInstallPath)
        {
            foreach (var i in Directory.GetFiles(cevioInstallPath))
            {
                if (!Regex.IsMatch(Path.GetFileName(i), @"cevio.*\.(?:exe|dll)", RegexOptions.IgnoreCase))
                {
                    continue;
                }
                var process = new Process();
                process.StartInfo.FileName = "C:\\Windows\\Microsoft.NET\\Framework64\\v4.0.30319\\ngen.exe";
                process.StartInfo.Arguments = $"uninstall \"{i}\"";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.CreateNoWindow = true;
                process.Start();
                process.WaitForExit();
                Console.WriteLine("ngen uninstalled " + i);
            }
        }

        public static void ReplaceFile(string cevioInstallPath)
        {
            var sourcePath = Path.GetFullPath(TARGET_FILE);
            var targetPath = Path.Combine(cevioInstallPath, TARGET_FILE);
            var process = new Process();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.Arguments = $"/c \"timeout 1 /nobreak & copy /y \"{sourcePath}\" \"{targetPath}\" & del \"{sourcePath}\" & echo Completed & pause\"";
            process.StartInfo.UseShellExecute = false;
            process.Start();
        }
    }
}
