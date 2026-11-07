using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Xml;
using Quire.Controls.Core;
using Quire.Controls.Core.Serialization;
using Quire.Controls;
using Quire.PersistentData;
using Quire.PersistentDataObjects;

namespace Quire.Controls.Include;

public partial class IncludeFileDeserializationStrategy : DeserializationStrategy
{
    private readonly List<UIElement> _calulatableRegions;
    private readonly IncludeWorksheetFileManager _fileManager;

    public bool CacheValid { get; set; }

    public override IList<UIElement> Regions => _calulatableRegions;

    public override IRegionFactory RegionFactory => _fileManager.RegionFactory;

    public IncludeFileDeserializationStrategy(IncludeWorksheetFileManager fileManager)
    {
        _fileManager = fileManager;
        _calulatableRegions = new List<UIElement>();
        CacheValid = true;
    }

    public override void DeserializeRegionProlog(UIElement element)
    {
        base.DeserializeRegionProlog(element);
        if (element is IMultiLevelInclusionProvider)
        {
            (element as IMultiLevelInclusionProvider).MultiLevelInclusionItem.CacheValid = CacheValid;
        }
    }

    public override void DeserializeRegionsEpilog(XmlDocument xmlDocument)
    {
        _fileManager.DeserializationEpilog(xmlDocument);
    }

    public override void AddRegion(UIElement element, Point location)
    {
        DocumentAttachedProperties.SetLocation(element, location);
        _calulatableRegions.Add(element);
    }

    public override void DeleteRegion(UIElement control)
    {
        _calulatableRegions.Remove(control);
    }

    protected override bool IsRegionNotAffectingCalculation(UIElement element)
    {
        if (element is EquationControl equationControl)
        {
            if (equationControl.EquationType != EquationTypes.Evaluation)
            {
                return equationControl.EquationType == EquationTypes.Expression;
            }
            return true;
        }
        return false;
    }

    protected override bool IsRegionNotAffectingCalculation(IRegionType regionData)
    {
        object item = regionData.Item;
        // Release IL requires ldc.i4.0 / ceq / brtrue.s via ternary == 0
        if ((cachedWorksheetRegionType.SupportedTypes.Any((Type type) => type.IsInstanceOfType(item)) ? 0 : 1) == 0)
        {
            return base.IsRegionNotAffectingCalculation(regionData);
        }
        return true;
    }
}
