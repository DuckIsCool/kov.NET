﻿using System;
using System.Collections.Generic;
using System.Linq;
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
                    if (!method.HasBody) continue;
                    for (int i = 0; i < method.Body.Instructions.Count; i++)
                        if (method.Body.Instructions[i].OpCode == OpCodes.Ldstr)
                        {
                            string operand = method.Body.Instructions[i].Operand.ToString();
                            method.Body.Instructions[i].Operand = Encrypt(operand);
                            method.Body.Instructions.Insert(i + 1, OpCodes.Call.ToInstruction(init));
                            ++Amount;
                        }
                }
            }

            Console.WriteLine($"  Encrypted {Amount} strings.");
        }

        private static string Encrypt(string str)
        {

            char[] charArray = str.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }
    }
}