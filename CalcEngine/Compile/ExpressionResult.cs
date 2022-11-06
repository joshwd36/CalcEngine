namespace CalcEngine.Compile;

public record ExpressionResult(Func<object[], double> Function, IReadOnlyList<string> Parameters)
{
    public double Execute(IReadOnlyDictionary<string, object> parameters)
    {
        return (double)Function(Parameters.Select(p => parameters[p]).ToArray());
    }

    public double Execute(object[] parameters)
    {
        return Function(parameters);
    }
}
