#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Xml;
using Quire;
using Quire.Core;
using Quire.Mirror;
using Quire.PersistentDataObjects;
using Quire.Serialization;

namespace Quire.Controls.Core.Serialization;

public abstract partial class DeserializationStrategy : IDeserializationStrategy
{
    public abstract IList<UIElement> Regions { get; }

    [MirrorVerified("PtcControlsCoreSerializationDeserializationStrategyMechanicalDifferentialTests")]
    public IMathFormat DeserializeMathFormat { get; set; }

    [MirrorVerified("PtcControlsCoreSerializationDeserializationStrategyMechanicalDifferentialTests")]
    public IDictionary<IdLabels, ExtendedFont> DeserializeLabeledIdFormat { get; set; }

    public abstract void AddRegion(UIElement element, Point location);

    public abstract void DeleteRegion(UIElement control);

    public abstract IRegionFactory RegionFactory { get; }

    [MirrorVerified("PtcControlsCoreSerializationDeserializationStrategyMechanicalDifferentialTests")]
    protected virtual bool IsRegionNotAffectingCalculation(UIElement element) => false;

    protected virtual bool IsRegionNotAffectingCalculation(IRegionType regionData)
    {
        object item = regionData.Item;
        if (item is plotType || item is pictureType)
        {
            return true;
        }
        if (item is textType textType2)
        {
            if (textType2.Regions != null)
            {
                return !textType2.Regions.Any();
            }
            return true;
        }
        if (item is ICalculationHideable)
        {
            return (item as ICalculationHideable).IsCalculationDisabled;
        }
        return false;
    }

    public IList<IRegionPersistentData> SetRegions(IEnumerable<IRegionType> regions, IDeS11NProvider deS11NProvider)
    {
        List<IRegionPersistentData> list = new List<IRegionPersistentData>();
        bool skipRegionsNotAffectingCalculation = deS11NProvider.SkipRegionsNotAffectingCalculation;
        foreach (IRegionType region in regions)
        {
            Func<IRegionType, bool> skip = (IRegionType data) => skipRegionsNotAffectingCalculation && IsRegionNotAffectingCalculation(data);
            IRegionPersistentData regionPersistentData = null;
            try
            {
                regionPersistentData = RegionFactory.ConvertFromIRegionType(deS11NProvider, DeserializeRegionProlog, region, skip);
                if (regionPersistentData != null)
                {
                    AddRegion(regionPersistentData, skipRegionsNotAffectingCalculation);
                    regionPersistentData.DeserializeEpilog();
                    list.Add(regionPersistentData);
                }
            }
            catch (Exception ex)
            {
                string value = "Unknown";
                if (regionPersistentData != null)
                {
                    CleanUpLastRegion(regionPersistentData);
                    if (regionPersistentData.Control != null)
                    {
                        value = ((object)regionPersistentData.Control).GetType().Name;
                    }
                }
                IRegionPersistentData regionPersistentData2 = RegionFactory.CreateFailedRegionPersistentData();
                if (regionPersistentData2 != null)
                {
                    if (region is IRegionInWhiteboardType regionInWhiteboardType)
                    {
                        regionPersistentData2.Location = regionInWhiteboardType.Location;
                    }
                    AddRegion(regionPersistentData2, skipRegionsNotAffectingCalculation);
                    ex.Data.Add("RegionType", value);
                    ex.Data.Add("RegionId", region.regionid);
                    ex.Data.Add("Tag", region.Tag);
                    ex.Data.Add("RegionIndex", regions.IndexOf(region));
                    ex.Data.Add("ShowBorder", region.ShowBorder);
                    regionPersistentData2.DeserializationException = ex;
                    list.Add(regionPersistentData2);
                }
            }
        }
        return list;
    }

    private void AddRegion(IRegionPersistentData regionPersistentData, bool skipRegionsNotAffectingCalculation)
    {
        if (skipRegionsNotAffectingCalculation && IsRegionNotAffectingCalculation(regionPersistentData.Control))
        {
            DebugTools.LogData("DeserializationStrategy.AddRegion: skipRegionsNotAffectingCalculation && NOT RegionAffectingCalculation:" + ((object)regionPersistentData.Control).GetType().Name);
            return;
        }
        if (skipRegionsNotAffectingCalculation)
        {
            DebugTools.LogData("DeserializationStrategy.AddRegion: not skipping:" + ((object)regionPersistentData.Control).GetType().Name);
        }
        AddRegion(regionPersistentData);
    }

    protected virtual void AddRegion(IRegionPersistentData regionPersistentData)
    {
        AddRegion(regionPersistentData.Control, regionPersistentData.Location);
    }

    protected virtual void CleanUpLastRegion(IRegionPersistentData regionPersistentData)
    {
        try
        {
            DeleteRegion(regionPersistentData.Control);
        }
        catch
        {
        }
    }

    [MirrorVerified("PtcControlsCoreSerializationDeserializationStrategyMechanicalDifferentialTests")]
    public virtual void DeserializeRegionProlog(UIElement region) { }

    [MirrorVerified("PtcControlsCoreSerializationDeserializationStrategyMechanicalDifferentialTests")]
    public virtual void DeserializeRegionsEpilog(XmlDocument xmlContentDocument) { }

    [MirrorVerified("PtcControlsCoreSerializationDeserializationStrategyMechanicalDifferentialTests")]
    protected DeserializationStrategy() { }
}
