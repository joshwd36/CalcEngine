namespace CalcEngine.Tokenise;

public class InvalidCharacterException : Exception
{
    public InvalidCharacterException(char character, int position) : base($"Invalid character {character} at position {position}")
    {
        Character = character;
        Position = position;
    }

    public char Character { get; }
    public int Position { get; }
}
