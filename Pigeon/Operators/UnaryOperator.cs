using Kostic017.Pigeon.Symbols;
using System.Collections.Generic;
using System.Linq;

namespace Kostic017.Pigeon.Operators
{
    class UnaryOperator
    {
        private readonly PigeonType resultType;

        private UnaryOperator(PigeonType type)
        {
            resultType = type;
        }

        private bool Supports(PigeonType type)
        {
            return resultType == type;
        }

        internal static bool TryGetResType(string op, PigeonType operandType, out PigeonType pigeonType)
        {
            pigeonType = PigeonType.Error;
            if (operators.TryGetValue(op, out var combinations))
            {
                UnaryOperator uop;
                if ((uop = combinations.FirstOrDefault(t => t.Supports(operandType))) != null)
                {
                    pigeonType = uop.resultType;
                    return true;
                }
            }
            return false;
        }

        private static readonly Dictionary<string, UnaryOperator[]> operators = new Dictionary<string, UnaryOperator[]>()
        {
                {
                    "+",
                    new[]
                    {
                        new UnaryOperator(PigeonType.Int),
                        new UnaryOperator(PigeonType.Float),
                    }
                },
                {
                    "-",
                    new[]
                    {
                        new UnaryOperator(PigeonType.Int),
                        new UnaryOperator(PigeonType.Float)
                    }
                },
                {
                    "!",
                    new[]
                    {
                        new UnaryOperator(PigeonType.Bool)
                    }
                }
            };
    }
}
