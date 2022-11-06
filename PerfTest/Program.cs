using CalcEngine.Compile;
using System.Diagnostics;

namespace CalcEngine.PerfTest;

internal class Program
{
    private static void Main(string[] args)
    {
        var expression = "1 + 2 + b + 5 * 3 * 2 / 5 + 2 * 54 * 6 + 1 + 2 + 5 * pow(2, 4)";

        var compiler = new ILCompiler()
        {
            UseCache = false
        };

        ExpressionResult compiled = compiler.Compile(expression);

        var timer = new Stopwatch();
        timer.Start();
        for (int i = 0; i < 1000000; i++)
        {
            compiled = compiler.Compile(expression);
        }
        timer.Stop();
        Console.WriteLine(timer.ElapsedMilliseconds);
        timer.Restart();

        double result = 1;
        for (int i = 0; i < 1000000; i++)
        {
            result *= compiled.Execute(new object[] { result });
        }
        timer.Stop();
        Console.WriteLine(timer.ElapsedMilliseconds);

        Console.WriteLine(result);
    }
}
