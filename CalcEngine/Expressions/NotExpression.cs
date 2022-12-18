using CalcEngine.Check;
using CalcEngine.Functions;
using CalcEngine.Parse;

namespace CalcEngine.Expressions;

public record NotExpression(int Expression) : Expr
{
    public override string Format(IReadOnlyList<Expr> expressions, IReadOnlyList<string> variables)
    {
        return $"(!{expressions[Expression].Format(expressions, variables)})";
    }

    public override TypedExpr TypeCheck(ExprType expectedType, TypedExpr[] typedExpressions, TypedVariable[] typedVariables, object[] constants, ParseResult parseResult, FunctionRegistry functionRegistry)
    {
        if (expectedType == ExprType.Any || expectedType == ExprType.Bool)
        {
            typedExpressions[Expression] = parseResult.Expressions[Expression].TypeCheck(ExprType.Bool, typedExpressions, typedVariables, constants, parseResult, functionRegistry);
            return new TypedNotExpr(Expression);
        }
        else
        {
            throw new InvalidTypeException(expectedType, ExprType.Bool);
        }
    }
}
