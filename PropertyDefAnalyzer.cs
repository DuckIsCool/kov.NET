using dnlib.DotNet;

namespace kov.NET.Utils.Analyzer
{
	public class PropertyDefAnalyzer : DefAnalyzer
	{
		public override bool Execute(object context)
		{
			PropertyDef propertyDef = (PropertyDef)context;
			bool isRuntimeSpecialName = propertyDef.IsRuntimeSpecialName;
			bool result;
			if (isRuntimeSpecialName)
			{
				result = false;
			}
			else
			{
				bool isEmpty = propertyDef.IsEmpty;
				if (isEmpty)
				{
					result = false;
				}
				else
				{
					bool isSpecialName = propertyDef.IsSpecialName;
					result = !isSpecialName;
				}
			}
			return result;
		}
	}
}
