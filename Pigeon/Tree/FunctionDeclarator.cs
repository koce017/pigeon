using Antlr4.Runtime.Misc;
using Kostic017.Pigeon.Errors;
using Kostic017.Pigeon.Symbols;
using System.Collections.Generic;
using System.Linq;

namespace Kostic017.Pigeon
{
    internal class FunctionDeclarator : PigeonBaseListener
    {
        private readonly CodeErrorBag errorBag;
        private readonly GlobalScope globalScope;

        internal FunctionDeclarator(CodeErrorBag errorBag, GlobalScope globalScope)
        {
            this.errorBag = errorBag;
            this.globalScope = globalScope;
        }

        public override void EnterFunctionDecl([NotNull] PigeonParser.FunctionDeclContext context)
        {
            var parameters = new List<Variable>();
            var returnType = PigeonType.FromName(context.TYPE().GetText());
            var parameterCount = context.functionParams()?.ID()?.Length ?? 0;

            for (var i = 0; i < parameterCount; ++i)
            {
                var parameterType = PigeonType.FromName(context.functionParams().TYPE(i).GetText());
                var parameterName = context.functionParams().ID(i).GetText();

                if (parameters.Any(v => v.Name == parameterName))
                    errorBag.ReportDuplicatedArgument(context.functionParams().GetTextSpan(), parameterName);

                parameters.Add(new Variable(parameterType, parameterName, false));
            }
            
            globalScope.DeclareFunction(returnType, context.ID().GetText(), parameters.ToArray(), context.stmtBlock());
        }
    }
}