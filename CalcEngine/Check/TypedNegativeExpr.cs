using System.Reflection.Emit;

namespace CalcEngine.Check;

public record TypedNegativeExpr(TypedExpr Expr) : TypedExpr(ExprType.Number)
{
    public override void GenerateIl(ILGenerator il, double comparisonFactor)
    {
        Expr.GenerateIl(il, comparisonFactor);
        il.Emit(OpCodes.Neg);
    }
}
