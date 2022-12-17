using System.Reflection.Emit;

namespace CalcEngine.Check;

public record TypedNegativeExpr(int Index) : TypedExpr(ExprType.Number)
{
    public override void GenerateIl(IReadOnlyList<TypedExpr> expressions, ILGenerator il, double comparisonFactor)
    {
        expressions[Index].GenerateIl(expressions, il, comparisonFactor);
        il.Emit(OpCodes.Neg);
    }
}
