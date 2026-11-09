using System;
using System.Windows;
using System.Xml;

namespace Quire.Controls;

internal partial class SolveBlockSerializationStrategy
{
    protected readonly SolveBlockControl _control;
    private readonly global::Quire.DelegateFunction0<global::Quire.PersistentDataObjects.IRegionType> _regionCreator;

    public SolveBlockSerializationStrategy(
        SolveBlockControl control,
        global::Quire.DelegateFunction0<global::Quire.PersistentDataObjects.IRegionType> regionCreator)
        : base(control)
    {
        _control = control;
        _regionCreator = regionCreator;
    }

    protected override global::Quire.PersistentDataObjects.IRegionInWhiteboardType ConvertToRegionPersistantData(
        global::Quire.Serialization.IS11NProvider s11NProvider,
        global::Quire.Controls.Core.IWhiteboardItem item)
    {
        if (!(_control.RegionFactory.ConvertToIRegionType(
                _regionCreator,
                s11NProvider,
                item as UIElement) is global::Quire.PersistentDataObjects.solveBlockRegionType region))
        {
            global::Quire.DebugTools.DevAssert(false, null);
            return null;
        }

        region.Location = GetLocation(item);
        string attributeName = _control.GetBlockEquationTypeAttributeName(item);
        if (!string.IsNullOrEmpty(attributeName) &&
            global::Quire.DebugTools.DevAssert(
                Enum.TryParse<global::Quire.PersistentDataObjects.solveBlockRegionTypeSolveblockcategory>(
                    attributeName, false, out var category),
                "failed to parse {0}",
                attributeName).Passed)
        {
            region.solveblockcategory = category;
            region.solveblockcategorySpecified = true;
        }

        return region;
    }

    public override void SerializeRegionsEpilog(XmlDocument xmlDocument)
    {
        _control.ReplaceCalculatablesAndResultWithWrappersInResultManager();
    }
}
