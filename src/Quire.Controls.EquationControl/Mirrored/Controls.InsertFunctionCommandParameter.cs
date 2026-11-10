using Quire.Controls.Core;

namespace Quire.Controls;

public partial class InsertFunctionCommandParameter
{
    private int _argsCount;
    private string[] _args;

    public InsertFunctionCommandParameter(string funcName, int argsCount, IdLabels? label)
        : base(funcName, label)
    {
        _argsCount = argsCount;
    }

    public InsertFunctionCommandParameter(string funcName, int argsCount)
        : this(funcName, argsCount, IdLabels.Function)
    {
    }

    public InsertFunctionCommandParameter(string funcName, string[] args)
        : this(funcName, args.Length, IdLabels.Function)
    {
        _args = args;
    }

    public int ArgsCount
    {
        get => _argsCount;
        set => _argsCount = value;
    }

    public string[] Args
    {
        get => _args;
        set => _args = value;
    }
}
