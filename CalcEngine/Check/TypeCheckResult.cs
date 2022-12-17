using CalcEngine.Expressions;

namespace CalcEngine.Check;

public record TypeCheckResult(int Root, IReadOnlyList<Expr> Expressions, IReadOnlyList<TypedVariable> Variables);

public record TypedVariable(string Name, ExprType Type);
