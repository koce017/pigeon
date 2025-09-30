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

        private static readonly Dictionary<string, AssignmentOperator[]> operators = new Dictionary<string, AssignmentOperator[]>()
        {
                {
                    "=",
                    new[]
                    {
                        new AssignmentOperator(PigeonType.Int, PigeonType.Int),
                        //new AssignmentOperator(PigeonType.Int, PigeonType.Float), // cannot implicitly convert type 'float' to 'int'
                        new AssignmentOperator(PigeonType.Float, PigeonType.Int),
                        new AssignmentOperator(PigeonType.Float, PigeonType.Float),
                        new AssignmentOperator(PigeonType.String, PigeonType.String),
                        new AssignmentOperator(PigeonType.Bool, PigeonType.Bool),
                        new AssignmentOperator(PigeonType.IntList, PigeonType.Int),
                        //new AssignmentOperator(PigeonType.IntList, PigeonType.Float),
                        new AssignmentOperator(PigeonType.FloatList, PigeonType.Float),
                        new AssignmentOperator(PigeonType.FloatList, PigeonType.Int),
                        new AssignmentOperator(PigeonType.StringList, PigeonType.String),
                        new AssignmentOperator(PigeonType.BoolList, PigeonType.Bool),
                    }
                },
                {
                    "+=",
                    new[]
                    {
                        new AssignmentOperator(PigeonType.Int, PigeonType.Int),
                        //new AssignmentOperator(PigeonType.Int, PigeonType.Float),
                        new AssignmentOperator(PigeonType.Float, PigeonType.Int),
                        new AssignmentOperator(PigeonType.Float, PigeonType.Float),
                        new AssignmentOperator(PigeonType.String, PigeonType.Int),
                        new AssignmentOperator(PigeonType.String, PigeonType.Float),
                        new AssignmentOperator(PigeonType.String, PigeonType.Bool),
                        new AssignmentOperator(PigeonType.String, PigeonType.String),
                        new AssignmentOperator(PigeonType.IntList, PigeonType.Int),
                        //new AssignmentOperator(PigeonType.IntList, PigeonType.Float),
                        new AssignmentOperator(PigeonType.FloatList, PigeonType.Float),
                        new AssignmentOperator(PigeonType.FloatList, PigeonType.Int),
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
                        new AssignmentOperator(PigeonType.IntList, PigeonType.Int),
                        //new AssignmentOperator(PigeonType.IntList, PigeonType.Float),
                        new AssignmentOperator(PigeonType.FloatList, PigeonType.Float),
                        new AssignmentOperator(PigeonType.FloatList, PigeonType.Int),
                    }
                },
                {
                    "*=",
                    new[]
                    {
                        new AssignmentOperator(PigeonType.Int, PigeonType.Int),
                        //new AssignmentOperator(PigeonType.Int, PigeonType.Float),
                        new AssignmentOperator(PigeonType.Float, PigeonType.Int),
                        new AssignmentOperator(PigeonType.Float, PigeonType.Float),
                        new AssignmentOperator(PigeonType.IntList, PigeonType.Int),
                        //new AssignmentOperator(PigeonType.IntList, PigeonType.Float),
                        new AssignmentOperator(PigeonType.FloatList, PigeonType.Float),
                        new AssignmentOperator(PigeonType.FloatList, PigeonType.Int),
                    }
                },
                {
                    "/=",
                    new[]
                    {
                        new AssignmentOperator(PigeonType.Int, PigeonType.Int),
                        //new AssignmentOperator(PigeonType.Int, PigeonType.Float),
                        new AssignmentOperator(PigeonType.Float, PigeonType.Int),
                        new AssignmentOperator(PigeonType.Float, PigeonType.Float),
                        new AssignmentOperator(PigeonType.IntList, PigeonType.Int),
                        //new AssignmentOperator(PigeonType.IntList, PigeonType.Float),
                        new AssignmentOperator(PigeonType.FloatList, PigeonType.Float),
                        new AssignmentOperator(PigeonType.FloatList, PigeonType.Int),
                    }
                },
                {
                    "%=",
                    new[]
                    {
                        new AssignmentOperator(PigeonType.Int, PigeonType.Int),
                        new AssignmentOperator(PigeonType.IntList, PigeonType.Int),
                    }
                },

            };
    }
}
