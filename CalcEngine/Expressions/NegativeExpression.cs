using CalcEngine.Check;
using CalcEngine.Functions;
using CalcEngine.Parse;

namespace CalcEngine.Expressions;

public record NegativeExpression(int Expression) : Expr
{
    public override string Format(IReadOnlyList<Expr> expressions, IReadOnlyList<string> variables)
    {
        return $"(-{expressions[Expression].Format(expressions, variables)})";
    }

    public override TypedExpr TypeCheck(ExprType expectedType, TypedExpr[] typedExpressions, TypedVariable[] typedVariables, object[] constants, ParseResult parseResult, FunctionRegistry functionRegistry)
    {
        if (expectedType == ExprType.Any || expectedType == ExprType.Number)
        {
            typedExpressions[Expression] = parseResult.Expressions[Expression].TypeCheck(ExprType.Number, typedExpressions, typedVariables, constants, parseResult, functionRegistry);
            return new TypedNegativeExpr(Expression);
        }
        else
        {
            throw new InvalidTypeException(expectedType, ExprType.Number);
        }
    }
}
