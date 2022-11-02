using BenchmarkDotNet.Attributes;
using CalcEngine.Compile;
using Jace;

namespace CalcEngine.Benchmarks;

public class ExecutionWithCache : BenchmarkBase
{
    private readonly ICompiler _calcEngine = new ILCompiler();
    private readonly CalculationEngine _jace = new CalculationEngine();

    private ExpressionResult _calcEngineCompiled = default!;
    private Func<IDictionary<string, double>, double> _jaceCompiled = default!;
    private NCalc.Expression _ncalcCompiled = default!;

    [GlobalSetup]
    public void GlobalSetup()
    {
        _calcEngineCompiled = _calcEngine.Compile(Expression);
        _jaceCompiled = _jace.Build(Expression);
        _ncalcCompiled = new NCalc.Expression(NCalc.Expression.Compile(Expression, false));
    }

    [Benchmark]
    public double CalcEngine() => _calcEngineCompiled.Execute(ObjectParameters);

    [Benchmark]
    public double JaceBuild() => _jaceCompiled(Parameters);

    [Benchmark]
    public object Ncalc()
    {
        _ncalcCompiled.Parameters = ObjectParameters;
        return _ncalcCompiled.Evaluate();
    }
}
