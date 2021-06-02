using dnlib.DotNet;

namespace kov.NET.Utils.Analyzer
{
	public class FieldDefAnalyzer : DefAnalyzer
	{
		public override bool Execute(object context)
		{
			FieldDef field = (FieldDef)context;
			if (field.IsRuntimeSpecialName)
				return false;
			if (field.IsLiteral && field.DeclaringType.IsEnum)
				return false;
			return true;
		}
	}
}