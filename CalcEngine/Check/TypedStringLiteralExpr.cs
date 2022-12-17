using System.Reflection.Emit;

namespace CalcEngine.Check;

public record TypedStringLiteralExpr(string Value) : TypedExpr(ExprType.String)
{
    public override void GenerateIl(IReadOnlyList<TypedExpr> expressions, ILGenerator il, double comparisonFactor)
    {
        il.Emit(OpCodes.Ldstr, Value);
    }
}
