using BenchmarkDotNet.Attributes;
using CalcEngine.Compile;
using Jace;

namespace CalcEngine.Benchmarks;

public class FullWithCache : BenchmarkBase
{
    private readonly ICompiler _calcEngine = new ILCompiler();
    private readonly CalculationEngine _jace = new();

    [Benchmark]
    public double CalcEngine() => (double)_calcEngine.Compile(Expression).Execute(ObjectParameters);

    [Benchmark]
    public double JaceBuild() => _jace.Build(Expression)(Parameters);

    [Benchmark]
    public double Ncalc() => (double)new NCalc.Expression(NCalc.Expression.Compile(Expression, false))
    { Parameters = ObjectParameters }.Evaluate();
}
