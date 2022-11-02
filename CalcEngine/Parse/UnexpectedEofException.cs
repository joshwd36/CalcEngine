namespace CalcEngine.Parse;

public class UnexpectedEofException : Exception
{
    public UnexpectedEofException() : base("Unexpected end of input")
    {
    }
}
