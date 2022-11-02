namespace CalcEngine.Functions;

public class InvalidFunctionException : Exception
{
    public string Name { get; }

    public InvalidFunctionException(string name) : base($"Invalid function `{name}`")
    {
        Name = name;
    }
}
