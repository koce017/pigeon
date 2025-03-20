using Kostic017.Pigeon.Symbols;
using System.Linq;

namespace Kostic017.Pigeon.Operators
{
    class TernaryOperator
    {
        private readonly PigeonType whenTrueType;
        private readonly PigeonType whenFalseType;
        private readonly PigeonType resultType;

        private TernaryOperator(PigeonType whenTrueType, PigeonType whenFalseType, PigeonType resultType)
        {
            this.whenTrueType = whenTrueType;
            this.whenFalseType = whenFalseType;
            this.resultType = resultType;
        }

        internal static bool TryGetResType(PigeonType whenTrue, PigeonType whenFalse, out PigeonType pigeonType)
        {
            pigeonType = PigeonType.Error;
            TernaryOperator top;
            if ((top = combinations.FirstOrDefault(t => t.Supports(whenTrue, whenFalse))) != null)
            {
                pigeonType = top.resultType;
                return true;
            }
            return false;
        }


        private bool Supports(PigeonType whenTrue, PigeonType whenFalse)
        {
            return whenTrueType == whenTrue && whenFalseType == whenFalse;
        }

        private static readonly TernaryOperator[] combinations = new TernaryOperator[]
        {
            new TernaryOperator(PigeonType.Int, PigeonType.Int, PigeonType.Int),
            new TernaryOperator(PigeonType.Int, PigeonType.Float, PigeonType.Float),
            new TernaryOperator(PigeonType.Float, PigeonType.Int, PigeonType.Float),
            new TernaryOperator(PigeonType.Float, PigeonType.Float, PigeonType.Float),
            new TernaryOperator(PigeonType.Bool, PigeonType.Bool, PigeonType.Bool),
            new TernaryOperator(PigeonType.String, PigeonType.String, PigeonType.String),
        };
    }
}
