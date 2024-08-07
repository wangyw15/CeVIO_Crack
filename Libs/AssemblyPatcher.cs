﻿using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace CeVIOActivator
{
    public static class AssemblyPatcher
    {
        private const string TARGET_FILE = "CeVIO.ToolBarControl.dll";
        private const string TARGET_CLASS = "CeVIO.Editor.MissionAssistant.Authorizer";

        [Obsolete("Directly patch executable will make it not work. Use PatchFile instead.")]
        public static void PatchExecutable(string cevioExecutablePath)
        {
            var resolver = new DefaultAssemblyResolver();
            resolver.AddSearchDirectory(Path.GetDirectoryName(cevioExecutablePath));

            var asm = AssemblyDefinition.ReadAssembly(cevioExecutablePath, new ReaderParameters
            {
                AssemblyResolver = resolver
            });
            var type = asm.MainModule.GetType("CeVIO.Editor.MissionAssistant.Authorizer");
            var setter = type.Properties.First(x => x.Name == "HasAuthorized").SetMethod;
            var method = type.Methods.First(x => x.Name == "Authorize");
            var processor = method.Body.GetILProcessor();
            processor.Replace(method.Body.Instructions[2], processor.Create(OpCodes.Nop));
            processor.Replace(method.Body.Instructions[3], processor.Create(OpCodes.Ldc_I4_1));
            processor.Replace(method.Body.Instructions[4], processor.Create(OpCodes.Call, setter));
            asm.Write("CeVIO AI.exe");
        }

        public static bool PatchFile(string cevioInstallPath)
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

            // detect if patched
            if (method.Body.Instructions.Any(x => x.Operand as string == TARGET_CLASS))
            {
                return false;
            }

            // generate instructions
            var processor = method.Body.GetILProcessor();
            var instructions = new Instruction[]
            {
                processor.Create(OpCodes.Call, module.ImportReference(typeof(Assembly).GetMethod("GetEntryAssembly"))),
                processor.Create(OpCodes.Ldstr, TARGET_CLASS),
                processor.Create(OpCodes.Callvirt, module.ImportReference(typeof(Assembly).GetMethod("GetType", new Type[] { typeof(string) }))),
                processor.Create(OpCodes.Ldstr, "HasAuthorized"),
                processor.Create(OpCodes.Callvirt, module.ImportReference(typeof(Type).GetMethod("GetProperty", new Type[] { typeof(string) }))),
                processor.Create(OpCodes.Ldnull),
                processor.Create(OpCodes.Ldc_I4_1),
                processor.Create(OpCodes.Box, module.ImportReference(typeof(bool))),
                processor.Create(OpCodes.Callvirt, module.ImportReference(typeof(PropertyInfo).GetMethod("SetValue", new Type[] { typeof(object), typeof(object) })))
            };

            // patch
            for (var i = instructions.Length - 1; i >= 0; i--)
            {
                processor.InsertBefore(method.Body.Instructions[0], instructions[i]);
            }
            
            // write
            module.Write(TARGET_FILE);

            return true;
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
            // backup unmodified file
            File.Copy(targetPath, targetPath + ".bak", true);
            // replace
            File.Copy(sourcePath, targetPath, true);
            // delete source
            File.Delete(sourcePath);

            // old method by cmd
            //var process = new Process();
            //process.StartInfo.FileName = "cmd.exe";
            //process.StartInfo.Arguments = $"/c \"timeout 1 /nobreak & copy /y \"{targetPath}\" \"{targetPath}.bak\" & copy /y \"{sourcePath}\" \"{targetPath}\" & del \"{sourcePath}\" & echo Completed & pause\"";
            //process.StartInfo.UseShellExecute = false;
            //process.Start();
        }
    }
}
