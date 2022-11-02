using BenchmarkDotNet.Attributes;

namespace CalcEngine.Benchmarks;

public abstract class BenchmarkBase
{
    [Params(
        "1 + 2",
        "a + b",
        "a * b / (1 + 5)"
    )]
    public string Expression { get; set; } = default!;

    public Dictionary<string, double> Parameters = new Dictionary<string, double>
    {
        { "a", 5},
        { "b", 10},
    };

    public Dictionary<string, object> ObjectParameters = new Dictionary<string, object>
    {
        { "a", 5},
        { "b", 10},
    };
}
