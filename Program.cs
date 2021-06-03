using System;
using System.IO;
using dnlib.DotNet;
using dnlib.DotNet.Writer;
using kov.NET.Protections;
using kov.NET.Utils;

namespace kov.NET
{
    class Program
    {
        public static ModuleDefMD Module { get; set; }
        public ModuleDef ManifestModule;

        public static string FileExtension { get; set; }

        public static bool DontRename { get; set; }

        public static bool ForceWinForms { get; set; }

        public static string FilePath { get; set; }

        static void Main(string[] args)
        {
            Console.WriteLine("Drag n drop your file : ");
            string path = Console.ReadLine();
            Module = ModuleDefMD.Load(path);
            FileExtension = Path.GetExtension(path);

            Console.WriteLine("Encrypting strings...");
            StringEncryption.Execute();

            Console.WriteLine("Renaming...");
            Renamer.Execute();


            Console.WriteLine("Adding ints...");
            AddInteger.Execute();


            Console.WriteLine("Encoding ints...");
            IntEncoding.Execute();

            
            Console.WriteLine("Injecting ControlFlow...");
            ControlFlow.Execute();


            Console.WriteLine("Injecting local to fields...");
            L2F.Execute();


            Console.WriteLine("Saving file...");
            var pathez = $"{Path.GetFileNameWithoutExtension(path)}-kov.exe";
            ModuleWriterOptions opts = new ModuleWriterOptions(Module) { Logger = DummyLogger.NoThrowInstance };
            Module.Write(pathez, opts);


            Console.WriteLine("Done! Press any key to exit...");
            Console.ReadKey();
        }
    }
}
