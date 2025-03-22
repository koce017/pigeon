using System.Collections.Generic;

namespace Kostic017.Pigeon.Symbols
{
    class Scope
    {
        private readonly Dictionary<string, Variable> variables = [];

        internal Scope Parent { get; }

        internal Scope(Scope parent)
        {
            Parent = parent;
        }

        internal Variable DeclareVariable(PigeonType type, string name, bool readOnly = false, object value = null)
        {
            var variable = new Variable(type, name, readOnly) { Value = value };
            variables.Add(variable.Name, variable);
            return variable;
        }

        internal bool TryGetVariable(string name, out Variable variable)
        {
            variable = null;
            var scope = this;
            while (scope != null)
            {
                if (scope.variables.TryGetValue(name, out variable))
                    return true;
                scope = scope.Parent;
            }
            return false;
        }

        internal bool IsDeclaredHere(string name)
        {
            return variables.ContainsKey(name);
        }

        internal void Assign(string name, object value)
        {
            TryGetVariable(name, out var variable);
            variable.Value = value;
        }

        internal object Evaluate(string name)
        {
            TryGetVariable(name, out var variable);
            return variable?.Value;
        }
    }
}
