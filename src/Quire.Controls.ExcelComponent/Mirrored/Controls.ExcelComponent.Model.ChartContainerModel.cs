using System;
using Quire.Calculation;
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

    public override void Commit()
    {
        AssertDispatcherAccess();
        RemoveImageFreezeSource(ImageFreezeSources.AsynchronousCalculation);
    }

    public override global::System.Collections.Generic.IList<global::Quire.Serialization.IRegionPersistentData> SetRegionPersistentData(global::Quire.Serialization.IDeS11NProvider d11NProvider, global::Quire.PersistentData.chartContainerType chartContainer)
    {
        Unpack(d11NProvider, chartContainer.itemidref);
        return null;
    }

    public override IValue GetValue(string place)
    {
        throw new NotImplementedException();
    }

    private void SetString(string place, IValue value)
    {
        throw new AddressErrorException(place);
    }

    public void AddImageFreezeSource(ImageFreezeSources source, bool forceRun = false)
    {
        if (forceRun)
        {
            Run();
        }
        AddOrRemoveImageFreezeSource(source, add: true);
    }

    public void RemoveImageFreezeSource(ImageFreezeSources source)
    {
        AddOrRemoveImageFreezeSource(source, add: false);
    }
}
