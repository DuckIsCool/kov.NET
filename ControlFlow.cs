using dnlib.DotNet.Emit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kov.NET.Protections
{
    public static class ControlFlow
    {
        public static void Execute()
        {
            foreach (var type in Program.Module.Types)
            {
                foreach (var method in type.Methods.ToArray())
                {
                    if (!method.HasBody || !method.Body.HasInstructions || method.Body.HasExceptionHandlers) continue;
                    for (var i = 0; i < method.Body.Instructions.Count - 2; i++)
                    {
                        var inst = method.Body.Instructions[i + 1];
                        method.Body.Instructions.Insert(i + 1, Instruction.Create(OpCodes.Ldstr, L2F.RandomString(+600)));
                        method.Body.Instructions.Insert(i + 1, Instruction.Create(OpCodes.Br_S, inst));
                        i += 2;
                    }
                }
            }
        }
    }
}
