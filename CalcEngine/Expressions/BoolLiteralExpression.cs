using CalcEngine.Check;
using CalcEngine.Functions;
using CalcEngine.Parse;

namespace CalcEngine.Expressions;

public record BoolLiteralExpression(bool Value) : Expr
{
    public override string Format(IReadOnlyList<Expr> expressions, IReadOnlyList<string> variables)
    {
        return $"{Value}";
    }

    public override TypedExpr TypeCheck(ExprType expectedType, TypedExpr[] typedExpressions, TypedVariable[] typedVariables, object[] constants, ParseResult parseResult, FunctionRegistry functionRegistry)
    {
        if (expectedType == ExprType.Bool || expectedType == ExprType.Any)
        {
            return new TypedBoolLiteralExpr(Value);
        }
        else
        {
            throw new InvalidTypeException(expectedType, ExprType.Bool);
        }
    }
}
