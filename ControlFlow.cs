using dnlib.DotNet;
using dnlib.DotNet.Emit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static kov.NET.CFHelper;

namespace kov.NET.Protections
{
    public static class ControlFlow
    {
		public static void Execute()
		{
			CFHelper cfhelper = new CFHelper();
			foreach (TypeDef typeDef in Program.Module.Types)
			{
				if (!typeDef.IsGlobalModuleType)
				{
					foreach (MethodDef methodDef in typeDef.Methods)
					{
						if (methodDef.HasBody && methodDef.Body.Instructions.Count > 0 && !methodDef.IsConstructor && !cfhelper.HasUnsafeInstructions(methodDef))
						{
							if (Simplify(methodDef))
							{
								Blocks blocks = cfhelper.GetBlocks(methodDef);
								if (blocks.blocks.Count != 1)
								{
									toDoBody(cfhelper, methodDef, blocks, typeDef);
								}
							}
							Optimize(methodDef);
						}
					}
				}
			}
		}

		public static bool Optimize(MethodDef methodDef)
		{
			bool result;
			if (methodDef.Body == null)
			{
				result = false;
			}
			else
			{
				methodDef.Body.OptimizeMacros();
				methodDef.Body.OptimizeBranches();
				result = true;
			}
			return result;
		}
		public static bool Simplify(MethodDef methodDef)
		{
			bool result;
			if (methodDef.Parameters == null)
			{
				result = false;
			}
			else
			{
				methodDef.Body.SimplifyMacros(methodDef.Parameters);
				result = true;
			}
			return result;
		}
		private static void toDoBody(CFHelper cFHelper, MethodDef method, Blocks blocks, TypeDef typeDef)
		{
			blocks.Scramble(out blocks);
			method.Body.Instructions.Clear();
			Local local = new Local(Program.Module.CorLibTypes.Int32);
			method.Body.Variables.Add(local);
			Instruction instruction = Instruction.Create(OpCodes.Nop);
			Instruction instruction2 = Instruction.Create(OpCodes.Br, instruction);
			foreach (Instruction item in cFHelper.Calc(0))
			{
				method.Body.Instructions.Add(item);
			}
			method.Body.Instructions.Add(Instruction.Create(OpCodes.Stloc, local));
			method.Body.Instructions.Add(Instruction.Create(OpCodes.Br, instruction2));
			method.Body.Instructions.Add(instruction);
			foreach (Block block in blocks.blocks)
			{
				if (block != blocks.getBlock(blocks.blocks.Count - 1))
				{
					method.Body.Instructions.Add(Instruction.Create(OpCodes.Ldloc, local));
					foreach (Instruction item2 in cFHelper.Calc(block.ID))
					{
						method.Body.Instructions.Add(item2);
					}
					method.Body.Instructions.Add(Instruction.Create(OpCodes.Ceq));
					Instruction instruction3 = Instruction.Create(OpCodes.Nop);
					method.Body.Instructions.Add(Instruction.Create(OpCodes.Brfalse, instruction3));
					foreach (Instruction item3 in block.instructions)
					{
						method.Body.Instructions.Add(item3);
					}
					foreach (Instruction item4 in cFHelper.Calc(block.nextBlock))
					{
						method.Body.Instructions.Add(item4);
					}
					method.Body.Instructions.Add(Instruction.Create(OpCodes.Stloc, local));
					method.Body.Instructions.Add(instruction3);
				}
			}
			method.Body.Instructions.Add(Instruction.Create(OpCodes.Ldloc, local));
			foreach (Instruction item5 in cFHelper.Calc(blocks.blocks.Count - 1))
			{
				method.Body.Instructions.Add(item5);
			}
			method.Body.Instructions.Add(Instruction.Create(OpCodes.Ceq));
			method.Body.Instructions.Add(Instruction.Create(OpCodes.Brfalse, instruction2));
			method.Body.Instructions.Add(Instruction.Create(OpCodes.Br, blocks.getBlock(blocks.blocks.Count - 1).instructions[0]));
			method.Body.Instructions.Add(instruction2);
			foreach (Instruction item6 in blocks.getBlock(blocks.blocks.Count - 1).instructions)
			{
				method.Body.Instructions.Add(item6);
			}
		}
	}

}
