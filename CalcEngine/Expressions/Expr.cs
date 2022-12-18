using CalcEngine.Check;
using CalcEngine.Functions;
using CalcEngine.Parse;

namespace CalcEngine.Expressions;

public abstract record Expr
{
    public abstract string Format(IReadOnlyList<Expr> expressions, IReadOnlyList<string> variables);

    public abstract TypedExpr TypeCheck(ExprType expectedType, TypedExpr[] typedExpressions, TypedVariable[] typedVariables, object[] constants, ParseResult parseResult, FunctionRegistry functionRegistry);
}
