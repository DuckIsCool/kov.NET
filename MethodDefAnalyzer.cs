using dnlib.DotNet;

namespace kov.NET.Utils.Analyzer
{
	public class MethodDefAnalyzer : DefAnalyzer
	{
		public override bool Execute(object context)
		{
			MethodDef method = (MethodDef)context;
			if (method.IsRuntimeSpecialName)
				return false;
			if (method.DeclaringType.IsForwarder)
				return false;
            if (method.IsConstructor || method.IsStaticConstructor)
                return false;
			return true;
		}
	}
}