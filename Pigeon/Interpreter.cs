﻿using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Kostic017.Pigeon.Errors;
using Kostic017.Pigeon.Symbols;
using System;
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

        public Interpreter(string code, Builtins builtins)
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

            builtins.RegisterFunction(PigeonType.Int, "list_count", ListCount, PigeonType.List);
            builtins.RegisterFunction(PigeonType.Void, "list_add", ListAdd, PigeonType.List, PigeonType.Any);
            builtins.Register(globalScope);

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

        private static object ListCount(object[] args)
        {
            var list = (List<object>)args[0];
            return list.Count;
        }

        private static object ListAdd(object[] args)
        {
            var list = (List<object>)args[0];
            list.Add(args[1]);
            return null;
        }
    }
}
