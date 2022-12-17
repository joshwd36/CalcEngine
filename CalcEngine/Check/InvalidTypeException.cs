namespace CalcEngine.Check;

public class InvalidTypeException : Exception
{
    public ExprType Expected { get; }
    public ExprType Found { get; }

    public InvalidTypeException(ExprType expected, ExprType found) : base($"Invalid type, expected {expected}, found {found}")
    {
        Expected = expected;
        Found = found;
    }
}
