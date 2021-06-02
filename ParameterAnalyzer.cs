using dnlib.DotNet;

namespace kov.NET.Utils.Analyzer
{
    public class ParameterAnalyzer : DefAnalyzer
    {
        public override bool Execute(object context)
        {
            Parameter param = (Parameter)context;
            if (param.Name == string.Empty)
                return false;
            return true;
        }
    }
}
