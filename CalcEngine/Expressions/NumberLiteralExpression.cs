using CalcEngine.Check;
using CalcEngine.Functions;
using CalcEngine.Parse;

namespace CalcEngine.Expressions;

public record NumberLiteralExpression(double Value) : Expr
{
    public override string Format(IReadOnlyList<Expr> expressions, IReadOnlyList<string> variables)
    {
        return $"({Value})";
    }

    public override TypedExpr TypeCheck(ExprType expectedType, TypedExpr[] typedExpressions, TypedVariable[] typedVariables, object[] constants, ParseResult parseResult, FunctionRegistry functionRegistry)
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
