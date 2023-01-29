using System.Reflection;
using System.Reflection.Emit;

namespace CalcEngine.Check;

public record TypedEqualityInfixExpr(TypedExpr Left, EqualityOp Operator, TypedExpr Right, ExprType EqualityType) : TypedExpr(ExprType.Bool)
{
    private static readonly MethodInfo _stringEquals = typeof(string).GetMethod("op_Equality")!;
    private static readonly MethodInfo _stringNotEquals = typeof(string).GetMethod("op_Inequality")!;
    private static readonly MethodInfo _compareNumbers = typeof(TypedEqualityInfixExpr).GetMethod(nameof(CompareNumbers), BindingFlags.NonPublic | BindingFlags.Static)!;

    private static bool CompareNumbers(double a, double b, double comparisonFactor)
    {
        double absolute = Math.Abs(a - b);
        return absolute < a * comparisonFactor || absolute < b * comparisonFactor;
    }

    public override void GenerateIl(ILGenerator il, double comparisonFactor)
    {
        Left.GenerateIl(il, comparisonFactor);
        Right.GenerateIl(il, comparisonFactor);
        switch (EqualityType)
        {
            case ExprType.Number:
                il.Emit(OpCodes.Ldc_R8, comparisonFactor);
                il.EmitCall(OpCodes.Call, _compareNumbers, null);
                if (Operator == EqualityOp.NotEqual)
                {
                    il.Emit(OpCodes.Ldc_I4_0);
                    il.Emit(OpCodes.Ceq);
                }
                break;
            case ExprType.Bool:
                il.Emit(OpCodes.Ceq);
                if (Operator == EqualityOp.NotEqual)
                {
                    il.Emit(OpCodes.Ldc_I4_0);
                    il.Emit(OpCodes.Ceq);
                }
                break;
            case ExprType.String:
                il.EmitCall(OpCodes.Call, Operator == EqualityOp.Equal ? _stringEquals : _stringNotEquals, null);
                break;
            case ExprType.Any:
                throw new InvalidOperationException("Cannot compare types of `Any`");
        }
    }
}

public enum EqualityOp
{
    Equal,
    NotEqual,
}
