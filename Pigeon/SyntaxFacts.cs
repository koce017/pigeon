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
            "void",
            "int",
            "float",
            "string",
            "bool",
            "[]int",
            "[]float",
            "[]string",
            "[]bool",
            "set",
        };

        public static readonly HashSet<string> ComplexLiterals = new HashSet<string>()
        {
            "{}",
            "int[]",
            "float[]",
            "string[]",
            "bool[]",
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
