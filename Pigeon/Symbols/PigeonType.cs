namespace Kostic017.Pigeon.Symbols
{
    public class PigeonType
    {
        internal static readonly PigeonType Error = new PigeonType("?");

        public static readonly PigeonType Any = new PigeonType("*");
        public static readonly PigeonType Bool = new PigeonType("bool");
        public static readonly PigeonType Int = new PigeonType("int");
        public static readonly PigeonType Float = new PigeonType("float");
        public static readonly PigeonType String = new PigeonType("string");
        public static readonly PigeonType Void = new PigeonType("void");

        internal static PigeonType FromName(string name)
        {
            switch (name)
            {
                case "bool": return Bool;
                case "int": return Int;
                case "float": return Float;
                case "string": return String;
                case "void": return Void;
                default: return Error;
            }
        }

        internal string Name { get; }

        internal PigeonType(string name)
        {
            Name = name;
        }
    }
}
