#nullable disable
using System;
using System.Collections.Generic;
using System.Windows;
using Quire.Controls.Core;
using Quire.Controls.Core.Serialization;
using Quire.Controls.Include;
using Quire.Mirror;
using Quire.PersistentData;
using Quire.Serialization;

namespace Quire.Controls;

internal partial class WorksheetSectionDeserializationStrategy : DeserializationStrategyWithMathFormat
{
    private readonly IWorksheetSectionPersistentData _worksheetSectionData;

    public WorksheetSectionDeserializationStrategy(IWorksheetSectionPersistentData sectionData, IMathFormat mathFormat, IDictionary<IdLabels, ExtendedFont> labeledIdFormat)
    {
        _worksheetSectionData = sectionData;
        DeserializeMathFormat = mathFormat;
        DeserializeLabeledIdFormat = labeledIdFormat;
    }

    public override void DeserializeRegionProlog(UIElement region)
    {
        if (region is IMultiLevelInclusionProvider multiLevelInclusionProvider)
        {
            multiLevelInclusionProvider.MultiLevelInclusionItem.CacheValid = true;
        }
        base.DeserializeRegionProlog(region);
    }

    [MirrorVerified("PtcControlsWorksheetSectionDeserializationStrategyMechanicalDifferentialTests")]
    public override IList<UIElement> Regions => throw new NotImplementedException();

    [MirrorVerified("PtcControlsWorksheetSectionDeserializationStrategyMechanicalDifferentialTests")]
    public override void AddRegion(UIElement element, Point location) => throw new NotImplementedException();

    [MirrorVerified("PtcControlsWorksheetSectionDeserializationStrategyMechanicalDifferentialTests")]
    public override void DeleteRegion(UIElement control) => throw new NotImplementedException();

    public override IRegionFactory RegionFactory => Quire.Controls.Worksheet.RegionFactory.Instance;

    protected override void AddRegion(IRegionPersistentData regionPersistentData)
    {
        _worksheetSectionData.SerializedRegions.Add(regionPersistentData);
    }

    protected override void CleanUpLastRegion(IRegionPersistentData regionPersistentData)
    {
        _worksheetSectionData.SerializedRegions.Remove(regionPersistentData);
    }
}
