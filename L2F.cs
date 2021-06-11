using dnlib.DotNet;
using dnlib.DotNet.Emit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kov.NET.Protections
{
    internal class L2F
    {
        private static Dictionary<Local, FieldDef> convertedLocals = new Dictionary<Local, FieldDef>();
        private static Random random = new Random();

        public static void Execute()
        {
            foreach (var type in Program.Module.Types.Where(x => x != Program.Module.GlobalType))
            {
                foreach (var method2 in type.Methods.Where(x => x.HasBody && x.Body.HasInstructions && !x.IsConstructor))
                {
                    convertedLocals = new Dictionary<Local, FieldDef>();
                    Process(Program.Module, method2);
                }
            }
        }
        public static string RandomString(int length)
        {
            const string chars = "duckduckduckduckduckduckduckduckduckduckduck";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        public static void Process(ModuleDef Module, MethodDef method)
        {
            method.Body.SimplifyMacros(method.Parameters);
            var instructions = method.Body.Instructions;
            foreach (var t in instructions)
            {
                if (!(t.Operand is Local local)) continue;
                FieldDef def = null;
                if (!convertedLocals.ContainsKey(local))
                {
                    def = new FieldDefUser(RandomString(+30), new FieldSig(local.Type), FieldAttributes.Public | FieldAttributes.Static);
                    Module.GlobalType.Fields.Add(def);
                    convertedLocals.Add(local, def);
                }
                else
                    def = convertedLocals[local];

                OpCode eq = null;
                switch (t.OpCode.Code)
                {
                    case Code.Ldloc:
                        eq = OpCodes.Ldsfld;
                        break;

                    case Code.Ldloca:
                        eq = OpCodes.Ldsflda;
                        break;

                    case Code.Stloc:
                        eq = OpCodes.Stsfld;
                        break;
                }
                t.OpCode = eq;
                t.Operand = def;
            }
            convertedLocals.ToList().ForEach(x => method.Body.Variables.Remove(x.Key));
            convertedLocals = new Dictionary<Local, FieldDef>();
        }
    }
}
