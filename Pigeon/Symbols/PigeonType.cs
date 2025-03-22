namespace Kostic017.Pigeon.Symbols
{
    public class PigeonType
    {
        internal static readonly PigeonType Error = new PigeonType("?");

        public static readonly PigeonType Any = new PigeonType("*");
        public static readonly PigeonType Void = new PigeonType("void");
        public static readonly PigeonType Bool = new PigeonType("bool");
        public static readonly PigeonType Int = new PigeonType("int");
        public static readonly PigeonType Float = new PigeonType("float");
        public static readonly PigeonType String = new PigeonType("string");
        public static readonly PigeonType IntList = new PigeonType("[]int");
        public static readonly PigeonType FloatList = new PigeonType("[]float");
        public static readonly PigeonType StringList = new PigeonType("[]string");
        public static readonly PigeonType BoolList = new PigeonType("[]bool");
        public static readonly PigeonType Set = new PigeonType("set");

        internal static PigeonType FromName(string name)
        {
            switch (name)
            {
                case "?": return Error;
                case "*": return Any;
                case "void": return Void;
                case "int": return Int;
                case "float": return Float;
                case "string": return String;
                case "bool": return Bool;
                case "[]int": return IntList;
                case "[]float": return FloatList;
                case "[]string": return StringList;
                case "[]bool": return BoolList;
                case "set": return Set;
                default: return Error;
            };
        }

        internal string Name { get; }

        internal PigeonType(string name)
        {
            Name = name;
        }
    }
}
