﻿using Kostic017.Pigeon.Symbols;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Xunit;

namespace Kostic017.Pigeon.Tests
{
    public class PigeonTest
    {
        private static readonly string SamplesFolder = "../../../../Samples";
        private static readonly string TestsFolder = "../../../Tests";

        private TextWriter outputStream;
        private Queue<string> inputStream;

        [Theory]
        [MemberData(nameof(TestCases))]
        public void Test(string sample)
        {
            var inFile = Path.Combine(TestsFolder, sample + ".in");
            var code = NormalizeWhitespace(File.ReadAllText(Path.Combine(SamplesFolder, sample + ".pig")));
            var outputs = ReadCases(Path.Combine(TestsFolder, sample + ".out"));

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
            foreach (var sample in Directory.GetFiles(TestsFolder, "*.out"))
                yield return new string[] { Path.GetFileNameWithoutExtension(sample) };
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
            b.RegisterFunction(PigeonType.String, "prompt", Prompt, PigeonType.String);
            b.RegisterFunction(PigeonType.Int, "prompt_i", PromptI, PigeonType.String);
            b.RegisterFunction(PigeonType.Float, "prompt_f", PromptF, PigeonType.String);
            b.RegisterFunction(PigeonType.Bool, "prompt_b", PromptB, PigeonType.String);
            b.RegisterFunction(PigeonType.Void, "print", Print, PigeonType.Any);

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

        private object Prompt(object[] args)
        {
            return inputStream.Dequeue();
        }

        private object PromptI(object[] args)
        {
            return int.Parse(inputStream.Dequeue());
        }

        private object PromptF(object[] args)
        {
            return float.Parse(inputStream.Dequeue(), NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture);
        }

        private object PromptB(object[] args)
        {
            return bool.Parse(inputStream.Dequeue());
        }
    }
}
