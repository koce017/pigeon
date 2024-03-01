namespace Kostic017.Pigeon.Symbols
{
    public class Variable
    {
        internal PigeonType Type { get; }
        internal string Name { get; }
        internal bool ReadOnly { get; }
        internal object Value { get; set; }

        public Variable(PigeonType type)
        {
            Type = type;
        }

        internal Variable(PigeonType type, string name, bool readOnly)
        {
            Type = type;
            Name = name;
            ReadOnly = readOnly;
        }
    }
}
