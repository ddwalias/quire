#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Quire.Controls.Core.IORegions;
using Quire.Controls.Core.Protection;
using Quire.Controls.Core.Whiteboard;
using Quire.Core;
using Quire.Mirror;
using Quire.Wpf;

namespace Quire.Controls.Core;

public static partial class AttachedProperties
{
    private static readonly Type _typeOfThis;

    public static readonly DependencyProperty RegisterCommandBindingsProperty;

    public static DependencyProperty PageElementXamlHelperProperty;

    public const string PlotBackgroundTypeKey = "PlotBackgroundType";

    public static readonly DependencyProperty PlotBackgroundTypeProperty;

    public const string PlotCameraTypeKey = "PlotCameraType";

    public static readonly DependencyProperty PlotCameraTypeProperty;

    public static DependencyProperty CalcIsDisabledProperty;

    public static DependencyProperty ResultMatrixFormatProperty;

    public static DependencyProperty DisplayNumberFormatProperty;

    public static DependencyProperty LabeledIdFormatProperty;

    public static DependencyProperty ProhibitMouseClickProperty;

    public static DependencyProperty ProhibitSizeChangedEventHandlerProperty;

    [MirrorVerified("PtcControlsCoreAttachedPropertiesMechanicalDifferentialTests")]
    public static readonly DependencyProperty CanActivateProperty;

    [MirrorVerified("PtcControlsCoreAttachedPropertiesMechanicalDifferentialTests")]
    public static readonly DependencyProperty HandlesContextMenuProperty;

    public static readonly DependencyProperty SupportsInputsOutputsProperty;

    public static readonly DependencyProperty IsOnVisiblePageProperty;

    public static readonly DependencyProperty ShouldAssertOnLocationProperty;

    public static readonly DependencyProperty PreviousLocationLeftProperty;

    public static readonly DependencyProperty PreviousLocationTopProperty;

    public static readonly DependencyProperty ContainedInCollapsedWhiteboardProperty;

    public static DependencyProperty IsBringToViewProperty;

    public static DependencyProperty WarningDisplayFilterProperty;

    static AttachedProperties()
    {
        _typeOfThis = typeof(AttachedProperties);
        RegisterCommandBindingsProperty = DependencyProperty.RegisterAttached("RegisterCommandBindings", typeof(CommandBindingCollection), _typeOfThis, new PropertyMetadata(null, OnRegisterCommandBindingChanged));
        PageElementXamlHelperProperty = DependencyProperty.RegisterAttached("PageElementXamlHelper", typeof(IPageElementXamlHelper), typeof(UIElement), new PropertyMetadata(new DefaultPageElementXamlHelper()));
        PlotBackgroundTypeProperty = DependencyProperty.RegisterAttached("PlotBackgroundType", typeof(BackgroundTypeE), _typeOfThis, new PropertyMetadata(BackgroundTypeE.Undefined));
        PlotCameraTypeProperty = DependencyProperty.RegisterAttached("PlotCameraType", typeof(CameraType3D), _typeOfThis, new PropertyMetadata(CameraType3D.Perspective));
        CalcIsDisabledProperty = DependencyProperty.RegisterAttached("CalcIsDisabled", typeof(bool), _typeOfThis, new FrameworkPropertyMetadata(false));
        ResultMatrixFormatProperty = DependencyProperty.RegisterAttached("ResultMatrixFormat", typeof(MatrixFormat), _typeOfThis, new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));
        DisplayNumberFormatProperty = DependencyProperty.RegisterAttached("DisplayNumberFormat", typeof(NumberFormat), _typeOfThis, new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));
        LabeledIdFormatProperty = DependencyProperty.RegisterAttached("LabeledIdFormat", typeof(IDictionary<IdLabels, ExtendedFont>), _typeOfThis, new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits, HandleLabeledIdFormatChanged));
        ProhibitMouseClickProperty = DependencyProperty.RegisterAttached("ProhibitMouseClick", typeof(bool), _typeOfThis, new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits));
        ProhibitSizeChangedEventHandlerProperty = DependencyProperty.RegisterAttached("ProhibitSizeChangedEventHandler", typeof(bool), _typeOfThis, new FrameworkPropertyMetadata(false));
        CanActivateProperty = DependencyProperty.RegisterAttached("CanActivate", typeof(bool), _typeOfThis, new FrameworkPropertyMetadata(true, OnCanActivateChanged, CanActivateCoerceValue));
        HandlesContextMenuProperty = DependencyProperty.RegisterAttached("HandlesContextMenu", typeof(bool), _typeOfThis, new FrameworkPropertyMetadata(false));
        SupportsInputsOutputsProperty = DependencyProperty.RegisterAttached("SupportsInputsOutputs", typeof(bool), _typeOfThis, new FrameworkPropertyMetadata(true, SupportsInputsOutputsPropertyChangedHandler));
        IsOnVisiblePageProperty = DependencyProperty.RegisterAttached("IsOnVisiblePage", typeof(bool), _typeOfThis, new FrameworkPropertyMetadata(false, IsOnVisiblePagePropertyChangedHandler));
        ShouldAssertOnLocationProperty = DependencyProperty.RegisterAttached("ShouldAssertOnLocation", typeof(bool), _typeOfThis, new FrameworkPropertyMetadata(true));
        PreviousLocationLeftProperty = DependencyProperty.RegisterAttached("PreviousLocationLeft", typeof(double), _typeOfThis, new PropertyMetadata(double.NaN));
        PreviousLocationTopProperty = DependencyProperty.RegisterAttached("PreviousLocationTop", typeof(double), _typeOfThis, new PropertyMetadata(double.NaN));
        ContainedInCollapsedWhiteboardProperty = DependencyProperty.RegisterAttached("ContainedInCollapsedWhiteboard", typeof(bool), _typeOfThis, new FrameworkPropertyMetadata(false, ContainedInCollapsedWhiteboardPropertyChangedHandler));
        IsBringToViewProperty = DependencyProperty.RegisterAttached("IsBringToView", typeof(bool), _typeOfThis, new PropertyMetadata(true));
        WarningDisplayFilterProperty = DependencyProperty.RegisterAttached("WarningDisplayFilter", typeof(WarningsTypesToShow), _typeOfThis, new FrameworkPropertyMetadata(WarningsTypesToShow.All, FrameworkPropertyMetadataOptions.Inherits));
        PageElementXamlHelperProperty.OverrideMetadata(typeof(FrameworkElement), new PropertyMetadata(new FrameworkElementXamlHelper()));
    }

    private static void OnRegisterCommandBindingChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        if (sender is UIElement uIElement && e.NewValue is CommandBindingCollection collection)
        {
            uIElement.CommandBindings.AddRange(collection);
        }
    }

    private static IPageElementXamlHelper GetPageElementXamlHelper(DependencyObject element)
    {
        if (element != null)
        {
            return (IPageElementXamlHelper)element.GetValue(PageElementXamlHelperProperty);
        }
        return null;
    }

    public static BackgroundTypeE GetPlotBackgroundType(DependencyObject depObj)
    {
        return (BackgroundTypeE)depObj.GetValue(PlotBackgroundTypeProperty);
    }

    public static void SetPlotBackgroundType(DependencyObject depObj, BackgroundTypeE value)
    {
        depObj.SetValue(PlotBackgroundTypeProperty, value);
    }

    public static CameraType3D GetPlotCameraType(DependencyObject depObj)
    {
        return (CameraType3D)depObj.GetValue(PlotCameraTypeProperty);
    }

    public static void SetPlotCameraType(DependencyObject depObj, BackgroundTypeE value)
    {
        depObj.SetValue(PlotCameraTypeProperty, value);
    }

    public static bool GetCalcIsDisabled(DependencyObject dObj)
    {
        return (bool)dObj.GetValue(CalcIsDisabledProperty);
    }

    public static void SetCalcIsDisabled(DependencyObject dObj, bool value)
    {
        dObj.SetValue(CalcIsDisabledProperty, value);
    }

    private static void HandleLabeledIdFormatChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
    {
        if (!(d is IIdLabelOwner idLabelOwner))
        {
            return;
        }
        IDictionary<IdLabels, ExtendedFont> dictionary = args.OldValue as IDictionary<IdLabels, ExtendedFont>;
        IDictionary<IdLabels, ExtendedFont> dictionary2 = args.NewValue as IDictionary<IdLabels, ExtendedFont>;
        if (dictionary != null)
        {
            foreach (KeyValuePair<IdLabels, ExtendedFont> item in dictionary)
            {
                item.Value.FormatChanged -= idLabelOwner.HandleLabeledIdFormatChanged;
            }
        }
        if (dictionary2 != null)
        {
            foreach (KeyValuePair<IdLabels, ExtendedFont> item2 in dictionary2)
            {
                item2.Value.FormatChanged += idLabelOwner.HandleLabeledIdFormatChanged;
            }
        }
        idLabelOwner.HandleLabeledIdFormatDictionaryChanged(args);
    }

    public static bool GetProhibitMouseClick(DependencyObject depObj)
    {
        if (DebugTools.AssertNotNull(depObj, null).Failed)
        {
            return false;
        }
        return (bool)depObj.GetValue(ProhibitMouseClickProperty);
    }

    public static bool GetProhibitSizeChangedEventHandler(DependencyObject depObj)
    {
        if (DebugTools.AssertNotNull(depObj, null).Failed)
        {
            return false;
        }
        return (bool)depObj.GetValue(ProhibitSizeChangedEventHandlerProperty);
    }

    public static void SetProhibitSizeChangedEventHandler(DependencyObject depObj, bool prohibit)
    {
        if (DebugTools.AssertNotNull(depObj, null).Failed)
        {
            return;
        }
        depObj.SetValue(ProhibitSizeChangedEventHandlerProperty, prohibit);
    }

    public static void ClearProhibitSizeChangedEventHandler(DependencyObject depObj)
    {
        if (DebugTools.AssertNotNull(depObj, null).Failed)
        {
            return;
        }
        depObj.ClearValue(ProhibitSizeChangedEventHandlerProperty);
    }

    private static object CanActivateCoerceValue(DependencyObject d, object value)
    {
        if (d is IProtectableControl protectableControl && protectableControl.ProtectionLevel.IsActivationProtected())
        {
            return false;
        }
        return value;
    }

    public static bool GetCanActivate(DependencyObject dObj)
    {
        return (bool)dObj.GetValue(CanActivateProperty);
    }

    public static void SetCanActivate(DependencyObject dObj, bool value)
    {
        dObj.SetValue(CanActivateProperty, value);
    }

    [MirrorVerified("PtcControlsCoreAttachedPropertiesMechanicalDifferentialTests")]
    public static void ClearCanActivate(DependencyObject dObj) => dObj.ClearValue(CanActivateProperty);

    public static bool GetHandlesContextMenu(DependencyObject dObj)
    {
        return (bool)dObj.GetValue(HandlesContextMenuProperty);
    }

    public static void SetHandlesContextMenu(DependencyObject dObj, bool value)
    {
        dObj.SetValue(HandlesContextMenuProperty, value);
    }

    [MirrorVerified("PtcControlsCoreAttachedPropertiesMechanicalDifferentialTests")]
    public static void ClearHandlesContextMenu(DependencyObject dObj) => dObj.ClearValue(HandlesContextMenuProperty);

    private static void SupportsInputsOutputsPropertyChangedHandler(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
        bool flag = (bool)args.NewValue;
        if (obj is IWhiteboardItemContainer whiteboardItemContainer)
        {
            whiteboardItemContainer.WhiteboardItems.ParentSupportsInputsOutputs = flag;
        }
        if (!flag)
        {
            IOTag.ClearIOTag(obj);
        }
    }

    public static bool GetSupportsInputsOutputs(UIElement elem)
    {
        if (DebugTools.AssertNotNull(elem, null).Passed)
        {
            return (bool)((DependencyObject)elem).GetValue(SupportsInputsOutputsProperty);
        }
        return false;
    }

    public static void SetSupportsInputsOutputs(UIElement elem, bool value)
    {
        if (DebugTools.AssertNotNull(elem, null).Passed)
        {
            ((DependencyObject)elem).SetValue(SupportsInputsOutputsProperty, value);
        }
    }

    public static void ClearSupportsInputsOutputs(UIElement elem)
    {
        if (DebugTools.AssertNotNull(elem, null).Passed)
        {
            ((DependencyObject)elem).ClearValue(SupportsInputsOutputsProperty);
        }
    }

    private static void IsOnVisiblePagePropertyChangedHandler(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
        if (obj is IWhiteboardItemContainer whiteboardItemContainer)
        {
            whiteboardItemContainer.WhiteboardItems.ParentIsOnVisiblePage = (bool)args.NewValue;
        }
        IMaybeHasPointThatSnapsToPixel maybeHasPointThatSnapsToPixel = obj as IMaybeHasPointThatSnapsToPixel;
        ISnappableItemHelper snappableItemHelper = (maybeHasPointThatSnapsToPixel == null) ? null : (maybeHasPointThatSnapsToPixel.SnappingHelper as ISnappableItemHelper);
        if (snappableItemHelper != null)
        {
            DebugTools.DevAssert(snappableItemHelper.IsPageDescendant, null);
        }
    }

    public static bool GetIsOnVisiblePage(UIElement elem)
    {
        if (DebugTools.AssertNotNull(elem, null).Passed)
        {
            return (bool)((DependencyObject)elem).GetValue(IsOnVisiblePageProperty);
        }
        return false;
    }

    public static void SetIsOnVisiblePage(UIElement elem, bool value)
    {
        if (DebugTools.AssertNotNull(elem, null).Passed)
        {
            ((DependencyObject)elem).SetValue(IsOnVisiblePageProperty, value);
        }
    }

    public static void ClearIsOnVisiblePage(UIElement elem)
    {
        if (DebugTools.AssertNotNull(elem, null).Passed)
        {
            ((DependencyObject)elem).ClearValue(IsOnVisiblePageProperty);
        }
    }

    public static bool GetShouldAssertOnLocation(FrameworkElement elem)
    {
        if (DebugTools.AssertNotNull(elem, null).Passed)
        {
            return (bool)((DependencyObject)elem).GetValue(ShouldAssertOnLocationProperty);
        }
        return false;
    }

    public static void SetShouldAssertOnLocation(FrameworkElement elem, bool value)
    {
        if (DebugTools.AssertNotNull(elem, null).Passed)
        {
            ((DependencyObject)elem).SetValue(ShouldAssertOnLocationProperty, value);
        }
    }

    public static void ClearShouldAssertOnLocation(FrameworkElement elem)
    {
        if (DebugTools.AssertNotNull(elem, null).Passed)
        {
            ((DependencyObject)elem).ClearValue(ShouldAssertOnLocationProperty);
        }
    }

    public static double GetPreviousLocationLeft(DependencyObject dependencyObject)
    {
        return (double)dependencyObject.GetValue(PreviousLocationLeftProperty);
    }

    public static void SetPreviousLocationLeft(DependencyObject dependencyObject, double value)
    {
        dependencyObject.SetValue(PreviousLocationLeftProperty, value);
    }

    public static double GetPreviousLocationTop(DependencyObject dependencyObject)
    {
        return (double)dependencyObject.GetValue(PreviousLocationTopProperty);
    }

    public static void SetPreviousLocationTop(DependencyObject dependencyObject, double value)
    {
        dependencyObject.SetValue(PreviousLocationTopProperty, value);
    }

    public static Point GetPreviousLocation(DependencyObject dependencyObject)
    {
        return new Point(GetPreviousLocationLeft(dependencyObject), GetPreviousLocationTop(dependencyObject));
    }

    public static void ClearPreviousLocation(DependencyObject dependencyObject)
    {
        dependencyObject.ClearValue(PreviousLocationLeftProperty);
        dependencyObject.ClearValue(PreviousLocationTopProperty);
    }

    public static bool PreviousLocationHasLocalValue(DependencyObject dependencyObject)
    {
        if (dependencyObject.HasLocalValue(PreviousLocationLeftProperty))
        {
            return dependencyObject.HasLocalValue(PreviousLocationTopProperty);
        }
        return false;
    }

    public static bool GetContainedInCollapsedWhiteboard(DependencyObject obj)
    {
        if (DebugTools.AssertNotNull(obj, null).Passed)
        {
            return (bool)obj.GetValue(ContainedInCollapsedWhiteboardProperty);
        }
        return false;
    }

    public static void SetContainedInCollapsedWhiteboard(DependencyObject obj, bool value)
    {
        if (DebugTools.AssertNotNull(obj, null).Passed)
        {
            obj.SetValue(ContainedInCollapsedWhiteboardProperty, value);
        }
    }

    public static void ClearContainedInCollapsedWhiteboard(DependencyObject obj)
    {
        if (DebugTools.AssertNotNull(obj, null).Passed)
        {
            obj.ClearValue(ContainedInCollapsedWhiteboardProperty);
        }
    }

    private static void ContainedInCollapsedWhiteboardPropertyChangedHandler(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
        if (obj is IWhiteboardItemContainer whiteboardItemContainer)
        {
            whiteboardItemContainer.WhiteboardItems.ContainedInCollapsedWhiteboard = (bool)args.NewValue;
        }
    }

    public static void SuppressBringIntoViewIfNeededAndExecute(DependencyObject depObj, Action action, bool suppressBringToView)
    {
        bool isBringToView = GetIsBringToView(depObj);
        try
        {
            if (suppressBringToView)
            {
                SetIsBringToView(depObj, value: false);
            }
            action();
        }
        finally
        {
            if (suppressBringToView)
            {
                SetIsBringToView(depObj, isBringToView);
            }
        }
    }

    public static WarningsTypesToShow GetWarningDisplayFilter(DependencyObject dObj)
    {
        return (WarningsTypesToShow)dObj.GetValue(WarningDisplayFilterProperty);
    }

    public static void SetWarningDisplayFilter(DependencyObject dObj, WarningsTypesToShow value)
    {
        dObj.SetValue(WarningDisplayFilterProperty, value);
    }
}
