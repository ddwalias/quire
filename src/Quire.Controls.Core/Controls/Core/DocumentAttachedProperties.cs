#nullable disable
using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using Quire;
using Quire.Controls;
using Quire.Controls.Core.BookmarkTags;
using Quire.Controls.Core.IORegions;
using Quire.Mirror;
using Quire.Wpf;

namespace Quire.Controls.Core;

public static partial class DocumentAttachedProperties
{
    private static readonly Type _typeOfThis = typeof(DocumentAttachedProperties);

    public static readonly DependencyProperty ZoomProperty = DependencyProperty.RegisterAttached("Zoom", typeof(double), _typeOfThis, new FrameworkPropertyMetadata(1.0, FrameworkPropertyMetadataOptions.Inherits, null, CoerceZoom));

    public static readonly DependencyProperty LocationProperty = DependencyProperty.RegisterAttached("Location", typeof(Point), typeof(DocumentAttachedProperties));

    public static readonly DependencyProperty SnapToGridProperty = DependencyProperty.RegisterAttached("SnapToGrid", typeof(bool), _typeOfThis, new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.Inherits));

    public static readonly DependencyProperty DefaultUnitSystemProperty = DependencyProperty.RegisterAttached("DefaultUnitSystem", typeof(Calculation.Core.UnitSystems), typeof(DocumentAttachedProperties));

    public static readonly DependencyProperty CustomBaseUnitSystemProperty = DependencyProperty.RegisterAttached("CustomBaseUnitSystem", typeof(Calculation.Core.UnitSystems), typeof(DocumentAttachedProperties));

    [MirrorVerified("PtcControlsCoreDocumentAttachedPropertiesMechanicalDifferentialTests")]
    public static readonly DependencyProperty DocCalculationOptionsProperty = DependencyProperty.RegisterAttached("DocCalculationOptions", typeof(CalculationOptions), _typeOfThis, new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));

    [MirrorVerified("PtcControlsCoreDocumentAttachedPropertiesMechanicalDifferentialTests")]
    public static readonly DependencyProperty DocCalculationVariablesProperty = DependencyProperty.RegisterAttached("DocCalculationVariables", typeof(CalculationVariables), _typeOfThis, new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));

    public static readonly DependencyProperty PreRecalculateAllProperty = DependencyProperty.RegisterAttached("PreRecalculateAll", typeof(bool), _typeOfThis, new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits));

    public static readonly double defaultVerticalPageGutter = WpfUtils.InchesToDips(0.25);

    public static readonly DependencyProperty VerticalPageGutterProperty = DependencyProperty.RegisterAttached("VerticalPageGutter", typeof(double), _typeOfThis, new FrameworkPropertyMetadata(defaultVerticalPageGutter, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.Inherits));

    public static readonly DependencyProperty DocumentIsActiveProperty = DependencyProperty.RegisterAttached("DocumentIsActive", typeof(bool), _typeOfThis, new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.Inherits));

    public static readonly DependencyProperty CacheWorksheetProperty = DependencyProperty.RegisterAttached("CacheWorksheet", typeof(bool), _typeOfThis, new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.Inherits));

    public static readonly DependencyProperty ShowInputOutputDesignationProperty = DependencyProperty.RegisterAttached("ShowInputOutputDesignation", typeof(bool), _typeOfThis, new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.Inherits));

    [MirrorVerified("PtcControlsCoreDocumentAttachedPropertiesMechanicalDifferentialTests")]
    public static readonly DependencyProperty InputOutputRegionManagerProperty = DependencyProperty.RegisterAttached("InputOutputRegionManager", typeof(IIORegionManager), _typeOfThis, new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));

    [MirrorVerified("PtcControlsCoreDocumentAttachedPropertiesMechanicalDifferentialTests")]
    public static readonly DependencyProperty BookmarksManagerProperty = DependencyProperty.RegisterAttached("BookmarksManager", typeof(IBookmarksManager), _typeOfThis, new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));

    [MirrorVerified("PtcControlsCoreDocumentAttachedPropertiesMechanicalDifferentialTests")]
    public static readonly DependencyProperty LabelProperty = DependencyProperty.RegisterAttached("Label", typeof(string), _typeOfThis);

    [MirrorVerified("PtcControlsCoreDocumentAttachedPropertiesMechanicalDifferentialTests")]
    public static readonly DependencyProperty FilePathProperty = DependencyProperty.RegisterAttached("FilePath", typeof(string), _typeOfThis, new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits, null, CoerceFilePath));

    [MirrorVerified("PtcControlsCoreDocumentAttachedPropertiesMechanicalDifferentialTests")]
    public static readonly DependencyProperty IsInPrintingProperty = DependencyProperty.RegisterAttached("IsInPrinting", typeof(bool), _typeOfThis, new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits));

    [MirrorVerified("PtcControlsCoreDocumentAttachedPropertiesMechanicalDifferentialTests")]
    public static readonly DependencyProperty IsInCopyProperty = DependencyProperty.RegisterAttached("IsInCopy", typeof(bool), _typeOfThis, new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits));

    [MirrorVerified("PtcControlsCoreDocumentAttachedPropertiesMechanicalDifferentialTests")]
    public static readonly DependencyProperty IsInPastingProperty = DependencyProperty.RegisterAttached("IsInPasting", typeof(bool), _typeOfThis, new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits));

    [MirrorVerified("PtcControlsCoreDocumentAttachedPropertiesMechanicalDifferentialTests")]
    public static readonly DependencyProperty CopyAsImageSettingsProperty = DependencyProperty.RegisterAttached("CopyAsImageSettings", typeof(RegionImageSettings?), _typeOfThis, new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));

    private static object CoerceZoom(DependencyObject obj, object basevalue)
    {
        return Math.Round((double)basevalue, 2);
    }

    public static void SetZoom<TEngineeringDocument>(TEngineeringDocument dObj, double value) where TEngineeringDocument : DependencyObject, IEngineeringDocument
    {
        if (dObj != null)
        {
            dObj.SetValue(ZoomProperty, value);
        }
    }

    public static double GetZoom(DependencyObject dObj)
    {
        if (dObj == null)
        {
            return (double)ZoomProperty.DefaultMetadata.DefaultValue;
        }
        return (double)dObj.GetValue(ZoomProperty);
    }

    public static Point GetLocation(DependencyObject element)
    {
        return (Point)element.GetValue(LocationProperty);
    }

    public static void SetLocation(DependencyObject element, Point value)
    {
        element.SetValue(LocationProperty, value);
    }

    public static bool GetSnapToGrid(DependencyObject element)
    {
        return (bool)element.GetValue(SnapToGridProperty);
    }

    public static void SetSnapToGrid(DependencyObject element, bool value)
    {
        element.SetValue(SnapToGridProperty, value);
    }

    public static CalculationOptions GetDocCalculationOptions(DependencyObject o)
    {
        return (CalculationOptions)o.GetValue(DocCalculationOptionsProperty);
    }

    [MirrorVerified("PtcControlsCoreDocumentAttachedPropertiesMechanicalDifferentialTests")]
    public static void SetDocCalculationOptions(DependencyObject o, CalculationOptions value) => o.SetValue(DocCalculationOptionsProperty, value);

    public static CalculationVariables GetDocCalculationVariables(DependencyObject o)
    {
        return (CalculationVariables)o.GetValue(DocCalculationVariablesProperty);
    }

    [MirrorVerified("PtcControlsCoreDocumentAttachedPropertiesMechanicalDifferentialTests")]
    public static void SetDocCalculationVariables(DependencyObject o, CalculationVariables value) => o.SetValue(DocCalculationVariablesProperty, value);

    public static bool GetPreRecalculateAll(DependencyObject o)
    {
        return (bool)o.GetValue(PreRecalculateAllProperty);
    }

    public static void SetPreRecalculateAll(DependencyObject o, bool value)
    {
        o.SetValue(PreRecalculateAllProperty, value);
    }

    public static double GetVerticalPageGutter(DependencyObject element)
    {
        return (double)element.GetValue(VerticalPageGutterProperty);
    }

    public static void SetVerticalPageGutter(DependencyObject element, double value)
    {
        element.SetValue(VerticalPageGutterProperty, value);
    }

    public static bool GetDocumentIsActive(DependencyObject doc)
    {
        return (bool)doc.GetValue(DocumentIsActiveProperty);
    }

    public static void SetDocumentIsActive(DependencyObject doc, bool activeState)
    {
        doc.SetValue(DocumentIsActiveProperty, activeState);
    }

    public static bool GetCacheWorksheet(DependencyObject o)
    {
        return (bool)o.GetValue(CacheWorksheetProperty);
    }

    public static void SetCacheWorksheet(DependencyObject o, bool value)
    {
        o.SetValue(CacheWorksheetProperty, value);
    }

    public static bool GetShowInputOutputDesignation(DependencyObject o)
    {
        return (bool)o.GetValue(ShowInputOutputDesignationProperty);
    }

    public static void SetShowInputOutputDesignation(DependencyObject o, bool value)
    {
        o.SetValue(ShowInputOutputDesignationProperty, value);
    }

    public static IIORegionManager GetInputOutputRegionManager(DependencyObject region)
    {
        return (IIORegionManager)region.GetValue(InputOutputRegionManagerProperty);
    }

    [MirrorVerified("PtcControlsCoreDocumentAttachedPropertiesMechanicalDifferentialTests")]
    public static void SetInputOutputRegionManager(DependencyObject region, IIORegionManager value) => region.SetValue(InputOutputRegionManagerProperty, value);

    public static IBookmarksManager GetBookmarksManager(DependencyObject region)
    {
        return (IBookmarksManager)region.GetValue(BookmarksManagerProperty);
    }

    [MirrorVerified("PtcControlsCoreDocumentAttachedPropertiesMechanicalDifferentialTests")]
    public static void SetBookmarksManager(DependencyObject region, IBookmarksManager value) => region.SetValue(BookmarksManagerProperty, value);

    public static string GetLabel(DependencyObject dependencyObject)
    {
        return (string)dependencyObject.GetValue(LabelProperty);
    }

    [MirrorVerified("PtcControlsCoreDocumentAttachedPropertiesMechanicalDifferentialTests")]
    public static void SetLabel(DependencyObject dependencyObject, string value) => dependencyObject.SetValue(LabelProperty, value);

    private static object CoerceFilePath(DependencyObject dependencyObject, object baseValue)
    {
        if (baseValue == null)
        {
            return baseValue;
        }
        try
        {
            return Path.GetFullPath((string)baseValue);
        }
        catch (Exception ex)
        {
            DebugTools.FailAssertion(ex.Message);
            return baseValue;
        }
    }

    public static string GetFilePath(DependencyObject o)
    {
        return (string)o.GetValue(FilePathProperty);
    }

    [MirrorVerified("PtcControlsCoreDocumentAttachedPropertiesMechanicalDifferentialTests")]
    public static void SetFilePath(DependencyObject o, string value) => o.SetValue(FilePathProperty, value);

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public static bool GetIsInPrinting(DependencyObject o)
    {
        return (bool)o.GetValue(IsInPrintingProperty);
    }

    public static void SetIsInPrinting(DependencyObject o, bool value)
    {
        o.SetValue(IsInPrintingProperty, value);
    }

    [MirrorVerified("PtcControlsCoreDocumentAttachedPropertiesMechanicalDifferentialTests")]
    public static void ClearIsInPrinting(DependencyObject o) => o.ClearValue(IsInPrintingProperty);

    public static bool GetIsInCopyNonSerializable(DependencyObject o)
    {
        return (bool)o.GetValue(IsInCopyProperty);
    }

    public static void SetIsInCopy(DependencyObject o, bool value)
    {
        o.SetValue(IsInCopyProperty, value);
    }

    [MirrorVerified("PtcControlsCoreDocumentAttachedPropertiesMechanicalDifferentialTests")]
    public static void ClearIsInCopy(DependencyObject o) => o.ClearValue(IsInCopyProperty);

    public static bool GetIsInPasting(DependencyObject o)
    {
        return (bool)o.GetValue(IsInPastingProperty);
    }

    public static void SetIsInPasting(DependencyObject o, bool value)
    {
        o.SetValue(IsInPastingProperty, value);
    }

    [MirrorVerified("PtcControlsCoreDocumentAttachedPropertiesMechanicalDifferentialTests")]
    public static void ClearIsInPasting(DependencyObject o) => o.ClearValue(IsInPastingProperty);

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public static RegionImageSettings? GetCopyAsImageSettings(DependencyObject target)
    {
        return (RegionImageSettings?)target.GetValue(CopyAsImageSettingsProperty);
    }

    public static void SetCopyAsImageSettings(DependencyObject target, RegionImageSettings? value)
    {
        target.SetValue(CopyAsImageSettingsProperty, value);
    }

    [MirrorVerified("PtcControlsCoreDocumentAttachedPropertiesMechanicalDifferentialTests")]
    public static void ClearCopyAsImageSettings(DependencyObject target) => target.ClearValue(CopyAsImageSettingsProperty);
}
