using CalcEngine.Check;
using CalcEngine.Functions;
using CalcEngine.Parse;

namespace CalcEngine.Expressions;

public record StringLiteralExpression(string Value) : Expr
{
    public override string Format(IReadOnlyList<Expr> expressions, IReadOnlyList<string> variables)
    {
        return $"\"{Value}\"";
    }

    public override TypedExpr TypeCheck(ExprType expectedType, TypedExpr[] typedExpressions, TypedVariable[] typedVariables, ParseResult parseResult, FunctionRegistry functionRegistry)
    {
        if (expectedType == ExprType.Any || expectedType == ExprType.String)
        {
            return new TypedStringLiteralExpr(Value);
        }
        else
        {
            throw new InvalidTypeException(expectedType, ExprType.String);
        }
    }
}
