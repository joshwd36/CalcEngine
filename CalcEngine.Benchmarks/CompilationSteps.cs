using BenchmarkDotNet.Attributes;
using CalcEngine.Compile;
using CalcEngine.Parse;

namespace CalcEngine.Benchmarks
{
    public class CompilationSteps : BenchmarkBase
    {
        private readonly ICompiler _calcEngine = new ILCompiler();
        private readonly Parser _parser = new();
        private ParseResult _parseResult = default!;


        [GlobalSetup]
        public void GlobalSetup()
        {
            _parseResult = _parser.Parse(Expression);
        }

        [Benchmark]
        public ParseResult Parse() => _parser.Parse(Expression);

        [Benchmark]
        public ExpressionResult Compile() => _calcEngine.Compile(_parseResult);
    }
}
