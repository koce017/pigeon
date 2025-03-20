namespace Kostic017.Pigeon.Symbols
{
    public class Function
    {
        internal string Name { get; }
        internal PigeonType ReturnType { get; }
        internal Variable[] Parameters { get; }
        internal object FuncBody { get; }
        
        internal Function(PigeonType returnType, string name, Variable[] parameters, object funcBody)
        {
            Name = name;
            ReturnType = returnType;
            Parameters = parameters;
            FuncBody = funcBody;
        }
    }
}
