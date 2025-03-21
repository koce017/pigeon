using System.Collections.Generic;
using System.Linq;

namespace Kostic017.Pigeon.Symbols
{
    public class BuiltinBag
    {
        private readonly Dictionary<string, Variable> variables = [];
        private readonly Dictionary<string, Function> functions = [];

        public void RegisterVariable(PigeonType type, string name, bool readOnly, object value)
        {
            variables.Add(name, new Variable(type, name, readOnly) { Value = value });
        }

        public void RegisterFunction(PigeonType returnType, string name, FuncPointer funcPointer, params PigeonType[] parameters)
        {
            string signature = name + "(" + string.Join(", ", parameters.Select(p => p.Name)) + ")";

            if (returnType == PigeonType.Any)
            {
                functions.Add(signature, new Function(PigeonType.String, name, parameters.Select(p => new Variable(p)).ToArray(), funcPointer));
                functions.Add(signature + "_i", new Function(PigeonType.Int, name + "_i", parameters.Select(p => new Variable(p)).ToArray(), funcPointer));
                functions.Add(signature + "_f", new Function(PigeonType.Float, name + "_f", parameters.Select(p => new Variable(p)).ToArray(), funcPointer));
                functions.Add(signature + "_b", new Function(PigeonType.Bool, name + "_b", parameters.Select(p => new Variable(p)).ToArray(), funcPointer));
            }
            else
            {
                functions.Add(signature, new Function(returnType, name, parameters.Select(p => new Variable(p)).ToArray(), funcPointer));
            }
        }

        internal void PopulateGlobalScope(GlobalScope globalScope)
        {
            foreach (var v in variables.Values)
                globalScope.DeclareVariable(v.Type, v.Name, v.ReadOnly, v.Value);
            foreach (var f in functions.Values)
                globalScope.DeclareFunction(f.ReturnType, f.Name, f.Parameters, f.FuncBody);
        }

    }
}
