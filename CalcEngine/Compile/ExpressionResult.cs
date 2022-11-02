namespace CalcEngine.Compile;

public record ExpressionResult(Delegate Function, IReadOnlyList<string> Parameters)
{
    public double Execute(IReadOnlyDictionary<string, object> parameters)
    {
        return (double)Function.DynamicInvoke(Parameters.Select(p => parameters[p]).ToArray())!;
    }

    public double Execute(IEnumerable<object> parameters)
    {
        return Execute(parameters.ToArray());
    }

    public double Execute(params object[] parameters)
    {
        return (double)Function.DynamicInvoke(parameters)!;
    }
}
