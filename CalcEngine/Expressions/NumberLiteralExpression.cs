using CalcEngine.Check;
using CalcEngine.Functions;

namespace CalcEngine.Expressions;

public record NumberLiteralExpression(double Value) : Expr
{
    public override string Format(IReadOnlyList<string> variables)
    {
        return $"({Value})";
    }

    public override TypedExpr TypeCheck(ExprType expectedType, TypedVariable[] typedVariables, IReadOnlyList<string> variables, object[] constants, FunctionRegistry functionRegistry)
    {
        if (expectedType == ExprType.Any || expectedType == ExprType.Number)
        {
            return new TypedNumberLiteralExpr(Value);
        }
        else
        {
            throw new InvalidTypeException(expectedType, ExprType.Number);
        }
    }
}
