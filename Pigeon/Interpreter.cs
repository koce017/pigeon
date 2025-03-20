using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Kostic017.Pigeon.Errors;
using Kostic017.Pigeon.Symbols;
using System.Collections.Generic;
using System.IO;

namespace Kostic017.Pigeon
{
    public delegate object FuncPointer(params object[] arguments);

    public class Interpreter
    {
        private readonly IParseTree tree;
        private readonly PigeonParser parser;
        private readonly CodeErrorBag errorBag;
        private readonly SemanticAnalyser analyser;

        public Interpreter(string code, BuiltinBag builtinBag)
        {
            errorBag = new CodeErrorBag();
            
            var inputStream = new AntlrInputStream(code);
            var lexer = new PigeonLexer(inputStream);
            var tokenStream = new CommonTokenStream(lexer);
            parser = new PigeonParser(tokenStream);
            var errorListener = new CodeErrorListener(errorBag);
            parser.AddErrorListener(errorListener);
            tree = parser.program();

            if (!errorBag.IsEmpty())
                return;

            var walker = new ParseTreeWalker();
            var globalScope = new GlobalScope();

            // TODO: Method overloading
            builtinBag.RegisterFunction(PigeonType.Int, "list_count_i", ListCount<int>, PigeonType.IntList);
            builtinBag.RegisterFunction(PigeonType.Int, "list_count_f", ListCount<float>, PigeonType.FloatList);
            builtinBag.RegisterFunction(PigeonType.Int, "list_count_s", ListCount<string>, PigeonType.StringList);
            builtinBag.RegisterFunction(PigeonType.Int, "list_count_b", ListCount<bool>, PigeonType.BoolList);
            builtinBag.RegisterFunction(PigeonType.Void, "list_add_i", ListAdd<int>, PigeonType.IntList, PigeonType.Int);
            builtinBag.RegisterFunction(PigeonType.Void, "list_add_f", ListAdd<float>, PigeonType.FloatList, PigeonType.Float);
            builtinBag.RegisterFunction(PigeonType.Void, "list_add_s", ListAdd<string>, PigeonType.StringList, PigeonType.String);
            builtinBag.RegisterFunction(PigeonType.Void, "list_add_b", ListAdd<bool>, PigeonType.BoolList, PigeonType.Bool);
            builtinBag.Register(globalScope);

            var functionDeclarator = new FunctionDeclarator(errorBag, globalScope);
            walker.Walk(functionDeclarator, tree);

            analyser = new SemanticAnalyser(errorBag, globalScope);
            walker.Walk(analyser, tree);
        }

        public void Evaluate()
        {
            if (!errorBag.IsEmpty())
                throw new EvaluatorException("Cannot evaluate because of parsing and other errors");
            new Evaluator(analyser).Visit(tree);
        }

        public void PrintTree(TextWriter writer)
        {
            tree.PrintTree(writer, parser);
        }

        public bool HasNoErrors()
        {
            return errorBag.IsEmpty();
        }

        public void PrintErr(TextWriter writer)
        {
            foreach (var error in errorBag)
                writer.WriteLine(error.ToString());
        }

        private static object ListCount<T>(object[] args)
        {
            var list = (List<T>) args[0];
            return list.Count;
        }

        private static object ListAdd<T>(object[] args)
        {
            var list = (List<T>) args[0];
            list.Add((T) args[1]);
            return null;
        }
    }
}
