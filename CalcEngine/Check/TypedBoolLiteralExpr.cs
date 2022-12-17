using System.Reflection.Emit;

namespace CalcEngine.Check;

public record TypedBoolLiteralExpr(bool Value) : TypedExpr(ExprType.Bool)
{
    public override void GenerateIl(IReadOnlyList<TypedExpr> expressions, ILGenerator il, double comparisonFactor)
    {
        il.Emit(OpCodes.Ldc_I4, Value ? 1 : 0);
    }
}
