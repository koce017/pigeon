using Kostic017.Pigeon.Symbols;
using System.Collections.Generic;
using System.Linq;

namespace Kostic017.Pigeon.Operators
{
    class BinaryOperator
    {
        private readonly PigeonType leftType;
        private readonly PigeonType rightType;
        private readonly PigeonType resultType;
    
        private BinaryOperator(PigeonType left, PigeonType right, PigeonType result)
        {
            leftType = left;
            rightType = right;
            resultType = result;
        }

        private bool Supports(PigeonType left, PigeonType right)
        {
            return leftType == left && rightType == right;
        }

        internal static bool TryGetResType(string op, PigeonType leftType, PigeonType rightType, out PigeonType pigeonType)
        {
            pigeonType = PigeonType.Error;
            if (operators.TryGetValue(op, out var combinations))
            {
                BinaryOperator bop;
                if ((bop = combinations.FirstOrDefault(t => t.Supports(leftType, rightType))) != null)
                {
                    pigeonType = bop.resultType;
                    return true;
                }
            }
            return false;
        }

        private static readonly Dictionary<string, BinaryOperator[]> operators = new Dictionary<string, BinaryOperator[]>
        {
            {
                "+",
                new[]
                {
                    new BinaryOperator(PigeonType.Int, PigeonType.Int, PigeonType.Int),
                    new BinaryOperator(PigeonType.Int, PigeonType.Float, PigeonType.Float),
                    new BinaryOperator(PigeonType.Float, PigeonType.Int, PigeonType.Float),
                    new BinaryOperator(PigeonType.Float, PigeonType.Float, PigeonType.Float),
                    new BinaryOperator(PigeonType.Int, PigeonType.String, PigeonType.String),
                    new BinaryOperator(PigeonType.String, PigeonType.Int, PigeonType.String),
                    new BinaryOperator(PigeonType.Float, PigeonType.String, PigeonType.String),
                    new BinaryOperator(PigeonType.String, PigeonType.Float, PigeonType.String),
                    new BinaryOperator(PigeonType.Bool, PigeonType.String, PigeonType.String),
                    new BinaryOperator(PigeonType.String, PigeonType.Bool, PigeonType.String),
                    new BinaryOperator(PigeonType.String, PigeonType.String, PigeonType.String),
                }
            },
            {
                "-",
                new[]
                {
                    new BinaryOperator(PigeonType.Int, PigeonType.Int, PigeonType.Int),
                    new BinaryOperator(PigeonType.Int, PigeonType.Float, PigeonType.Float),
                    new BinaryOperator(PigeonType.Float, PigeonType.Int, PigeonType.Float),
                    new BinaryOperator(PigeonType.Float, PigeonType.Float, PigeonType.Float),
                }
            },
            {
                "*",
                new[]
                {
                    new BinaryOperator(PigeonType.Int, PigeonType.Int, PigeonType.Int),
                    new BinaryOperator(PigeonType.Int, PigeonType.Float, PigeonType.Float),
                    new BinaryOperator(PigeonType.Float, PigeonType.Int, PigeonType.Float),
                    new BinaryOperator(PigeonType.Float, PigeonType.Float, PigeonType.Float),
                }
            },
            {
                "/",
                new []
                {
                    new BinaryOperator(PigeonType.Int, PigeonType.Int, PigeonType.Int),
                    new BinaryOperator(PigeonType.Int, PigeonType.Float, PigeonType.Float),
                    new BinaryOperator(PigeonType.Float, PigeonType.Int, PigeonType.Float),
                    new BinaryOperator(PigeonType.Float, PigeonType.Float, PigeonType.Float),
                }
            },
            {
                "%",
                new []
                {
                    new BinaryOperator(PigeonType.Int, PigeonType.Int, PigeonType.Int),
                }
            },
            {
                "&&",
                new []
                {
                    new BinaryOperator(PigeonType.Bool, PigeonType.Bool, PigeonType.Bool),
                }
            },
            {
                "||",
                new []
                {
                    new BinaryOperator(PigeonType.Bool, PigeonType.Bool, PigeonType.Bool),
                }
            },
            {
                "<",
                new []
                {
                    new BinaryOperator(PigeonType.Int, PigeonType.Int, PigeonType.Bool),
                    new BinaryOperator(PigeonType.Int, PigeonType.Float, PigeonType.Bool),
                    new BinaryOperator(PigeonType.Float, PigeonType.Int, PigeonType.Bool),
                    new BinaryOperator(PigeonType.Float, PigeonType.Float, PigeonType.Bool),
                }
            },
            {
                ">",
                new []
                {
                    new BinaryOperator(PigeonType.Int, PigeonType.Int, PigeonType.Bool),
                    new BinaryOperator(PigeonType.Int, PigeonType.Float, PigeonType.Bool),
                    new BinaryOperator(PigeonType.Float, PigeonType.Int, PigeonType.Bool),
                    new BinaryOperator(PigeonType.Float, PigeonType.Float, PigeonType.Bool),
                }
            },
            {
                "<=",
                new []
                {
                    new BinaryOperator(PigeonType.Int, PigeonType.Int, PigeonType.Bool),
                    new BinaryOperator(PigeonType.Int, PigeonType.Float, PigeonType.Bool),
                    new BinaryOperator(PigeonType.Float, PigeonType.Int, PigeonType.Bool),
                    new BinaryOperator(PigeonType.Float, PigeonType.Float, PigeonType.Bool),
                }
            },
            {
                ">=",
                new []
                {
                    new BinaryOperator(PigeonType.Int, PigeonType.Int, PigeonType.Bool),
                    new BinaryOperator(PigeonType.Int, PigeonType.Float, PigeonType.Bool),
                    new BinaryOperator(PigeonType.Float, PigeonType.Int, PigeonType.Bool),
                    new BinaryOperator(PigeonType.Float, PigeonType.Float, PigeonType.Bool),
                }
            },
            {
                "==",
                new []
                {
                    new BinaryOperator(PigeonType.Int, PigeonType.Int, PigeonType.Bool),
                    new BinaryOperator(PigeonType.Int, PigeonType.Float, PigeonType.Bool),
                    new BinaryOperator(PigeonType.Float, PigeonType.Int, PigeonType.Bool),
                    new BinaryOperator(PigeonType.Float, PigeonType.Float, PigeonType.Bool),
                    new BinaryOperator(PigeonType.Bool, PigeonType.Bool, PigeonType.Bool),
                    new BinaryOperator(PigeonType.String, PigeonType.String, PigeonType.Bool),
                }
            },
            {
                "!=",
                new []
                {
                    new BinaryOperator(PigeonType.Int, PigeonType.Int, PigeonType.Bool),
                    new BinaryOperator(PigeonType.Int, PigeonType.Float, PigeonType.Bool),
                    new BinaryOperator(PigeonType.Float, PigeonType.Int, PigeonType.Bool),
                    new BinaryOperator(PigeonType.Float, PigeonType.Float, PigeonType.Bool),
                    new BinaryOperator(PigeonType.Bool, PigeonType.Bool, PigeonType.Bool),
                    new BinaryOperator(PigeonType.String, PigeonType.String, PigeonType.Bool),
                }
            },
        };
    }
}
