using dnlib.DotNet;
using dnlib.DotNet.Emit;
using kov.NET.Protections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace kov.NET
{
    internal class AntiDe4Dot
    {
        public static void Execute()
        {
            foreach (var module in Program.Module.Assembly.Modules)
            {
                string lol = ("kov.NET");
                for (var i = 0; i < 1; i++)
                {
                    var typeDef1 = new TypeDefUser(string.Empty, lol);
                    var interface1 = new InterfaceImplUser(typeDef1);
                    module.Types.Add(typeDef1);
                    typeDef1.Interfaces.Add(interface1);
                }
            }
        }
    }
}
