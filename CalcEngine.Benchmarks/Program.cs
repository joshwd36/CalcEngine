using BenchmarkDotNet.Running;

namespace CalcEngine.Benchmarks;

internal class Program
{
    private static void Main(string[] args)
    {
        var summary = BenchmarkRunner.Run(typeof(CompilationSteps));
    }
}
