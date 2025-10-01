using Kostic017.Pigeon.Symbols;
using System.Collections.Generic;

namespace Kostic017.Pigeon
{
    public static class SyntaxFacts
    {
        public static readonly HashSet<string> Keywords = new HashSet<string>()
        {
            "let",
            "const",
            "if",
            "else",
            "for",
            "to",
            "while",
            "do",
            "break",
            "continue",
            "return",
        };

        public static readonly HashSet<string> Types = new HashSet<string>()
        {
            PigeonType.Void.Name,
            PigeonType.Int.Name,
            PigeonType.Float.Name,
            PigeonType.String.Name,
            PigeonType.Bool.Name,
            PigeonType.IntList.Name,
            PigeonType.FloatList.Name,
            PigeonType.StringList.Name,
            PigeonType.BoolList.Name,
            PigeonType.Set.Name,
        };

        public static readonly HashSet<string> ComplexLiterals = new HashSet<string>()
        {
            "set()",
            "list<int>()",
            "list<float>()",
            "list<string>()",
            "list<bool>()",
        };

        internal static readonly HashSet<char> EscapeChars = new HashSet<char>()
        {
            '\\',
            't',
            'n',
            '"',
        };
    }
}
