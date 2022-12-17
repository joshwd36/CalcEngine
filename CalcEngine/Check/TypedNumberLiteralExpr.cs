using System.Reflection.Emit;

namespace CalcEngine.Check;

public record TypedNumberLiteralExpr(double Value) : TypedExpr(ExprType.Number)
{
    public override void GenerateIl(IReadOnlyList<TypedExpr> expressions, ILGenerator il, double comparisonFactor)
    {
        il.Emit(OpCodes.Ldc_R8, Value);
    }
}
