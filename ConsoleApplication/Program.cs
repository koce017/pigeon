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
        private readonly Builtins b = new Builtins();

        private Program()
        {
            b.RegisterVariable(PigeonType.String, "author", true, "Nikola Kostic Koce");
            b.RegisterFunction(PigeonType.String, "prompt", Prompt, PigeonType.String);
            b.RegisterFunction(PigeonType.Int, "prompt_i", PromptI, PigeonType.String);
            b.RegisterFunction(PigeonType.Float, "prompt_f", PromptF, PigeonType.String);
            b.RegisterFunction(PigeonType.Bool, "prompt_b", PromptB, PigeonType.String);
            b.RegisterFunction(PigeonType.Void, "print", Print, PigeonType.Any);
        }
        
        private void ExecuteCode(string code)
        {
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
            ExecuteCode(Normalize(File.ReadAllText(file)));
        }

        private string Normalize(string str)
        {
            return str.Replace("\r\n", "\n").Trim();
        }

        private bool HandleCommand(string line)
        {
            if (!line.StartsWith("#"))
                return false;
            var tokens = line.Split(' ');

            switch (tokens[0])
            {
                case "#cls:":
                    Console.Clear();
                    break;
                case "#tree":
                    printTree = !printTree;
                    break;
                default:
                    Console.WriteLine($"Valid commands: #cls, #tree");
                    break;
            }

            return true;
        }

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

        private static object Print(object[] args)
        {
            Console.WriteLine(args[0]);
            return null;
        }

        private static object Prompt(object[] args)
        {
            Console.Write(args[0]);
            return Console.ReadLine();
        }

        private static object PromptI(object[] args)
        {
            Console.Write(args[0]);
            return int.Parse(Console.ReadLine());
        }

        private static object PromptF(object[] args)
        {
            Console.Write(args[0]);
            return float.Parse(Console.ReadLine(), NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture);
        }

        private static object PromptB(object[] args)
        {
            Console.Write(args[0]);
            return bool.Parse(Console.ReadLine());
        }
    }
}
