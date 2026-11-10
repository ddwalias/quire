using Quire.FunctionalitiesLimitation;

namespace Quire.Controls.ExcelComponent.Model;

public partial class ChartContainerModel
{
    public override string ProgId => "PTC.ChartAppOLE";

    public override bool AsyncUpdateVisibleSize => true;

    protected override string[] ExcludeElementsNames => _excludeElementsNames;

    protected override EnumFunctionalityKey ToggleActivateStateFunctionalityKey => (EnumFunctionalityKey)800;

    public bool IsTypeDefault => true;

    public global::Quire.Calculation.Core.Trace2DResults.TraceType Type => default;

    public override bool ShouldCommitAtLeastOnce => false;

    public override bool ShouldCommitIfDirty => true;

    protected override bool CheckOleActiveState => false;

    public bool IsImageFrozen => ImageFreezeSources != ImageFreezeSources.None;
}
