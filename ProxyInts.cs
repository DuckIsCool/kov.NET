using dnlib.DotNet;
using dnlib.DotNet.Emit;
using kov.NET.Protections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kov.NET
{
    class ProxyInts
    {
        public static Random rand = new Random();
        private static int Amount { get; set; }
        public static void Execute()
        {
            var ManifestModule = Program.Module;
            foreach (TypeDef type in ManifestModule.GetTypes())
            {
                if (type.IsGlobalModuleType) continue;
                foreach (MethodDef method in type.Methods)
                {
                    if (!method.HasBody) continue;
                    var instr = method.Body.Instructions;
                    for (int i = 0; i < instr.Count; i++)
                    {
                        if (method.Body.Instructions[i].IsLdcI4())
                        {
                            var methImplFlags = MethodImplAttributes.IL | MethodImplAttributes.Managed;
                            var methFlags = MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.HideBySig | MethodAttributes.ReuseSlot;
                            var meth1 = new MethodDefUser(L2F.RandomString(+30),
                                        MethodSig.CreateStatic(ManifestModule.CorLibTypes.Int32),
                                        methImplFlags, methFlags);
                            ManifestModule.GlobalType.Methods.Add(meth1);
                            meth1.Body = new CilBody();
                            meth1.Body.Variables.Add(new Local(ManifestModule.CorLibTypes.Int32));
                            meth1.Body.Instructions.Add(Instruction.Create(OpCodes.Ldc_I4, instr[i].GetLdcI4Value()));
                            meth1.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));
                            instr[i].OpCode = OpCodes.Call;
                            instr[i].Operand = meth1;
                            Amount++;
                        }
                        else if (method.Body.Instructions[i].OpCode == OpCodes.Ldc_R4)
                        {
                            var methImplFlags = MethodImplAttributes.IL | MethodImplAttributes.Managed;
                            var methFlags = MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.HideBySig | MethodAttributes.ReuseSlot;
                            var meth1 = new MethodDefUser(L2F.RandomString(+30),
                                        MethodSig.CreateStatic(ManifestModule.CorLibTypes.Double),
                                        methImplFlags, methFlags);
                            ManifestModule.GlobalType.Methods.Add(meth1);
                            meth1.Body = new CilBody();
                            meth1.Body.Variables.Add(new Local(ManifestModule.CorLibTypes.Double));
                            meth1.Body.Instructions.Add(Instruction.Create(OpCodes.Ldc_R4, (float)method.Body.Instructions[i].Operand));
                            meth1.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));
                            instr[i].OpCode = OpCodes.Call;
                            instr[i].Operand = meth1;
                            Amount++;
                        }
                    }
                }
            }
            Console.WriteLine("   " + Amount + " ints proxied!");
        }
    }
}
