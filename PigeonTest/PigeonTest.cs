using Kostic017.Pigeon.Symbols;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Xunit;

namespace Kostic017.Pigeon.Tests
{
    public class PigeonTest
    {
        private static readonly string SamplesFolder = "../../../Samples";
        private static readonly string TestCasesFolder = "../../../TestCases";

        private StringWriter outputStream;
        private Queue<string> inputStream;

        [Theory]
        [MemberData(nameof(TestCases))]
        public void Test(string sample)
        {
            var inFile = Path.Combine(TestCasesFolder, sample + ".in.txt");
            var code = NormalizeWhitespace(File.ReadAllText(Path.Combine(SamplesFolder, sample + ".pig")));
            var outputs = ReadCases(Path.Combine(TestCasesFolder, sample + ".out.txt"));

            if (File.Exists(inFile))
            {
                var inputs = ReadCases(inFile);
                for (var i = 0; i < inputs.Length; ++i)
                {
                        
                    inputStream = new Queue<string>();
                    outputStream = new StringWriter();
                        
                    foreach (var input in inputs[i].Split('\n'))
                        inputStream.Enqueue(input);

                    Execute(code);

                    Assert.Equal(outputs[i], ActualOutput());
                    
                }
            }
            else
            {
                outputStream = new StringWriter();
                Execute(code);
                Assert.Equal(outputs[0], ActualOutput());
            }
        }

        public static IEnumerable<object[]> TestCases()
        {
            foreach (var outFilePath in Directory.GetFiles(TestCasesFolder, "*.out.txt"))
            {
                string fileName = Path.GetFileName(outFilePath);
                yield return new string[] { fileName.Substring(0, fileName.IndexOf('.')) };
            }

        }

        private string ActualOutput()
        {
            return NormalizeWhitespace(outputStream.ToString());
        }

        private static string[] ReadCases(string file)
        {
            return NormalizeWhitespace(File.ReadAllText(file)).Split("---").Select(v => v.Trim()).ToArray();
        }

        private static string NormalizeWhitespace(string str)
        {
            return str.Replace("\r\n", "\n").Trim();
        }

        private void Execute(string code)
        {
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
            interpreter.PrintErr(outputStream);
            interpreter.Evaluate();
        }

        private object Print(object[] args)
        {
            if (args[0] is float f)
                outputStream.WriteLine(f.ToString(CultureInfo.InvariantCulture));
            else
                outputStream.WriteLine(args[0]);
            return null;
        }

        private object PromptI(object[] args)
        {
            return int.Parse(inputStream.Dequeue());
        }

        private object PromptF(object[] args)
        {
            return float.Parse(inputStream.Dequeue(), NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture);
        }

        private object PromptS(object[] args)
        {
            return inputStream.Dequeue();
        }

        private object PromptB(object[] args)
        {
            return bool.Parse(inputStream.Dequeue());
        }

    }
}
