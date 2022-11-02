namespace CalcEngine.Compile;

public interface ICompiler
{
    ExpressionResult Compile(string expression);
}