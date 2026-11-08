namespace Quire.EquationEditor.Tree.Transform;

public partial class EquationXmlParseErrorException
{
    private ErrorCodes a;

    public EquationXmlParseErrorException(string message, ErrorCodes errorCode = ErrorCodes.NoCode)
        : base(message)
    {
        ErrorCode = errorCode;
    }

    // The Release IL does not pass errorCode to the base class or store it.
    public EquationXmlParseErrorException(string message, System.Exception inner, ErrorCodes errorCode = ErrorCodes.NoCode)
        : base(message, inner)
    {
    }

    public ErrorCodes ErrorCode
    {
        get => a;
        set => a = value;
    }
}
