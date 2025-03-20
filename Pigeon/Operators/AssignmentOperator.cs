using Kostic017.Pigeon.Symbols;
using System.Collections.Generic;
using System.Linq;

namespace Kostic017.Pigeon.Operators
{
    class AssignmentOperator
    {
        private readonly PigeonType variableType;
        private readonly PigeonType valueType;

        private AssignmentOperator(PigeonType variableType, PigeonType valueType)
        {
            this.variableType = variableType;
            this.valueType = valueType;
        }

        private bool Supports(PigeonType variableType, PigeonType valueType)
        {
            return this.variableType == variableType && this.valueType == valueType;
        }

        internal static bool IsAssignable(string op, PigeonType variableType, PigeonType valueType)
        {
            if (operators.TryGetValue(op, out var combinations))
                return combinations.Any(t => t.Supports(variableType, valueType));
            return false;
        }

        private static readonly Dictionary<string, AssignmentOperator[]> operators = new()
        {
                {
                    "=",
                    new[]
                    {
                        new AssignmentOperator(PigeonType.Int, PigeonType.Int),
                        new AssignmentOperator(PigeonType.Int, PigeonType.Float),
                        new AssignmentOperator(PigeonType.Float, PigeonType.Int),
                        new AssignmentOperator(PigeonType.Float, PigeonType.Float),
                        new AssignmentOperator(PigeonType.String, PigeonType.String),
                        new AssignmentOperator(PigeonType.Bool, PigeonType.Bool),
                        new AssignmentOperator(PigeonType.List, PigeonType.Int),
                        new AssignmentOperator(PigeonType.List, PigeonType.Float),
                        new AssignmentOperator(PigeonType.List, PigeonType.String),
                        new AssignmentOperator(PigeonType.List, PigeonType.Bool),
                    }
                },
                {
                    "+=",
                    new[]
                    {
                        new AssignmentOperator(PigeonType.Int, PigeonType.Int),
                        new AssignmentOperator(PigeonType.Int, PigeonType.Float),
                        new AssignmentOperator(PigeonType.Float, PigeonType.Int),
                        new AssignmentOperator(PigeonType.Float, PigeonType.Float),
                        new AssignmentOperator(PigeonType.String, PigeonType.Int),
                        new AssignmentOperator(PigeonType.String, PigeonType.Float),
                        new AssignmentOperator(PigeonType.String, PigeonType.Bool),
                        new AssignmentOperator(PigeonType.String, PigeonType.String),
                    }
                },
                {
                    "-=",
                    new[]
                    {
                        new AssignmentOperator(PigeonType.Int, PigeonType.Int),
                        new AssignmentOperator(PigeonType.Int, PigeonType.Float),
                        new AssignmentOperator(PigeonType.Float, PigeonType.Int),
                        new AssignmentOperator(PigeonType.Float, PigeonType.Float),
                    }
                },
                {
                    "*=",
                    new[]
                    {
                        new AssignmentOperator(PigeonType.Int, PigeonType.Int),
                        new AssignmentOperator(PigeonType.Int, PigeonType.Float),
                        new AssignmentOperator(PigeonType.Float, PigeonType.Int),
                        new AssignmentOperator(PigeonType.Float, PigeonType.Float),
                    }
                },
                {
                    "/=",
                    new[]
                    {
                        new AssignmentOperator(PigeonType.Int, PigeonType.Int),
                        new AssignmentOperator(PigeonType.Int, PigeonType.Float),
                        new AssignmentOperator(PigeonType.Float, PigeonType.Int),
                        new AssignmentOperator(PigeonType.Float, PigeonType.Float),
                    }
                },
                {
                    "%=",
                    new[]
                    {
                        new AssignmentOperator(PigeonType.Int, PigeonType.Int),
                    }
                },

            };
    }
}
