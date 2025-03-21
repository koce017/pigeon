﻿using Antlr4.Runtime.Misc;
using Kostic017.Pigeon.Errors;
using Kostic017.Pigeon.Symbols;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Kostic017.Pigeon
{
    class BreakLoopException : Exception { }
    class ContinueLoopException : Exception { }
    
    class FuncReturnValueException : Exception
    {
        internal object Value { get; }

        internal FuncReturnValueException(object value)
        {
            Value = value;
        }
    }

    class Evaluator : PigeonBaseVisitor<object>
    {
        private readonly SemanticAnalyser analyser;
        private readonly Stack<FunctionScope> functionScopes = new();

        internal Evaluator(SemanticAnalyser analyser)
        {
            this.analyser = analyser;
        }

        public override object VisitProgram([NotNull] PigeonParser.ProgramContext context)
        {
            functionScopes.Push(new FunctionScope(analyser.GlobalScope));

            foreach (var stmt in context.stmt())
                Visit(stmt);
            
            return null;
        }

        public override object VisitStmtBlock([NotNull] PigeonParser.StmtBlockContext context)
        {
            if (ShouldCreateScope(context))
                functionScopes.Peek().EnterScope();
            try
            {
                foreach (var statement in context.stmt())
                {
                    var r = Visit(statement);
                    if (statement is PigeonParser.ContinueStatementContext)
                        throw new ContinueLoopException();
                    if (statement is PigeonParser.BreakStatementContext)
                        throw new BreakLoopException();
                    if (statement is PigeonParser.ReturnStatementContext)
                        throw new FuncReturnValueException(r);
                }
            }
            finally
            {
                if (ShouldCreateScope(context))
                    functionScopes.Peek().ExitScope();
            }
            return null;
        }


        public override object VisitVariableDeclarationStatement([NotNull] PigeonParser.VariableDeclarationStatementContext context)
        {
            var name = context.varDecl().ID().GetText();
            var type = analyser.Types.Get(context.varDecl().expr());
            var value = Visit(context.varDecl().expr());
            Declare(type, name, value);
            return null;
        }

        public override object VisitVariableAssignmentStatement([NotNull] PigeonParser.VariableAssignmentStatementContext context)
        {
            return Visit(context.varAssign());
        }

        public override object VisitVarAssign([NotNull] PigeonParser.VarAssignContext context)
        {
            var name = context.varAssignLhs().ID().GetText();
            var value = Visit(context.expr());
            var type = analyser.Types.Get(context.expr());

            if (context.varAssignLhs().index == null)
            {
                var currentValue = functionScopes.Peek().Evaluate(name);

                switch (context.op.Text)
                {
                    case "=":
                        Assign(name, value);
                        break;

                    case "+=":
                        if (type == PigeonType.Int)
                            Assign(name, (int)currentValue + (int)value);
                        else if (type == PigeonType.Float)
                            Assign(name, (float)currentValue + (float)value);
                        else
                            Assign(name, (string)currentValue + (string)value);
                        break;

                    case "-=":
                        if (type == PigeonType.Int)
                            Assign(name, (int)currentValue - (int)value);
                        else if (type == PigeonType.Float)
                            Assign(name, (float)currentValue - (float)value);
                        break;

                    case "*=":
                        if (type == PigeonType.Int)
                            Assign(name, (int)currentValue * (int)value);
                        else if (type == PigeonType.Float)
                            Assign(name, (float)currentValue * (float)value);
                        break;

                    case "/=":
                        if (type == PigeonType.Int)
                            Assign(name, (int)currentValue / (int)value);
                        else if (type == PigeonType.Float)
                            Assign(name, (float)currentValue / (float)value);
                        break;

                    case "%=":
                        Assign(name, (int)currentValue % (int)value);
                        break;
                }
            }
            else
            {
                var list = (List<object>)functionScopes.Peek().Evaluate(name);
                int index = (int)Visit(context.varAssignLhs().expr());
                CheckBounds(index, list, context.GetTextSpan().Line);

                switch (context.op.Text)
                {
                    case "=":
                        list[index] = value;
                        break;
                    case "+=":
                        if (type == PigeonType.Int)
                            list[index] = (int)list[index] + (int)value;
                        else if (type == PigeonType.Float)
                            list[index] = (float)list[index] + (float)value;
                        else
                            list[index] = (string)list[index] + (string)value;
                        break;
                    case "-=":
                        if (type == PigeonType.Int)
                            list[index] = (int)list[index] - (int)value;
                        else if (type == PigeonType.Float)
                            list[index] = (float)list[index] - (float)value;
                        break;
                    case "*=":
                        if (type == PigeonType.Int)
                            list[index] = (int)list[index] * (int)value;
                        else if (type == PigeonType.Float)
                            list[index] = (float)list[index] * (float)value;
                        break;
                    case "/=":
                        if (type == PigeonType.Int)
                            list[index] = (int)list[index] / (int)value;
                        else if (type == PigeonType.Float)
                            list[index] = (float)list[index] / (float)value;
                        break;
                    case "%=":
                        list[index] = (int)list[index] % (int)value;
                        break;
                }

            }

            return null;
        }

        public override object VisitFunctionCallStatement([NotNull] PigeonParser.FunctionCallStatementContext context)
        {
            return Visit(context.functionCall());
        }

        public override object VisitIfStatement([NotNull] PigeonParser.IfStatementContext context)
        {
            if ((bool)Visit(context.expr()))
                Visit(context.stmtBlock(0));
            else if (context.stmtBlock(1) != null)
                Visit(context.stmtBlock(1));
            return null;
        }

        public override object VisitForStatement([NotNull] PigeonParser.ForStatementContext context)
        {
            var startValue = (int)Visit(context.expr(0));
            var targetValue = (int)Visit(context.expr(1));
            var counter = context.ID().GetText();
            var isIncrementing = context.dir.Text == "to";
            var i = startValue;

            functionScopes.Peek().EnterScope();
            Declare(PigeonType.Int, counter, i);

            while (isIncrementing ? i <= targetValue : i >= targetValue)
            {
                try
                {
                    Visit(context.stmtBlock());
                    functionScopes.Peek().ExitScope();
                    functionScopes.Peek().EnterScope();
                    Declare(PigeonType.Int, counter, i);
                }
                catch (BreakLoopException)
                {
                    return null;
                }
                catch (ContinueLoopException)
                {
                }
                i += isIncrementing ? 1 : -1;
                Assign(counter, i);
                targetValue = (int)Visit(context.expr(1));
            }
            return null;
        }

        public override object VisitWhileStatement([NotNull] PigeonParser.WhileStatementContext context)
        {
            while ((bool)Visit(context.expr()))
                try
                {
                    Visit(context.stmtBlock());
                }
                catch (BreakLoopException)
                {
                    return null;
                }
                catch (ContinueLoopException)
                {
                }
            return null;
        }

        public override object VisitDoWhileStatement([NotNull] PigeonParser.DoWhileStatementContext context)
        {
            do
                try
                {
                    Visit(context.stmtBlock());
                }
                catch (BreakLoopException)
                {
                    return null;
                }
                catch (ContinueLoopException)
                {
                }
            while ((bool)Visit(context.expr()));
            return null;
        }

        public override object VisitReturnStatement([NotNull] PigeonParser.ReturnStatementContext context)
        {
            return context.expr() != null ? Visit(context.expr()) : null;
        }

        public override object VisitVariableExpression([NotNull] PigeonParser.VariableExpressionContext context)
        {
            return functionScopes.Peek().Evaluate(context.ID().GetText());
        }

        public override object VisitListElementExpression([NotNull] PigeonParser.ListElementExpressionContext context)
        {
            int index = (int)Visit(context.expr());
            var list = (List<object>)functionScopes.Peek().Evaluate(context.ID().GetText());
            CheckBounds(index, list, context.GetTextSpan().Line);
            return list[index];
        }

        public override object VisitNumberLiteral([NotNull] PigeonParser.NumberLiteralContext context)
        {
            if (analyser.Types.Get(context) == PigeonType.Int)
                return int.Parse(context.NUMBER().GetText());
            return float.Parse(context.NUMBER().GetText(), NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture);
        }

        public override object VisitStringLiteral([NotNull] PigeonParser.StringLiteralContext context)
        {
            return context.STRING().GetText().Trim('"');
        }

        public override object VisitBoolLiteral([NotNull] PigeonParser.BoolLiteralContext context)
        {
            return bool.Parse(context.BOOL().GetText());
        }

        public override object VisitEmptyIntListLiteral([NotNull] PigeonParser.EmptyIntListLiteralContext context)
        {
            return new List<object>();
        }

        public override object VisitEmptyFloatListLiteral([NotNull] PigeonParser.EmptyFloatListLiteralContext context)
        {
            return new List<object>();
        }

        public override object VisitEmptyStringListLiteral([NotNull] PigeonParser.EmptyStringListLiteralContext context)
        {
            return new List<object>();
        }

        public override object VisitEmptyBoolListLiteral([NotNull] PigeonParser.EmptyBoolListLiteralContext context)
        {
            return new List<object>();
        }

        public override object VisitEmptySetLiteral([NotNull] PigeonParser.EmptySetLiteralContext context)
        {
            return new HashSet<object>();
        }

        public override object VisitParenthesizedExpression([NotNull] PigeonParser.ParenthesizedExpressionContext context)
        {
            return Visit(context.expr());
        }

        public override object VisitFunctionCallExpression([NotNull] PigeonParser.FunctionCallExpressionContext context)
        {
            return Visit(context.functionCall());
        }

        public override object VisitUnaryExpression([NotNull] PigeonParser.UnaryExpressionContext context)
        {
            var operand = Visit(context.expr());
            var resType = analyser.Types.Get(context);
            switch (context.op.Text)
            {
                case "+":
                    if (resType == PigeonType.Int)
                        return (int)operand;
                    return (float)operand;
                case "-":
                    if (resType == PigeonType.Int)
                        return -(int)operand;
                    return -(float)operand;
                case "!":
                    return !(bool)operand;
                default:
                    throw new InternalErrorException($"Unsupported unary operator {context.op.Text}");
            }
        }
        public override object VisitBinaryExpression([NotNull] PigeonParser.BinaryExpressionContext context)
        {
            var left = Visit(context.expr(0));
            var right = Visit(context.expr(1));
            var resType = analyser.Types.Get(context);

            var areBothInt =
                analyser.Types.Get(context.expr(0)) == PigeonType.Int &&
                analyser.Types.Get(context.expr(1)) == PigeonType.Int;
            
            switch (context.op.Text)
            {
                case "==":
                    return left.Equals(right);

                case "!=":
                    return !left.Equals(right);

                case "&&":
                    return (bool)left && (bool)right;

                case "||":
                    return (bool)left || (bool)right;

                case "<":
                    if (areBothInt)
                        return (int)left < (int)right;
                    return Convert.ToSingle(left) < Convert.ToSingle(right);

                case ">":
                    if (areBothInt)
                        return (int)left > (int)right;
                    return Convert.ToSingle(left) > Convert.ToSingle(right);

                case "<=":
                    if (areBothInt)
                        return (int)left <= (int)right;
                    return Convert.ToSingle(left) <= Convert.ToSingle(right);

                case ">=":
                    if (areBothInt)
                        return (int)left >= (int)right;
                    return Convert.ToSingle(left) >= Convert.ToSingle(right);

                case "+":
                    if (resType == PigeonType.Int)
                        return (int)left + (int)right;
                    else if (resType == PigeonType.Float)
                        return Convert.ToSingle(left) + Convert.ToSingle(right);
                    else
                        return left.ToString() + right.ToString();

                case "-":
                    if (resType == PigeonType.Int)
                        return (int)left - (int)right;
                    return Convert.ToSingle(left) - Convert.ToSingle(right);

                case "*":
                    if (resType == PigeonType.Int)
                        return (int)left * (int)right;
                    return Convert.ToSingle(left) * Convert.ToSingle(right);

                case "/":
                    if (resType == PigeonType.Int)
                        return (int)left / (int)right;
                    return Convert.ToSingle(left) / Convert.ToSingle(right);

                case "%":
                    return (int)left % (int)right;

                default:
                    throw new InternalErrorException($"Unsupported binary operator {context.op.Text} at {context.GetTextSpan().Line}");
            }
        }

        public override object VisitFunctionCall([NotNull] PigeonParser.FunctionCallContext context)
        {
            
            var argValues = new List<object>();
            var argTypes = new List<PigeonType>();

            if (context.functionArgs() != null)
            {
                foreach (var arg in context.functionArgs().expr())
                    argValues.Add(Visit(arg));
                foreach (var arg in context.functionArgs().expr())
                    argTypes.Add(analyser.Types.Get(arg));
            }

            string signature = context.ID().GetText() + "(" + string.Join(", ", argTypes.Select(a => a.Name)) + ")";
            analyser.GlobalScope.TryGetFunction(signature, out var function);

            if (function.FuncBody is FuncPointer fp)
                return fp(argValues.ToArray());

            var funcBody = (PigeonParser.StmtBlockContext)function.FuncBody;
            functionScopes.Push(new FunctionScope(analyser.GlobalScope));

            for (var i = 0; i < argValues.Count; ++i)
                Declare(function.Parameters[i].Type, function.Parameters[i].Name, argValues[i]);

            try
            {
                return Visit(funcBody);
            }
            catch (FuncReturnValueException e)
            {
                return e.Value;
            }
            finally
            {
                functionScopes.Pop();
            }
        }

        public override object VisitTernaryExpression([NotNull] PigeonParser.TernaryExpressionContext context)
        {
            var condition = Visit(context.expr(0));
            var whenTrue = Visit(context.expr(1));
            var whenFalse = Visit(context.expr(2));
            return (bool) condition ? whenTrue : whenFalse;
        }

        private void Declare(PigeonType type, string name, object value)
        {
            functionScopes.Peek().Declare(type, name, value);
        }

        private void Assign(string name, object value)
        {
            functionScopes.Peek().Assign(name, value);
        }

        private static void CheckBounds(int index, List<object> list, int line)
        {
            if (index < 0 || index >= list.Count)
                throw new EvaluatorException($"Index {index} out of bounds for length {list.Count} at line {line}");
        }

        private static bool ShouldCreateScope(PigeonParser.StmtBlockContext context)
        {
            return context.Parent is not PigeonParser.ForStatementContext;
        }
        
    }
}
