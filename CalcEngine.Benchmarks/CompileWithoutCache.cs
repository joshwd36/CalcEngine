using BenchmarkDotNet.Attributes;
using CalcEngine.Compile;
using Jace;
using NCalc.Domain;

namespace CalcEngine.Benchmarks;

public class CompileWithoutCache : BenchmarkBase
{
    private readonly ICompiler _calcEngine = new ILCompiler()
    {
        UseCache = false
    };
    private readonly CalculationEngine _jace = new CalculationEngine(
        new JaceOptions()
        {
            CacheEnabled = false
        }
    );

    [Benchmark]
    public ExpressionResult CalcEngine() => _calcEngine.Compile(Expression);

    [Benchmark]
    public Func<IDictionary<string, double>, double> JaceBuild() => _jace.Build(Expression);

    [Benchmark]
    public LogicalExpression Ncalc() => NCalc.Expression.Compile(Expression, true);
}
