using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using kov.NET.Utils;

namespace kov.NET.Protections
{
    public class StringEncryption : Randomizer
    {
        private static int Amount { get; set; }

        public static void Execute()
        {
            ModuleDefMD typeModule = ModuleDefMD.Load(typeof(StringDecoder).Module);
            TypeDef typeDef = typeModule.ResolveTypeDef(MDToken.ToRID(typeof(StringDecoder).MetadataToken));
            IEnumerable<IDnlibDef> members = InjectHelper.Inject(typeDef, Program.Module.GlobalType,
                Program.Module);
            MethodDef init = (MethodDef)members.Single(method => method.Name == "Decrypt");
            init.Rename(GenerateRandomString(MemberRenamer.StringLength()));

            foreach (MethodDef method in Program.Module.GlobalType.Methods)
                if (method.Name.Equals(".ctor"))
                {
                    Program.Module.GlobalType.Remove(method);
                    break;
                }

            foreach (TypeDef type in Program.Module.Types)
            {
                if (type.IsGlobalModuleType) continue;
                foreach (MethodDef method in type.Methods)
                {
                    var cryptoRandom = new CryptoRandom();
                    if (!method.HasBody) continue;
                    for (int i = 0; i < method.Body.Instructions.Count; i++)
                        if (method.Body.Instructions[i].OpCode == OpCodes.Ldstr)
                        {
                            var key = method.Name.Length + Next();

                            var encryptedString =
                                Encrypt(new Tuple<string, int>(method.Body.Instructions[i].Operand.ToString(), key));

                            method.Body.Instructions[i].OpCode = OpCodes.Ldstr;
                            method.Body.Instructions[i].Operand = encryptedString;
                            method.Body.Instructions.Insert(i + 1, OpCodes.Ldc_I4.ToInstruction(key));
                            method.Body.Instructions.Insert(i + 2, OpCodes.Call.ToInstruction(init));
                            Amount++;
                            i += 2;
                        }
                }
            }

            Console.WriteLine($"  Encrypted {Amount} strings.");
        }
        public static int Next()
        {
            return BitConverter.ToInt32(RandomBytes(sizeof(int)), 0);
        }
        private static readonly RandomNumberGenerator csp = RandomNumberGenerator.Create();
        private static byte[] RandomBytes(int bytes)
        {
            byte[] buffer = new byte[bytes];
            csp.GetBytes(buffer);
            return buffer;
        }
        public static string Encrypt(Tuple<string, int> values)
        {
            StringBuilder input = new StringBuilder(values.Item1);
            StringBuilder output = new StringBuilder(values.Item1.Length);
            char Textch;
            int key = values.Item2;
            for (int iCount = 0; iCount < values.Item1.Length; iCount++)
            {
                Textch = input[iCount];
                Textch = (char)(Textch ^ key);
                output.Append(Textch);
            }
            return output.ToString();
        }
    }
}
