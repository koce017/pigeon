namespace Kostic017.Pigeon.Symbols
{
    class FunctionScope
    {
        private Scope scope;

        internal FunctionScope(GlobalScope globalScope)
        {
            scope = new Scope(globalScope);
        }

        internal void Declare(PigeonType type, string name, object value)
        {
            scope.DeclareVariable(type, name, false);
            Assign(name, value);
        }

        internal void Assign(string name, object value)
        {
            scope.Assign(name, value);
        }

        internal object Evaluate(string name)
        {
            return scope.Evaluate(name) ?? scope.Parent.Evaluate(name);
        }

        internal void EnterScope()
        {
            scope = new Scope(scope);
        }

        internal void ExitScope()
        {
            scope = scope.Parent;
        }
    }
}
