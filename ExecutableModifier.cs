using System.IO;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace CeVIO_crack
{
    public class ExecutableModifier
    {
        public static void Run(string executablePath)
        {
            var resolver = new DefaultAssemblyResolver();
            resolver.AddSearchDirectory(Path.GetDirectoryName(executablePath));

            var asm = AssemblyDefinition.ReadAssembly(executablePath, new ReaderParameters
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
    }
}
