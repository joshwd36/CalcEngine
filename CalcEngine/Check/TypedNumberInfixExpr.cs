using System.Reflection.Emit;

namespace CalcEngine.Check;

public record TypedNumberInfixExpr(TypedExpr Left, NumberOp Operator, TypedExpr Right) : TypedExpr(ExprType.Number)
{
    public override void GenerateIl(ILGenerator il, double comparisonFactor)
    {
        Left.GenerateIl(il, comparisonFactor);
        Right.GenerateIl(il, comparisonFactor);

        switch (Operator)
        {
            case NumberOp.Addition:
                il.Emit(OpCodes.Add);
                break;
            case NumberOp.Subtraction:
                il.Emit(OpCodes.Sub);
                break;
            case NumberOp.Division:
                il.Emit(OpCodes.Div);
                break;
            case NumberOp.Multiplication:
                il.Emit(OpCodes.Mul);
                break;
            case NumberOp.Remainder:
                il.Emit(OpCodes.Rem);
                break;
        }
    }
}

public enum NumberOp
{
    Addition,
    Subtraction,
    Division,
    Multiplication,
    Remainder,
}
