using CalcEngine.Check;
using CalcEngine.Functions;

namespace CalcEngine.Expressions;

public abstract record Expr
{
    public abstract string Format(IReadOnlyList<string> variables);

    public abstract TypedExpr TypeCheck(ExprType expectedType, TypedVariable[] typedVariables, IReadOnlyList<string> variables, object[] constants, FunctionRegistry functionRegistry);
}
