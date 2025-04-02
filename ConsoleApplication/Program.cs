using Kostic017.Pigeon;
using Kostic017.Pigeon.Symbols;
using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace TestProject
{
    class Program
    {
        private bool printTree = false;

        static void Main(string[] args)
        {
            var program = new Program();

            if (args.Length == 1)
            {
                program.ExecuteFile(args[0]);
                return;
            }

            while (true)
            {
                Console.Write("> ");
                var sb = new StringBuilder();
                var line = Console.ReadLine();

                if (program.HandleCommand(line))
                    continue;

                while (!string.IsNullOrWhiteSpace(line))
                {
                    sb.AppendLine(line);
                    Console.Write("| ");
                    line = Console.ReadLine();
                }

                program.ExecuteCode(sb.ToString());
            }
        }

        private void ExecuteCode(string code)
        {
            if (code.Trim().Length == 0)
                return;

            var b = new BuiltinBag();
            b.RegisterFunction(PigeonType.Int, "prompt_i", PromptI, PigeonType.String);
            b.RegisterFunction(PigeonType.Float, "prompt_f", PromptF, PigeonType.String);
            b.RegisterFunction(PigeonType.String, "prompt_s", PromptS, PigeonType.String);
            b.RegisterFunction(PigeonType.Bool, "prompt_b", PromptB, PigeonType.String);
            b.RegisterFunction(PigeonType.Void, "print", Print, PigeonType.Int);
            b.RegisterFunction(PigeonType.Void, "print", Print, PigeonType.Float);
            b.RegisterFunction(PigeonType.Void, "print", Print, PigeonType.String);
            b.RegisterFunction(PigeonType.Void, "print", Print, PigeonType.Bool);

            var interpreter = new Interpreter(code, b);
            if (printTree)
                interpreter.PrintTree(Console.Out);
            interpreter.PrintErr(Console.Out);
            if (interpreter.HasNoErrors())
                interpreter.Evaluate();
        }

        private void ExecuteFile(string file)
        {
            Console.WriteLine();
            var name = Path.GetFileNameWithoutExtension(file);
            Console.WriteLine($"### {name} ###");
            ExecuteCode(NormalizeWhitespace(File.ReadAllText(file)));
        }

        private bool HandleCommand(string line)
        {
            if (!line.StartsWith('$'))
                return false;
            var tokens = line.Split(' ');

            switch (tokens[0])
            {
                case "$cls":
                    Console.Clear();
                    break;
                case "$tree":
                    printTree = !printTree;
                    break;
                default:
                    Console.WriteLine($"Valid commands: $cls, $tree");
                    break;
            }

            return true;
        }

        private static string NormalizeWhitespace(string str)
        {
            return str.Replace("\r\n", "\n").Trim();
        }

        private object Print(object[] args)
        {
            if (args[0] is float f)
                Console.WriteLine(f.ToString(CultureInfo.InvariantCulture));
            else
                Console.WriteLine(args[0]);
            return null;
        }

        private object PromptI(object[] args)
        {
            Console.Write(args[0]);
            return int.Parse(Console.ReadLine());
        }

        private object PromptF(object[] args)
        {
            Console.Write(args[0]);
            return float.Parse(Console.ReadLine(), NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture);
        }

        private object PromptS(object[] args)
        {
            Console.Write(args[0]);
            return Console.ReadLine();
        }

        private object PromptB(object[] args)
        {
            Console.Write(args[0]);
            return bool.Parse(Console.ReadLine());
        }

    }
}
