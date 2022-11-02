using CalcEngine.Compile;

namespace CalcEngine.PerfTest;

internal class Program
{
    private static void Main(string[] args)
    {
        var expression = "1 + 2 + 3";

        var compiler = new ILCompiler()
        {
            UseCache = false
        };

        ExpressionResult compiled = compiler.Compile(expression);

        for (int i = 0; i < 100000; i++)
        {
            compiled = compiler.Compile(expression);
        }

        double result = 0;
        for (int i = 0; i < 100000; i++)
        {
            result = compiled.Execute();
        }

        Console.WriteLine(result);
    }
}