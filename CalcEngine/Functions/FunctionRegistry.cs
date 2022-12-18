using CalcEngine.Check;

namespace CalcEngine.Functions;

public class FunctionRegistry
{
    private readonly Dictionary<string, List<FunctionEntry>> _functions;

    private static string IfString(bool condition, string ifTrue, string ifFalse)
    {
        return condition ? ifTrue : ifFalse;
    }

    private static double IfNumber(bool condition, double ifTrue, double ifFalse)
    {
        return condition ? ifTrue : ifFalse;
    }

    private static bool IfBool(bool condition, bool ifTrue, bool ifFalse)
    {
        return condition ? ifTrue : ifFalse;
    }

    public FunctionRegistry(bool addDefault = true)
    {
        _functions = new Dictionary<string, List<FunctionEntry>>();
        if (addDefault)
        {
            AddFunction("sqrt", Math.Sqrt);
            AddFunction("pow", Math.Pow);
            AddFunction("ifString", IfString);
            AddFunction("ifNumber", IfNumber);
            AddFunction("ifBool", IfBool);
            AddFunction("if", IfString);
            AddFunction("if", IfNumber);
            AddFunction("if", IfBool);
            AddFunction("pi", () => Math.PI);
        }
    }

    public void AddFunction(string name, Delegate method, Type[]? delegateArgs = null)
    {
        List<ExprType> parameterTypes = method.Method.GetParameters().Select(p => GetDataType(p.ParameterType)).ToList();
        delegateArgs ??= method.Method.GetParameters().Select(a => a.ParameterType).ToArray();
        ExprType returnType = GetDataType(method.Method.ReturnType);
        FunctionEntry newEntry = new(method, delegateArgs, parameterTypes, returnType);
        if (_functions.TryGetValue(name, out List<FunctionEntry>? existing) && existing is not null)
        {
            if (existing.Any(e => e.ArgumentTypes.Count == parameterTypes.Count && e.ArgumentTypes.Zip(parameterTypes, (a, b) => a == b).All(b => b)))
            {
                throw new Exception($"Function {name} with return type {returnType} already exists");
            }
            else
            {
                existing.Add(newEntry);
            }
        }
        else
        {
            _functions.Add(name, new List<FunctionEntry> { newEntry });
        }
    }

    private static ExprType GetDataType(Type type)
    {
        if (type == typeof(double))
        {
            return ExprType.Number;
        }
        else if (type == typeof(string))
        {
            return ExprType.String;
        }
        else if (type == typeof(bool))
        {
            return ExprType.Bool;
        }
        else
        {
            throw new Exception($"Cannot use method with type {type}");
        }
    }

    public void AddFunction<T1>(string name, Func<T1> method)
    {
        AddFunction(name, method, Array.Empty<Type>());
    }

    public void AddFunction<T1, T2>(string name, Func<T1, T2> method)
    {
        AddFunction(name, method, new[] { typeof(T1) });
    }

    public void AddFunction<T1, T2, T3>(string name, Func<T1, T2, T3> method)
    {
        AddFunction(name, method, new[] { typeof(T1), typeof(T2) });
    }

    public void AddFunction<T1, T2, T3, T4>(string name, Func<T1, T2, T3, T4> method)
    {
        AddFunction(name, method, new[] { typeof(T1), typeof(T2), typeof(T3) });
    }

    public void AddFunction<T1, T2, T3, T4, T5>(string name, Func<T1, T2, T3, T4, T5> method)
    {
        AddFunction(name, method, new[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4) });
    }

    public void AddFunction<T1, T2, T3, T4, T5, T6>(string name, Func<T1, T2, T3, T4, T5, T6> method)
    {
        AddFunction(name, method, new[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5) });
    }

    public FunctionEntry GetFunction(string name, ExprType[] parameterTypes)
    {
        if (_functions.TryGetValue(name, out var value))
        {
            var matching = value.Where(f => f.ArgumentTypes.Count == parameterTypes.Length && f.ArgumentTypes.Zip(parameterTypes, (a, b) => b == ExprType.Any || a == b).All(b => b));
            return matching.SingleOrDefault() ?? throw new InvalidFunctionException(name);
        }
        else
        {
            throw new InvalidFunctionException(name);
        }
    }
}
