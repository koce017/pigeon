using System.Collections.Generic;
using System.Linq;

namespace Kostic017.Pigeon.Symbols
{
    class GlobalScope : Scope
    {
        private readonly Dictionary<string, Function> functions = [];

        internal GlobalScope() : base(null)
        {
        }

        internal Function DeclareFunction(PigeonType returnType, string name, Variable[] parameters, object funcBody)
        {
            string signature = name + "(" + string.Join(", ", parameters.Select(p => p.Type.Name)) + ")";
            var function = new Function(returnType, name, parameters, funcBody);
            functions.Add(signature, function);
            return function;
        }

        internal bool TryGetFunction(string name, out Function function)
        {
            if (functions.TryGetValue(name, out function))
                return true;
            return false;
        }
    }
}
