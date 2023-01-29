using CalcEngine.Check;
using CalcEngine.Functions;
using CalcEngine.Parse;
using System.Reflection.Emit;

namespace CalcEngine.Compile;

public class ILCompiler : ICompiler
{
    private readonly Parser _parser;
    private readonly FunctionRegistry _functions;
    private ExpressionCache? _cache;
    private delegate object F(object[] constants, IReadOnlyList<object> parameters);
    private static readonly Type[] _methodArgs = new[] { typeof(object[]), typeof(IReadOnlyList<object>) };

    public bool UseCache
    {
        get { return _cache != null; }
        set { if (value) _cache = new ExpressionCache(); else _cache = null; }
    }

    public double ComparisonFactor { get; set; } = 0.01;

    public ILCompiler(FunctionRegistry? functions = null, ExpressionCache? cache = null)
    {
        _parser = new Parser();
        _functions = functions ?? new FunctionRegistry(true);
        _cache = cache;
    }

    public ExpressionResult Compile(ParseResult parsed)
    {
        var method = new DynamicMethod("", typeof(object), _methodArgs);

        TypedVariable[] typedVariables = new TypedVariable[parsed.Variables.Count];
        object[] constants = new object[parsed.ConstantCount];

        TypedExpr typeChecked = parsed.Root.TypeCheck(ExprType.Any, typedVariables, parsed.Variables, constants, _functions);

        ILGenerator il = method.GetILGenerator();

        typeChecked.GenerateIl(il, ComparisonFactor);

        foreach (TypedVariable typedVariable in typedVariables)
        {
            if (typedVariable.Type == ExprType.Any)
            {
                throw new Exception($"Could not determine type for variable {typedVariable.Type}");
            }
        }

        switch (typeChecked.Type)
        {
            case ExprType.Bool:
                il.Emit(OpCodes.Box, typeof(bool));
                break;
            case ExprType.Number:
                il.Emit(OpCodes.Box, typeof(double));
                break;
            case ExprType.Any:
                throw new Exception("Could not determine return type");
                // Don't need to do anything for string, already an object
        }

        il.Emit(OpCodes.Ret);

        Func<object[], IReadOnlyList<object>, object> function = method.CreateDelegate<Func<object[], IReadOnlyList<object>, object>>();

        return new ExpressionResult((variables) => function(constants, variables), typedVariables, typeChecked.Type);
    }

    public ExpressionResult Compile(string expression)
    {
        if (_cache is not null && _cache.TryGetExpression(expression) is ExpressionResult cached)
        {
            return cached;
        }
        var parsed = _parser.Parse(expression);
        var result = Compile(parsed);
        _cache?.AddExpression(expression, result);
        return result;
    }
}
