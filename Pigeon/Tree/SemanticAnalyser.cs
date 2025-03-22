using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using Kostic017.Pigeon.Errors;
using Kostic017.Pigeon.Symbols;
using Kostic017.Pigeon.Operators;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Kostic017.Pigeon
{
    class SemanticAnalyser : PigeonBaseListener
    {
        private Scope scope;
        private readonly CodeErrorBag errorBag;

        internal GlobalScope GlobalScope { get; }
        internal ParseTreeProperty<PigeonType> Types { get; } = new ParseTreeProperty<PigeonType>();

        internal SemanticAnalyser(CodeErrorBag errorBag, GlobalScope globalScope)
        {
            this.errorBag = errorBag;
            GlobalScope = globalScope;
        }

        public override void EnterProgram([NotNull] PigeonParser.ProgramContext context)
        {
            scope = new Scope(GlobalScope);
        }

        public override void EnterFunctionDecl([NotNull] PigeonParser.FunctionDeclContext context)
        {
            scope = new Scope(GlobalScope);
            
            var argTypes = new List<PigeonType>();
            foreach (var arg in context.functionParams().TYPE())
                argTypes.Add(PigeonType.FromName(arg.GetText()));
            string signature = $"{context.ID().GetText()}({string.Join(", ", argTypes.Select(a => a.Name))})";
            
            GlobalScope.TryGetFunction(signature, out var function);

            foreach (var parameter in function.Parameters)
                scope.DeclareVariable(parameter.Type, parameter.Name, parameter.ReadOnly);
        }

        public override void ExitFunctionDecl([NotNull] PigeonParser.FunctionDeclContext context)
        {
            scope = scope.Parent;
        }

        public override void ExitFunctionCall([NotNull] PigeonParser.FunctionCallContext context)
        {
            var argTypes = new List<PigeonType>();
            foreach (var arg in context.functionArgs().expr())
                argTypes.Add(Types.Get(arg));
            string signature = $"{context.ID().GetText()}({string.Join(", ", argTypes.Select(a => a.Name))})";

            if (!GlobalScope.TryGetFunction(signature, out var function))
            {
                errorBag.ReportUndeclaredFunction(context.GetTextSpan(), signature);
                return;
            }

            var argumentCount = context.functionArgs()?.expr()?.Length ?? 0;

            if (argumentCount != function.Parameters.Length)
            {
                errorBag.ReportInvalidNumberOfArguments(context.GetTextSpan(), signature, function.Parameters.Length);
                return;
            }

            for (var i = 0; i < argumentCount; ++i)
            {
                var argument = context.functionArgs().expr(i);
                var argumentType = Types.Get(argument);
                if (function.Parameters[i].Type != PigeonType.Any && argumentType != function.Parameters[i].Type)
                    errorBag.ReportInvalidArgumentType(argument.GetTextSpan(), i + 1, function.Parameters[i].Type, argumentType);
            }
        }

        public override void EnterStmtBlock([NotNull] PigeonParser.StmtBlockContext context)
        {
            if (ShouldCreateScope(context))
                scope = new Scope(scope);
        }

        public override void ExitStmtBlock([NotNull] PigeonParser.StmtBlockContext context)
        {
            if (ShouldCreateScope(context))
                scope = scope.Parent;
        }

        public override void ExitVarDecl([NotNull] PigeonParser.VarDeclContext context)
        {
            var name = context.ID().GetText();
            var type = Types.Get(context.expr());
            var readOnly = context.keyword.Text == "const";

            if (scope.IsDeclaredHere(name))
                errorBag.ReportVariableRedeclaration(context.GetTextSpan(), name);
            else
                scope.DeclareVariable(type, name, readOnly);
        }

        public override void ExitVarAssign([NotNull] PigeonParser.VarAssignContext context)
        {
            var varName = context.varAssignLhs().ID().GetText();
            var valType = Types.Get(context.expr());

            if (scope.TryGetVariable(varName, out var variable))
            {
                if (variable.ReadOnly)
                    errorBag.ReportRedefiningReadOnlyVariable(context.GetTextSpan(), varName);
                if (!AssignmentOperator.IsAssignable(context.op.Text, variable.Type, valType))
                    errorBag.ReportInvalidTypeAssignment(context.GetTextSpan(), varName, variable.Type, valType);
            }
            else
                errorBag.ReportUndeclaredVariable(context.GetTextSpan(), varName);
        }

        public override void ExitIfStatement([NotNull] PigeonParser.IfStatementContext context)
        {
            CheckExprType(context.expr(), PigeonType.Bool);
        }

        public override void EnterForStatement([NotNull] PigeonParser.ForStatementContext context)
        {
            scope = new Scope(scope);
            scope.DeclareVariable(PigeonType.Int, context.ID().GetText(), false);
        }

        public override void ExitForStatement([NotNull] PigeonParser.ForStatementContext context)
        {
            CheckExprType(context.expr(0), PigeonType.Int);
            CheckExprType(context.expr(1), PigeonType.Int);
            scope = scope.Parent;
        }

        public override void ExitWhileStatement([NotNull] PigeonParser.WhileStatementContext context)
        {
            CheckExprType(context.expr(), PigeonType.Bool);
        }

        public override void ExitDoWhileStatement([NotNull] PigeonParser.DoWhileStatementContext context)
        {
            CheckExprType(context.expr(), PigeonType.Bool);
        }


        public override void ExitBreakStatement([NotNull] PigeonParser.BreakStatementContext context)
        {
            if (!IsInLoop(context))
                errorBag.ReportStatementNotInLoop(context.Start.GetTextSpan(), "break");
        }

        public override void ExitContinueStatement([NotNull] PigeonParser.ContinueStatementContext context)
        {
            if (!IsInLoop(context))
                errorBag.ReportStatementNotInLoop(context.Start.GetTextSpan(), "continue");
        }

        public override void ExitReturnStatement([NotNull] PigeonParser.ReturnStatementContext context)
        {
            var returnType = context.expr() != null ? Types.Get(context.expr()) : PigeonType.Void;

            RuleContext node = context;
            while (node is not PigeonParser.FunctionDeclContext)
                node = node.Parent;

            var n = ((PigeonParser.FunctionDeclContext)node);
            var functionName = n.ID().GetText();
            
            var argTypes = new List<PigeonType>();
            foreach (var arg in n.functionParams().TYPE())
                argTypes.Add(PigeonType.FromName(arg.GetText()));
            string signature = functionName + "(" + string.Join(", ", argTypes.Select(a => a.Name)) + ")";

            GlobalScope.TryGetFunction(signature, out var function);

            if (returnType != function.ReturnType)
                if (!NumberTypes(returnType, function.ReturnType))
                    errorBag.ReportUnexpectedType(context.expr().GetTextSpan(), function.ReturnType, returnType);
        }

        public override void ExitVariableExpression([NotNull] PigeonParser.VariableExpressionContext context)
        {
            var name = context.ID().GetText();
            if (scope.TryGetVariable(name, out var variable))
            {
                Types.Put(context, variable.Type);
            }
            else
            {
                Types.Put(context, PigeonType.Error);
                errorBag.ReportUndeclaredVariable(context.GetTextSpan(), name);
            }
        }

        public override void ExitListElementExpression([NotNull] PigeonParser.ListElementExpressionContext context)
        {
            var name = context.ID().GetText();
            if (scope.TryGetVariable(name, out var variable))
            {
                var type = variable.Type.Name switch
                {
                    "int[]" => PigeonType.Int,
                    "float[]" => PigeonType.Float,
                    "string[]" => PigeonType.String,
                    "bool[]" => PigeonType.Bool,
                    _ => throw new InternalErrorException($"Unsupported list type {variable.Type.Name}"),
                };
                Types.Put(context, type);
            }
            else
            {
                Types.Put(context, PigeonType.Error);
                errorBag.ReportUndeclaredVariable(context.GetTextSpan(), name);
            }
        }

        public override void ExitNumberLiteral([NotNull] PigeonParser.NumberLiteralContext context)
        {
            Types.Put(context, context.GetText().Contains('.') ? PigeonType.Float : PigeonType.Int);
        }

        public override void ExitStringLiteral([NotNull] PigeonParser.StringLiteralContext context)
        {
            Types.Put(context, PigeonType.String);
        }

        public override void ExitBoolLiteral([NotNull] PigeonParser.BoolLiteralContext context)
        {
            Types.Put(context, PigeonType.Bool);
        }

        public override void ExitEmptyIntListLiteral([NotNull] PigeonParser.EmptyIntListLiteralContext context)
        {
            Types.Put(context, PigeonType.IntList);
        }

        public override void ExitEmptyFloatListLiteral([NotNull] PigeonParser.EmptyFloatListLiteralContext context)
        {
            Types.Put(context, PigeonType.FloatList);
        }

        public override void ExitEmptyStringListLiteral([NotNull] PigeonParser.EmptyStringListLiteralContext context)
        {
            Types.Put(context, PigeonType.StringList);
        }

        public override void ExitEmptyBoolListLiteral([NotNull] PigeonParser.EmptyBoolListLiteralContext context)
        {
            Types.Put(context, PigeonType.BoolList);
        }

        public override void ExitEmptySetLiteral([NotNull] PigeonParser.EmptySetLiteralContext context)
        {
            Types.Put(context, PigeonType.Set);
        }

        public override void ExitParenthesizedExpression([NotNull] PigeonParser.ParenthesizedExpressionContext context)
        {
            Types.Put(context, Types.Get(context.expr()));
        }

        public override void ExitFunctionCallExpression([NotNull] PigeonParser.FunctionCallExpressionContext context)
        {
            var functionName = context.functionCall().ID().GetText();

            var argTypes = new List<PigeonType>();
            foreach (var arg in context.functionCall().functionArgs().expr())
                argTypes.Add(Types.Get(arg));
            string signature = functionName + "(" + string.Join(", ", argTypes.Select(a => a.Name)) + ")";

            if (GlobalScope.TryGetFunction(signature, out var function))
            {
                Types.Put(context, function.ReturnType);
            }
            else
            {
                Types.Put(context, PigeonType.Error);
                errorBag.ReportUndeclaredFunction(context.GetTextSpan(), signature);
            }
        }

        public override void ExitUnaryExpression([NotNull] PigeonParser.UnaryExpressionContext context)
        {
            var operandType = Types.Get(context.expr());
            if (!UnaryOperator.TryGetResType(context.op.Text, operandType, out var type))
                errorBag.ReportInvalidTypeUnaryOperator(context.op.GetTextSpan(), context.op.Text, type);
            Types.Put(context, type);
        }

        public override void ExitBinaryExpression([NotNull] PigeonParser.BinaryExpressionContext context)
        {
            var left = Types.Get(context.expr(0));
            var right = Types.Get(context.expr(1));
            if (!BinaryOperator.TryGetResType(context.op.Text, left, right, out var type))
                errorBag.ReportInvalidTypeBinaryOperator(context.op.GetTextSpan(), context.op.Text, left, right);
            Types.Put(context, type);
        }

        public override void ExitTernaryExpression([NotNull] PigeonParser.TernaryExpressionContext context)
        {
            CheckExprType(context.expr(0), PigeonType.Bool);
            var whenTrue = Types.Get(context.expr(1));
            var whenFalse = Types.Get(context.expr(2));
            if (!TernaryOperator.TryGetResType(whenTrue, whenFalse, out var type))
                errorBag.ReportInvalidTypeTernaryOperator(context.GetTextSpan(), whenTrue, whenFalse);
            Types.Put(context, type);
        }

        private void CheckExprType(PigeonParser.ExprContext context, PigeonType expected)
        {
            var actual = Types.Get(context);
            if (actual != expected)
                errorBag.ReportUnexpectedType(context.GetTextSpan(), expected, actual);
        }


        private static bool NumberTypes(PigeonType t1, PigeonType t2)
        {
            return (t1 == PigeonType.Int && t2 == PigeonType.Float) || (t1 == PigeonType.Float && t2 == PigeonType.Int);
        }

        private static bool ShouldCreateScope(PigeonParser.StmtBlockContext context)
        {
            if (
                context.Parent is PigeonParser.FunctionDeclContext ||
                context.Parent is PigeonParser.ForStatementContext
            ) return false;
            return true;
        }

        private static bool IsInLoop(RuleContext node)
        {
            while (node != null)
            {
                if (
                    node.Parent is PigeonParser.DoWhileStatementContext ||
                    node.Parent is PigeonParser.WhileStatementContext ||
                    node.Parent is PigeonParser.ForStatementContext
                ) return true;
                node = node.Parent;
            }
            return false;
        }
    }
}
