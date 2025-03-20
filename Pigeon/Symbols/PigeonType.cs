namespace Kostic017.Pigeon.Symbols
{
    public class PigeonType
    {
        internal static readonly PigeonType Error = new("?");

        public static readonly PigeonType Any = new("*");
        public static readonly PigeonType Void = new("void");
        public static readonly PigeonType Bool = new("bool");
        public static readonly PigeonType Int = new("int");
        public static readonly PigeonType Float = new("float");
        public static readonly PigeonType String = new("string");
        public static readonly PigeonType IntList = new("int[]");
        public static readonly PigeonType FloatList = new("float[]");
        public static readonly PigeonType StringList = new("string[]");
        public static readonly PigeonType BoolList = new("bool[]");
        public static readonly PigeonType Dictionary = new("dict");
        public static readonly PigeonType Set = new("set");

        internal static PigeonType FromName(string name)
        {
            return name switch
            {
                "void" => Void,
                "bool" => Bool,
                "int" => Int,
                "float" => Float,
                "string" => String,
                "int[]" => IntList,
                "float[]" => FloatList,
                "string[]" => StringList,
                "bool[]" => BoolList,
                "dict" => Dictionary,
                "set" => Set,
                _ => Error,
            };
        }

        internal string Name { get; }

        internal PigeonType(string name)
        {
            Name = name;
        }
    }
}
