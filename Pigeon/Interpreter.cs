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

            builtinBag.RegisterVariable(PigeonType.String, "author", true, "Nikola Kostic Koce");

            builtinBag.RegisterFunction(PigeonType.Int, "ftoi", FloatToInt, PigeonType.Float);

            builtinBag.RegisterFunction(PigeonType.Int, "len", StringLen, PigeonType.String);
            builtinBag.RegisterFunction(PigeonType.String, "get_char_at", StringGetCharAt, PigeonType.String, PigeonType.Int);
            builtinBag.RegisterFunction(PigeonType.String, "set_char_at", StringSetCharAt, PigeonType.String, PigeonType.Int, PigeonType.String);

            builtinBag.RegisterFunction(PigeonType.Int, "len", ListCount, PigeonType.IntList);
            builtinBag.RegisterFunction(PigeonType.Int, "len", ListCount, PigeonType.FloatList);
            builtinBag.RegisterFunction(PigeonType.Int, "len", ListCount, PigeonType.StringList);
            builtinBag.RegisterFunction(PigeonType.Int, "len", ListCount, PigeonType.BoolList);
            builtinBag.RegisterFunction(PigeonType.Void, "sort", ListSort, PigeonType.IntList, PigeonType.String);
            builtinBag.RegisterFunction(PigeonType.Void, "sort", ListSort, PigeonType.FloatList, PigeonType.String);
            builtinBag.RegisterFunction(PigeonType.Void, "sort", ListSort, PigeonType.BoolList, PigeonType.String);
            builtinBag.RegisterFunction(PigeonType.Void, "sort", ListSort, PigeonType.StringList, PigeonType.String);
            builtinBag.RegisterFunction(PigeonType.Void, "list_add", ListAdd, PigeonType.IntList, PigeonType.Int);
            builtinBag.RegisterFunction(PigeonType.Void, "list_add", ListAdd, PigeonType.FloatList, PigeonType.Float);
            builtinBag.RegisterFunction(PigeonType.Void, "list_add", ListAdd, PigeonType.StringList, PigeonType.String);
            builtinBag.RegisterFunction(PigeonType.Void, "list_add", ListAdd, PigeonType.BoolList, PigeonType.Bool);

            builtinBag.RegisterFunction(PigeonType.Bool, "set_in", SetIn, PigeonType.Set, PigeonType.Int);
            builtinBag.RegisterFunction(PigeonType.Bool, "set_in", SetIn, PigeonType.Set, PigeonType.Float);
            builtinBag.RegisterFunction(PigeonType.Bool, "set_in", SetIn, PigeonType.Set, PigeonType.String);
            builtinBag.RegisterFunction(PigeonType.Bool, "set_in", SetIn, PigeonType.Set, PigeonType.Bool);
            builtinBag.RegisterFunction(PigeonType.Void, "set_add", SetAdd, PigeonType.Set, PigeonType.Int);
            builtinBag.RegisterFunction(PigeonType.Void, "set_add", SetAdd, PigeonType.Set, PigeonType.Float);
            builtinBag.RegisterFunction(PigeonType.Void, "set_add", SetAdd, PigeonType.Set, PigeonType.String);
            builtinBag.RegisterFunction(PigeonType.Void, "set_add", SetAdd, PigeonType.Set, PigeonType.Bool);
            builtinBag.RegisterFunction(PigeonType.Void, "set_remove", SetRemove, PigeonType.Set, PigeonType.Int);
            builtinBag.RegisterFunction(PigeonType.Void, "set_remove", SetRemove, PigeonType.Set, PigeonType.Float);
            builtinBag.RegisterFunction(PigeonType.Void, "set_remove", SetRemove, PigeonType.Set, PigeonType.String);
            builtinBag.RegisterFunction(PigeonType.Void, "set_remove", SetRemove, PigeonType.Set, PigeonType.Bool);

            builtinBag.PopulateGlobalScope(globalScope);

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

        private static object FloatToInt(object[] args)
        {
            return (int)(float)args[0];
        }

        private static object StringLen(object[] args)
        {
            var str = (string)args[0];
            return str.Length;
        }

        private static object StringGetCharAt(object[] args)
        {
            var str = (string)args[0];
            var index = (int)args[1];
            return str[index].ToString();
        }

        private static object StringSetCharAt(object[] args)
        {
            var str = (string)args[0];
            var index = (int)args[1];
            var ch = (string)args[2];
            return str.Substring(0, index) + ch + str.Substring(index + 1);
        }

        private static object ListCount(object[] args)
        {
            var list = (List<object>) args[0];
            return list.Count;
        }

        private static object ListAdd(object[] args)
        {
            var list = (List<object>) args[0];
            list.Add(args[1]);
            return null;
        }

        private static object ListSort(object[] args)
        {
            var list = (List<object>) args[0];
            var order = (string) args[1];
            list.Sort();
            if (order == "desc")
                list.Reverse();
            return null;
        }

        private static object SetIn(object[] args)
        {
            var set = (HashSet<object>)args[0];
            return set.Contains(args[1]);
        }

        private static object SetAdd(object[] args)
        {
            var set = (HashSet<object>)args[0];
            set.Add(args[1]);
            return null;
        }

        private static object SetRemove(object[] args)
        {
            var set = (HashSet<object>)args[0];
            set.Remove(args[1]);
            return null;
        }
    }
}
