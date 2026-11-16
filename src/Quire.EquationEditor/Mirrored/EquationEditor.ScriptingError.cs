using Quire.Calculation.Core;

namespace Quire.EquationEditor;

public partial class ScriptingError
{
    private readonly string a;
    private readonly StringCollection b;
    private uint c;
    private uint d;
    private string e;
    private readonly ICalculatable f;
    private string g;
    private bool h;
    private readonly EngineError i;

    public string ErrorCode => a;

    public StringCollection ErrorMessageArguments => b;

    public uint NodeHandle
    {
        get => c;
        set => c = value;
    }

    public uint Argument
    {
        get => d;
        set => d = value;
    }

    public string Message
    {
        get => e;
        set => e = value;
    }

    public ICalculatable ErrorSource => f;

    public string ResourceString
    {
        get => g;
        set => g = value;
    }

    public bool IsRuntime
    {
        get => h;
        set => h = value;
    }

    public EngineError OriginalError => i;

    public ScriptingError(string error)
    {
        ResourceString = error;
        a = error;
    }

    public object Clone()
    {
        return this;
    }

    public bool IsEquivalentTo(EngineError error, bool checkNodeHandle = true)
    {
        return true;
    }

    public EngineError FindNextError(EngineError error)
    {
        return null;
    }

    public bool IsAncestorOf(EngineError error)
    {
        return false;
    }
}
