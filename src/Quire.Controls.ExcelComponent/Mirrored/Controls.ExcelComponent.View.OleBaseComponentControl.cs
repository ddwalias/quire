using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Quire.Controls.Core;
using Quire.Controls.Internal;
using Quire.Wpf;

using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using Quire.Calculation.Core;
using Quire.Controls.ExcelComponent.ViewModel;
using Quire.Controls.ExcelComponent.Model;
using Quire.Controls.ExcelComponent.View;
using Quire.Controls.Mathboard;
using Quire.Controls.Core.Ole;
using Quire.Controls.Whiteboard;
namespace Quire.Controls.ExcelComponent.View;

public abstract partial class OleBaseComponentControl
{
    public static double GetItemHeight(DependencyObject whiteboardItem)
    {
        return (double)whiteboardItem.GetValue(FrameworkElement.HeightProperty);
    }

    public static void SetItemHeight(DependencyObject whiteboardItem, double value)
    {
        whiteboardItem.SetValue(FrameworkElement.HeightProperty, (object)value);
    }

    public static double GetOleComponentLocationLeft(DependencyObject whiteboardItem)
    {
        return (double)whiteboardItem.GetValue(OleComponentLocationLeftProperty);
    }

    public static void SetOleComponentLocationLeft(DependencyObject whiteboardItem, double value)
    {
        whiteboardItem.SetValue(OleComponentLocationLeftProperty, (object)value);
    }

    public Size CoerceNewSize(Size newSize, global::Quire.Controls.Core.IResizeData resizeData)
    {
        return newSize;
    }

    public override string GetBlockEquationTypeAttributeName(global::Quire.Controls.Core.IWhiteboardItem item)
    {
        return string.Empty;
    }

    public static double GetItemWidth(DependencyObject whiteboardItem)
    {
        return (double)whiteboardItem.GetValue(FrameworkElement.WidthProperty);
    }

    public static void SetItemWidth(DependencyObject whiteboardItem, double value)
    {
        whiteboardItem.SetValue(FrameworkElement.WidthProperty, (object)value);
    }

    public static double GetOleComponentLocationTop(DependencyObject whiteboardItem)
    {
        return (double)whiteboardItem.GetValue(OleComponentLocationTopProperty);
    }

    public static void SetOleComponentLocationTop(DependencyObject whiteboardItem, double value)
    {
        whiteboardItem.SetValue(OleComponentLocationTopProperty, (object)value);
    }

    private static void HandleOleComponentLocationTopChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
        obj.SetValue(AttachedProperties.PreviousLocationTopProperty, args.NewValue);
    }

    private static void HandleOleComponentLocationLeftChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
        obj.SetValue(AttachedProperties.PreviousLocationLeftProperty, args.NewValue);
    }

    public void UpdateAllErrors()
    {
        _oleBaseModel.UpdateAllErrors();
    }

    private void FireCalcErrorChanged()
    {
        FirePropertyChanged(global::Quire.Calculation.Core.ErrorChangedEventArgs.GetArgs());
    }

    public void SetPrintingZoom(bool set, global::System.Nullable<double> newPrintingZoom)
    {
        OleContainerItem.SetPrintingZoom(set, newPrintingZoom);
    }

    public global::Quire.Controls.Core.IResizeData StoreResizeData(global::System.Windows.Size oldSize, global::System.Windows.Size newSize)
    {
        return _oleBaseViewModel.StoreResizeData(oldSize, newSize);
    }

    public void RestoreResizeData(global::Quire.Controls.Core.IResizeData resizeData)
    {
        _oleBaseViewModel.RestoreResizeData(resizeData);
    }

    private static void OnOleComponentLoaded(object sender, RoutedEventArgs e)
    {
        OleBaseComponentControl oleBaseComponentControl = sender as OleBaseComponentControl;
        if (DebugTools.AssertNotNull(oleBaseComponentControl, null).Failed)
        {
            return;
        }
        oleBaseComponentControl.SizeChanged += oleBaseComponentControl._oleBaseViewModel.OnSizeChanged;
        oleBaseComponentControl.Loaded -= OnOleComponentLoaded;
    }

    private static void IsResizingChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
        OleBaseComponentControl oleBaseComponentControl = sender as OleBaseComponentControl;
        if (DebugTools.AssertNotNull(oleBaseComponentControl, null).Failed)
        {
            return;
        }
        oleBaseComponentControl._oleBaseViewModel.IsResizing = (bool)args.NewValue;
    }

    private static void ErrorChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
        OleBaseComponentControl oleBaseComponentControl = obj as OleBaseComponentControl;
        if (DebugTools.AssertNotNull(oleBaseComponentControl, null).Failed)
        {
            return;
        }
        oleBaseComponentControl.ErrorChanged();
    }

    protected abstract IOleContainerItem CreateOleContainerItem();

    public IOleContainerItem OleContainerItem => _oleBaseViewModel.OleContainerItem;

    public bool IsInputSectionVisible
    {
        get
        {
            return _oleBaseViewModel.IsInputSectionVisible;
        }
        set
        {
            _oleBaseViewModel.IsInputSectionVisible = value;
        }
    }

    public bool IsOutputSectionVisible
    {
        get
        {
            return _oleBaseViewModel.IsOutputSectionVisible;
        }
        set
        {
            _oleBaseViewModel.IsOutputSectionVisible = value;
        }
    }

    public Size ContentSizeForXamlSerialization
    {
        get
        {
            return ViewModel.ContentSize;
        }
        set
        {
            ViewModel.ContentSize = value;
        }
    }

    public bool OutputSectionExists => _oleBaseViewModel.OutputSectionExists;

    public double OleContainerItemHeight
    {
        get
        {
            Size oleContainerItemSize = _oleBaseViewModel.OleContainerItemSize;
            return oleContainerItemSize.Height;
        }
    }

    public double OleContainerItemWidth
    {
        get
        {
            Size oleContainerItemSize = _oleBaseViewModel.OleContainerItemSize;
            return oleContainerItemSize.Width;
        }
    }

    public double InputsIndicatorTop => _oleBaseViewModel.SectionIndicators[0].Top;

    public double InputsIndicatorBottom => InputsIndicatorTop + _oleBaseViewModel.SectionIndicators[0].Height;

    public double OutputsIndicatorTop => _oleBaseViewModel.SectionIndicators[1].Top;

    public double OutputsIndicatorBottom => OutputsIndicatorTop + _oleBaseViewModel.SectionIndicators[1].Height;

    public IEnumerable<EquationControl> Inputs => _oleBaseModel.Inputs.Cast<EquationControl>();

    public IEnumerable<EquationControl> Outputs => _oleBaseModel.Outputs.Cast<EquationControl>();

    public sealed override EngineResult EngineResult
    {
        get
        {
            return base.EngineResult;
        }
        set
        {
            base.EngineResult = value;
        }
    }

    public EngineResult EngineResultForSerialization
    {
        get
        {
            EngineError error = null;
            if (Error != null && Error.ErrorSource == this && !Error.IsRuntime)
            {
                error = Error.Clone();
                ResultManager.ErrorHandlesToSerialNumber(error, Rl);
            }
            return new EngineResult(EngineResult, error);
        }
        set
        {
            EngineResult = value;
        }
    }

    public override EngineError Error => (EngineError)((DependencyObject)this).GetValue(ErrorProperty);

    public bool IsPrintingZoomSet => OleContainerItem.IsPrintingZoomSet;

    public IEnumerable<NodeMessageAdorner> NodeMessageAdorners
    {
        get
        {
            List<NodeMessageAdorner> list = new List<NodeMessageAdorner>();
            if (_errorAdorner != null)
            {
                list.Add(_errorAdorner);
            }
            return list;
        }
    }

    public bool IsOleOpen => OleContainerItem.IsOutOfPlaceActive;

    public IDataExchangeModel DataExchangeModel => OleContainerItem.Component;

    public int? ComponentHandle
    {
        get
        {
            return _oleBaseModel.ComponentId;
        }
        set
        {
            if (!value.HasValue)
            {
                _oleBaseModel.ComponentId = 0;
                OleContainerItem.StartOleCloseOperation();
            }
            else
            {
                _oleBaseModel.ComponentId = value.Value;
                OleContainerItem.AbortOleCloseOperation();
            }
        }
    }

    public int ComponentCookieMultiThreaded
    {
        get
        {
            lock (_componentCookieLock)
            {
                return _componentCookie;
            }
        }
    }

    public SectionIndicatorInfo InputSectionIndicator
    {
        get => _oleBaseViewModel.InputSectionIndicator;
        set => _oleBaseViewModel.InputSectionIndicator = value;
    }

    public SectionIndicatorInfo OutputSectionIndicator
    {
        get => _oleBaseViewModel.OutputSectionIndicator;
        set => _oleBaseViewModel.OutputSectionIndicator = value;
    }

    public override void SetCalculatingStatus(CalculatingStatus computingStatus)
    {
        base.SetCalculatingStatus(computingStatus);
        ((DependencyObject)this).SetValue(CalculatingStatusProperty, (object)computingStatus);
    }

    public override void SetSynchronizationStatus(SynchronizationStatus syncStatus)
    {
        base.SetSynchronizationStatus(syncStatus);
        ((DependencyObject)this).SetValue(SynchronizationStatusProperty, (object)syncStatus);
    }

    protected void InsertNewOleComponentExpression(string expression, bool navigateToEnd)
    {
        OleBaseComponentCommands.NewOleComponentExpression.Execute(expression, this);
        if (navigateToEnd)
        {
            IEquationControl equationControl = (IEquationControl)ActiveItem;
            if (equationControl != null)
            {
                WhiteboardItemManager.ExecuteOnLoad(equationControl, equationControl.NavigateRegionEnd);
            }
        }
    }

    protected void CreateAndInitOleContainerItem()
    {
        IOleContainerItem oleContainerItem = CreateOleContainerItem();
        oleContainerItem.CreateWindowlessOle();
        _oleBaseViewModel.AddItemAtLocation(oleContainerItem, default);
        _oleBaseViewModel.SetInitialSize();
    }

    protected override void OnContextMenuOpening(ContextMenuEventArgs e)
    {
        base.OnContextMenuOpening(e);
        WpfUtils.SetMostRecentInputDeviceToKeyboardDevice();
    }

    protected override void ViewModelPropertyChangedHandler(object sender, PropertyChangedEventArgs e)
    {
        base.ViewModelPropertyChangedHandler(sender, e);
        if (e.PropertyName == "IsOutputSectionVisible" || e.PropertyName == "IsInputSectionVisible")
        {
            AttachedProperties.SetHandlesContextMenu(this, _oleBaseViewModel.IsInputSectionVisible || _oleBaseViewModel.IsOutputSectionVisible);
            FirePropertyChanged(e);
        }
    }

    protected override Size MeasureOverride(Size constraint)
    {
        return _oleBaseViewModel.Measure(base.MeasureOverride(constraint));
    }

    private void ErrorChanged()
    {
        if (Error != null)
        {
            if (Error.ErrorSource == null)
            {
                Error.ErrorSource = this;
            }
            else
            {
                DebugTools.DevAssert(Error.ErrorSource is IOleBaseComponentCalculatableWrapper, null);
            }
        }
        _oleBaseViewModel.IndicateHiddenErrors();
        UpdateErrorStateOnComponent();
        FireCalcErrorChanged();
    }

    private void UpdateErrorStateOnComponent()
    {
        ClearErrorOnComponent();
        if (Error != null && !_oleBaseModel.IsErrorFromChild)
        {
            string resourceString = Error.ResourceString;
            if (DebugTools.AssertNotNull(resourceString, null).Passed)
            {
                _errorFrame = new RubberBandHighlighter(this, new List<UIElement>())
                {
                    Color = Colors.Transparent,
                    CornerStyle = HighlightCornerStyle.RoundedWithBorder
                };
                _errorAdorner = new NodeMessageAdorner(this, resourceString, NodeMessageAdornerType.Error);
            }
        }
    }

    private void ClearErrorOnComponent()
    {
        if (_errorAdorner != null)
        {
            _errorAdorner.Remove();
            _errorAdorner = null;
        }
        if (_errorFrame != null)
        {
            _errorFrame.Remove();
            _errorFrame = null;
        }
    }

    public override void SetError(EngineError error)
    {
        _oleBaseModel.SetError(error);
    }

    public void CloseOle()
    {
        if (IsOleOpen)
        {
            OleContainerItem.StartOleCloseOperation();
        }
    }

    public int IncrementComponentCookieMultiThreaded(bool isAutoCalc = false)
    {
        lock (_componentCookieLock)
        {
            return ++_componentCookie;
        }
    }

    public void ReplaceCalculatablesWithWrappersInResultManager()
    {
        foreach (IOleBaseComponentCalculatableWrapper calculatableWrapper in _oleBaseModel.CalculatableWrappers)
        {
            ResultManager.Instance.ReplaceCalculatable(calculatableWrapper.WrappedCalculatable, calculatableWrapper);
        }
    }

    public void ReplaceCalculatablesAndResultWithWrappersInResultManager()
    {
        foreach (IOleBaseComponentCalculatableWrapper calculatableWrapper in _oleBaseModel.CalculatableWrappers)
        {
            ResultManager.Instance.ReplaceCalculatableAndResult(calculatableWrapper.WrappedCalculatable, calculatableWrapper);
        }
    }
}
