using dnlib.DotNet;
using dnlib.DotNet.Emit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kov.NET
{
	public class CFHelper
	{
		public bool HasUnsafeInstructions(MethodDef methodDef)
		{
			bool result;
			if (methodDef.HasBody && methodDef.Body.HasVariables)
			{
				result = methodDef.Body.Variables.Any((Local x) => x.Type.IsPointer);
			}
			else
			{
				result = false;
			}
			return result;
		}
		public class Blocks
		{
			public Block getBlock(int id)
			{
				return this.blocks.Single((Block block) => block.ID == id);
			}

			public void Scramble(out Blocks incGroups)
			{
				Blocks blocks = new Blocks();
				foreach (Block item in this.blocks)
				{
					blocks.blocks.Insert(Blocks.generator.Next(0, blocks.blocks.Count), item);
				}
				incGroups = blocks;
			}

			public List<Block> blocks = new List<Block>();

			private static Random generator = new Random();
		}
		public Blocks GetBlocks(MethodDef method)
		{
			Blocks blocks = new Blocks();
			Block block = new Block();
			int num = 0;
			block.ID = 0;
			int num2 = 1;
			block.nextBlock = block.ID + 1;
			block.instructions.Add(Instruction.Create(OpCodes.Nop));
			blocks.blocks.Add(block);
			block = new Block();
			foreach (Instruction instruction in method.Body.Instructions)
			{
				int num3 = 0;
				int num4;
				instruction.CalculateStackUsage(out num4, out num3);
				block.instructions.Add(instruction);
				num += num4 - num3;
				if (num4 == 0 && instruction.OpCode != OpCodes.Nop && (num == 0 || instruction.OpCode == OpCodes.Ret))
				{
					block.ID = num2;
					num2++;
					block.nextBlock = block.ID + 1;
					blocks.blocks.Add(block);
					block = new Block();
				}
			}
			return blocks;
		}
		public class Block
		{
			public int ID = 0;

			public int nextBlock = 0;

			public List<Instruction> instructions = new List<Instruction>();
		}
		public List<Instruction> Calc(int value)
		{
			List<Instruction> list = new List<Instruction>();
			int num = CFHelper.generator.Next(0, 2147483647);
			bool flag = Convert.ToBoolean(CFHelper.generator.Next(2147483647));
			int num2 = CFHelper.generator.Next(2147483647);
			list.Add(Instruction.Create(OpCodes.Ldc_I4, value - num + (flag ? (0 - num2) : num2)));
			list.Add(Instruction.Create(OpCodes.Ldc_I4, num));
			list.Add(Instruction.Create(OpCodes.Add));
			list.Add(Instruction.Create(OpCodes.Ldc_I4, num2));
			list.Add(Instruction.Create(flag ? OpCodes.Add : OpCodes.Sub));
			return list;
		}

		private static Random generator = new Random();
	}
}
