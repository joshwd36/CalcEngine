using CalcEngine.Parse;

namespace CalcEngine.Compile;

public interface ICompiler
{
    ExpressionResult Compile(string expression);
    ExpressionResult Compile(ParseResult parsed);
}