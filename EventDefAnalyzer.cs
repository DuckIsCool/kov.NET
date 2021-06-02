using dnlib.DotNet;

namespace kov.NET.Utils.Analyzer
{
	public class EventDefAnalyzer : DefAnalyzer
    {
		public override bool Execute(object context)
		{
			EventDef ev = (EventDef)context;
			if (ev.IsRuntimeSpecialName)
				return false;
			return true;
		}
	}
}