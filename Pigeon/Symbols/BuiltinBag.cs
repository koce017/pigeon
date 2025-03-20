using System.Collections.Generic;
using System.Linq;

namespace Kostic017.Pigeon.Symbols
{
    public class BuiltinBag
    {
        private readonly Dictionary<string, Variable> variables = [];
        private readonly Dictionary<string, Function> functions = [];

        internal void Register(GlobalScope globalScope)
        {
            foreach (var v in variables.Values)
                globalScope.DeclareVariable(v.Type, v.Name, v.ReadOnly, v.Value);
            foreach (var f in functions.Values)
                globalScope.DeclareFunction(f.ReturnType, f.Name, f.Parameters, f.FuncBody);
        }

        public void RegisterVariable(PigeonType type, string name, bool readOnly, object value)
        {
            variables.Add(name, new Variable(type, name, readOnly) { Value = value });
        }

        public void RegisterFunction(PigeonType returnType, string name, FuncPointer funcPointer, params PigeonType[] parameters)
        {
            if (returnType == PigeonType.Any)
            {
                functions.Add(name, new Function(PigeonType.String, name, parameters.Select(p => new Variable(p)).ToArray(), funcPointer));
                functions.Add(name + "_i", new Function(PigeonType.Int, name + "_i", parameters.Select(p => new Variable(p)).ToArray(), funcPointer));
                functions.Add(name + "_f", new Function(PigeonType.Float, name + "_f", parameters.Select(p => new Variable(p)).ToArray(), funcPointer));
                functions.Add(name + "_b", new Function(PigeonType.Bool, name + "_b", parameters.Select(p => new Variable(p)).ToArray(), funcPointer));
            }
            else
            {
                functions.Add(name, new Function(returnType, name, parameters.Select(p => new Variable(p)).ToArray(), funcPointer));
            }
        }

    }
}
