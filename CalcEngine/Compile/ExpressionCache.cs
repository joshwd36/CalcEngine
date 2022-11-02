using System.Collections.Concurrent;

namespace CalcEngine.Compile;

public class ExpressionCache
{
    private readonly IDictionary<string, ExpressionResult> _expressions;

    public ExpressionCache()
    {
        _expressions = new ConcurrentDictionary<string, ExpressionResult>();
    }

    public ExpressionResult? TryGetExpression(string expression)
    {
        if (_expressions.TryGetValue(expression, out var result))
        {
            return result;
        }
        else
        {
            return null;
        }
    }

    public void AddExpression(string expression, ExpressionResult result)
    {
        _expressions[expression] = result;
    }
}
