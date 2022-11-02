namespace CalcEngine.Utils;

public static class PeekableUtils
{
    public static Peekable<T> Peekable<T>(this IEnumerable<T> source)
        where T : notnull
    {
        return new Peekable<T>(source);
    }
}

public class Peekable<T>
    where T : notnull
{
    private readonly IEnumerator<T> _enumerator;
    private T? _peeked;

    public Peekable(IEnumerable<T> enumerable)
    {
        _enumerator = enumerable.GetEnumerator();
    }

    public T? Next()
    {
        if (_peeked is T value)
        {
            _peeked = default;
            return value;
        }
        else if (_enumerator.MoveNext())
        {
            return _enumerator.Current;
        }
        else
        {
            return default;
        }
    }

    public T? Peek()
    {
        if (_peeked is T value)
        {
            return value;
        }
        else if (_enumerator.MoveNext())
        {
            _peeked = _enumerator.Current;
            return _peeked;
        }
        else
        {
            return default;
        }
    }
}
