using System.Reflection;

namespace CalcEngine.Functions;

public class FunctionRegistry
{
    private readonly Dictionary<string, MethodInfo> _functions;

    public FunctionRegistry(bool addDefault = true)
    {
        _functions = new Dictionary<string, MethodInfo>();
        if (addDefault)
        {
            _functions.Add("sqrt", typeof(Math).GetMethod(nameof(Math.Sqrt))!);
            _functions.Add("pow", typeof(Math).GetMethod(nameof(Math.Pow))!);
        }
    }

    public void AddFunction(string name, MethodInfo method)
    {
        _functions.Add(name, method);
    }

    public void AddFunction<T1>(string name, Func<T1> method)
    {
        _functions.Add(name, method.Method);
    }

    public void AddFunction<T1, T2>(string name, Func<T1, T2> method)
    {
        _functions.Add(name, method.Method);
    }

    public void AddFunction<T1, T2, T3>(string name, Func<T1, T2, T3> method)
    {
        _functions.Add(name, method.Method);
    }

    public void AddFunction<T1, T2, T3, T4>(string name, Func<T1, T2, T3, T4> method)
    {
        _functions.Add(name, method.Method);
    }

    public void AddFunction<T1, T2, T3, T4, T5>(string name, Func<T1, T2, T3, T4, T5> method)
    {
        _functions.Add(name, method.Method);
    }

    public void AddFunction<T1, T2, T3, T4, T5, T6>(string name, Func<T1, T2, T3, T4, T5, T6> method)
    {
        _functions.Add(name, method.Method);
    }

    public MethodInfo GetFunction(string name)
    {
        if (_functions.TryGetValue(name, out var value))
        {
            return value;
        }
        else
        {
            throw new InvalidFunctionException(name);
        }
    }
}
