namespace Quire.AutomationServiceEngNotePlus;

public partial class AutomationSpiritInputsOutputsConflicts
{
    private readonly global::Quire.Controls.IO.IInputOutputItemsConflicts _conflicts;

    public AutomationSpiritInputsOutputsConflicts(global::Quire.Controls.IO.IInputOutputItemsConflicts conflicts)
    {
        _conflicts = conflicts;
    }

    public void AddGeneralWarning(string warningArg)
    {
        try
        {
            _conflicts.AddGeneralWarning(warningArg);
        }
        catch
        {
        }
    }

    public void AddGeneralError(string errorArg)
    {
        try
        {
            _conflicts.AddGeneralError(errorArg);
        }
        catch
        {
        }
    }

    public void AddItemWarning(string aliasArg, string warningArg)
    {
        try
        {
            _conflicts.AddItemWarning(aliasArg, warningArg);
        }
        catch
        {
        }
    }

    public void AddItemError(string aliasArg, string errorArg)
    {
        try
        {
            _conflicts.AddItemError(aliasArg, errorArg);
        }
        catch
        {
        }
    }
}
