using CalcEngine.Expressions;

namespace CalcEngine.Parse;

public record ParseResult(Expr Expression, IReadOnlyList<string> Variables);
