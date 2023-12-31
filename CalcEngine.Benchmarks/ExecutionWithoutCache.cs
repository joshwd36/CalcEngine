using BenchmarkDotNet.Attributes;
using CalcEngine.Compile;
using Jace;

namespace CalcEngine.Benchmarks;

public class ExecutionWithoutCache : BenchmarkBase
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

    private ExpressionResult _calcEngineCompiled = default!;
    private object[] listParameters = default!;
    private Func<IDictionary<string, double>, double> _jaceCompiled = default!;
    private NCalc.Expression _ncalcCompiled = default!;

    [GlobalSetup]
    public void GlobalSetup()
    {
        _calcEngineCompiled = _calcEngine.Compile(Expression);
        listParameters = _calcEngineCompiled.Variables.Select(p => (object)Parameters[p.Name]).ToArray();
        _jaceCompiled = _jace.Build(Expression);
        _ncalcCompiled = new NCalc.Expression(NCalc.Expression.Compile(Expression, true));
    }

    [Benchmark]
    public double CalcEngine() => (double)_calcEngineCompiled.Execute(ObjectParameters);

    [Benchmark]
    public double CalcEngineList() => (double)_calcEngineCompiled.Execute(listParameters);

    [Benchmark]
    public double JaceBuild() => _jaceCompiled(Parameters);

    [Benchmark]
    public object Ncalc()
    {
        _ncalcCompiled.Parameters = ObjectParameters;
        return _ncalcCompiled.Evaluate();
    }
}
