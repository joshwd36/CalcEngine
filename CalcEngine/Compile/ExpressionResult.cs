using CalcEngine.Check;

namespace CalcEngine.Compile;

public record ExpressionResult(Func<IReadOnlyList<object>, object> Function, IReadOnlyList<TypedVariable> Variables, ExprType ReturnType)
{
    public object Execute(IReadOnlyDictionary<string, object> parameters)
    {
        return Function(Variables.Select(p => parameters[p.Name]).ToArray());
    }

    public object Execute(IReadOnlyList<object> parameters)
    {
        return Function(parameters);
    }
}
