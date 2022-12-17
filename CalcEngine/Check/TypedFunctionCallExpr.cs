using System.Reflection;
using System.Reflection.Emit;

namespace CalcEngine.Check;

public record TypedFunctionCallExpr(MethodInfo MethodInfo, IReadOnlyList<int> Arguments, ExprType Type) : TypedExpr(Type)
{
    public override void GenerateIl(IReadOnlyList<TypedExpr> expressions, ILGenerator il, double comparisonFactor)
    {
        foreach (int argumentIndex in Arguments)
        {
            expressions[argumentIndex].GenerateIl(expressions, il, comparisonFactor);
        }
        il.EmitCall(OpCodes.Call, MethodInfo, null);
    }
}
