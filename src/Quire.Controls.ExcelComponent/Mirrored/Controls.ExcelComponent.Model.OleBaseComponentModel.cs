using Quire.Calculation.Core;

namespace Quire.Controls.ExcelComponent.Model;

public abstract partial class OleBaseComponentModel
{
    public global::System.Windows.ResourceDictionary Resources { get; set; }

    public override int BlockSectionCount => 2;

    internal virtual global::Quire.Controls.ExcelComponent.View.IOleContainerItem OleContainerItem { get; set; }

    public global::Quire.Calculation.Core.EngineError Error => _error;

    protected uint ComponentNodeHandle { get; set; }

    public int ComponentId { get; set; }

    private static void HandleContainerChildrenChanged()
    {
    }

    protected virtual ICalculatable LookupCalculatableWrapper(ICalculatable calc)
    {
        return GetWrapper(calc);
    }

    protected virtual void SetErrorFallback(EngineError error)
    {
        SetChildError(null);
        SetComponentError(error);
    }
}
