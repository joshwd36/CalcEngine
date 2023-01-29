using BenchmarkDotNet.Attributes;
using CalcEngine.Compile;
using Jace;

namespace CalcEngine.Benchmarks;

public class FullWithoutCache : BenchmarkBase
{
    private readonly ICompiler _calcEngine = new ILCompiler()
    { UseCache = false };
    private readonly CalculationEngine _jace = new(new JaceOptions()
    {
        CacheEnabled = false
    });

    [Benchmark]
    public double CalcEngine() => (double)_calcEngine.Compile(Expression).Execute(ObjectParameters);

    [Benchmark]
    public double JaceBuild() => _jace.Build(Expression)(Parameters);

    [Benchmark]
    public double Ncalc() => (double)new NCalc.Expression(NCalc.Expression.Compile(Expression, true))
    { Parameters = ObjectParameters }.Evaluate();
}
