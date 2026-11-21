#nullable disable
#pragma warning disable

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Quire;
using Quire.Calculation.Core;
using Quire.Controls;
using Quire.Controls.Core;
using Quire.Controls.Core.Serialization;
using Quire.Controls.Core.Whiteboard;
using Quire.Controls.ExcelComponent.Model;
using Quire.Controls.ExcelComponent.ViewModel;
using Quire.Controls.Internal;
using Quire.Controls.Mathboard;
using Quire.Wpf;

namespace Quire.Controls.ExcelComponent.View;

public abstract partial class OleBaseComponentControl
{
    public static DependencyProperty OleComponentLocationTopProperty;

    public static DependencyProperty OleComponentLocationLeftProperty;

    public static readonly DependencyProperty CalculatingStatusProperty;

    public static readonly DependencyProperty SynchronizationStatusProperty;

    private static void HandleOleComponentLocationTopChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
        obj.SetValue(AttachedProperties.PreviousLocationTopProperty, args.NewValue);
    }

    private static void HandleOleComponentLocationLeftChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
        obj.SetValue(AttachedProperties.PreviousLocationLeftProperty, args.NewValue);
    }

    public static double GetOleComponentLocationTop(DependencyObject whiteboardItem)
    {
        return (double)whiteboardItem.GetValue(OleComponentLocationTopProperty);
    }

    public static void SetOleComponentLocationTop(DependencyObject whiteboardItem, double value)
    {
        whiteboardItem.SetValue(OleComponentLocationTopProperty, (object)value);
    }

    public static double GetOleComponentLocationLeft(DependencyObject whiteboardItem)
    {
        return (double)whiteboardItem.GetValue(OleComponentLocationLeftProperty);
    }

    public static void SetOleComponentLocationLeft(DependencyObject whiteboardItem, double value)
    {
        whiteboardItem.SetValue(OleComponentLocationLeftProperty, (object)value);
    }

    public static void SetItemWidth(DependencyObject whiteboardItem, double value)
    {
        whiteboardItem.SetValue(FrameworkElement.WidthProperty, (object)value);
    }

    public static double GetItemWidth(DependencyObject whiteboardItem)
    {
        return (double)whiteboardItem.GetValue(FrameworkElement.WidthProperty);
    }

    public static void SetItemHeight(DependencyObject whiteboardItem, double value)
    {
        whiteboardItem.SetValue(FrameworkElement.HeightProperty, (object)value);
    }

    public static double GetItemHeight(DependencyObject whiteboardItem)
    {
        return (double)whiteboardItem.GetValue(FrameworkElement.HeightProperty);
    }

    public override void SetCalculatingStatus(CalculatingStatus computingStatus)
    {
        base.SetCalculatingStatus(computingStatus);
        ((DependencyObject)this).SetValue(CalculatingStatusProperty, (object)computingStatus);
    }

    public override CalculatingStatus CalculatingStatus => (CalculatingStatus)((DependencyObject)this).GetValue(CalculatingStatusProperty);

    public override void SetSynchronizationStatus(SynchronizationStatus syncStatus)
    {
        base.SetSynchronizationStatus(syncStatus);
        ((DependencyObject)this).SetValue(SynchronizationStatusProperty, (object)syncStatus);
    }

    public override void SetIdentifierLabels(IDictionary<ResolvedId, string[]> idLabelMap)
    {
        Dictionary<ICalculatable, IDictionary<ResolvedId, string[]>> dictionary = new Dictionary<ICalculatable, IDictionary<ResolvedId, string[]>>();
        foreach (KeyValuePair<ResolvedId, string[]> item in idLabelMap)
        {
            ResolvedId key = item.Key;
            string[] value = item.Value;
            IDictionary<ResolvedId, string[]> dictionary2;
            ICalculatable calculatable;
            if (key.Calculatable == this)
            {
                key = new ResolvedId
                {
                    Calculatable = _oleBaseModel,
                    Name = key.Name,
                    Tag = key.Tag
                };
            }
            calculatable = key.Calculatable;
            if (dictionary.ContainsKey(calculatable))
            {
                dictionary2 = dictionary[calculatable];
            }
            else
            {
                dictionary[calculatable] = (dictionary2 = new Dictionary<ResolvedId, string[]>());
            }
            dictionary2.Add(key, value);
        }
        dictionary.Each((KeyValuePair<ICalculatable, IDictionary<ResolvedId, string[]>> calcToLabelsPair) =>
        {
            calcToLabelsPair.Key.SetIdentifierLabels(calcToLabelsPair.Value);
        });
    }

    public override SynchronizationStatus SynchronizationStatus => (SynchronizationStatus)((DependencyObject)this).GetValue(SynchronizationStatusProperty);

    static OleBaseComponentControl()
    {
        OleComponentLocationTopProperty = DependencyProperty.RegisterAttached("OleComponentLocationTop", typeof(double), typeof(OleBaseComponentControl), new PropertyMetadata((object)double.NaN, (PropertyChangedCallback)HandleOleComponentLocationTopChanged));
        OleComponentLocationLeftProperty = DependencyProperty.RegisterAttached("OleComponentLocationLeft", typeof(double), typeof(OleBaseComponentControl), new PropertyMetadata((object)double.NaN, (PropertyChangedCallback)HandleOleComponentLocationLeftChanged));
        ErrorProperty = DependencyProperty.Register("Error", typeof(EngineError), typeof(OleBaseComponentControl), new FrameworkPropertyMetadata((object)null, (PropertyChangedCallback)ErrorChanged));
        ResizableElementAttachedProperties.IsResizingProperty.AddOwner(typeof(OleBaseComponentControl), new FrameworkPropertyMetadata(IsResizingChanged));
        EngineErrorMessages.MergeDictionary(ResourceProvider<OleBaseComponentErrorsDictionary>.Instance);
        CalculatingStatusProperty = EquationControl.CalculatingStatusProperty.AddOwner(typeof(OleBaseComponentControl));
        SynchronizationStatusProperty = EquationControl.SynchronizationStatusProperty.AddOwner(typeof(OleBaseComponentControl));
    }

    private readonly OleBaseComponentViewModel _oleBaseViewModel;

    private readonly OleBaseComponentModel _oleBaseModel;

    protected OleBaseComponentControl(OleBaseComponentViewModel viewModel, ResourceDictionary dict)
        : base(viewModel, dict)
    {
        _oleBaseViewModel = (OleBaseComponentViewModel)ViewModel;
        _oleBaseModel = (OleBaseComponentModel)_oleBaseViewModel.Model;
        Init();
        if (_oleBaseViewModel.OleContainerItem != null)
        {
            HandleOleContainerItemAdded(_oleBaseViewModel.OleContainerItem);
        }
    }

    private void Init()
    {
        if (Resources.MergedDictionaries.Count > 0)
        {
            _oleBaseModel.Resources = Resources.MergedDictionaries[0];
        }
        _oleBaseModel.OleContainerItemAdded += HandleOleContainerItemAdded;
        Loaded += OnOleComponentLoaded;
        SetBinding(ErrorProperty, "Error");
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

    public IOleContainerItem OleContainerItem => _oleBaseViewModel.OleContainerItem;

    protected abstract IOleContainerItem CreateOleContainerItem();

    protected void CreateAndInitOleContainerItem()
    {
        IOleContainerItem oleContainerItem = CreateOleContainerItem();
        oleContainerItem.CreateWindowlessOle();
        _oleBaseViewModel.AddItemAtLocation(oleContainerItem, default);
        _oleBaseViewModel.SetInitialSize();
    }

    protected void HandleOleContainerItemAdded(IOleContainerItem container)
    {
        WhiteboardItemManager.HookupLeftMouseButtonDown(container, (IWhiteboardItem item) =>
        {
            Activate(isActive: true);
        });
    }

    public SectionIndicatorInfo InputSectionIndicator
    {
        get
        {
            return _oleBaseViewModel.InputSectionIndicator;
        }
        set
        {
            _oleBaseViewModel.InputSectionIndicator = value;
        }
    }

    public SectionIndicatorInfo OutputSectionIndicator
    {
        get
        {
            return _oleBaseViewModel.OutputSectionIndicator;
        }
        set
        {
            _oleBaseViewModel.OutputSectionIndicator = value;
        }
    }

    protected const string WhiteboardItemsPropertyName = "WhiteboardItems";

    protected const string IsInputSectionVisiblePropertyName = "IsInputSectionVisible";

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

    protected const string IsOutputSectionVisiblePropertyName = "IsOutputSectionVisible";

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

    public IEnumerable<EquationControl> Items
    {
        get
        {
            if (_oleBaseModel.WhiteboardItems.Count > 1)
            {
                return _oleBaseModel.WhiteboardItems.Where((IWhiteboardItem item) => !item.Equals(OleContainerItem)).Cast<EquationControl>();
            }
            return null;
        }
    }

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

    protected override void OnContextMenuOpening(ContextMenuEventArgs e)
    {
        base.OnContextMenuOpening(e);
        WpfUtils.SetMostRecentInputDeviceToKeyboardDevice();
    }

    public override void EndInit()
    {
        base.EndInit();
        if (DebugTools.AssertNotNull(OleContainerItem, null).Passed)
        {
            _oleBaseViewModel.EndInit();
        }
        foreach (IOleBaseComponentCalculatableWrapper calculatableWrapper in _oleBaseModel.CalculatableWrappers)
        {
            EngineResult engineResult = calculatableWrapper.EngineResult;
            if (engineResult.Errors.IsNotEmpty())
            {
                foreach (EngineError error in engineResult.Errors)
                {
                    error.ErrorSource = calculatableWrapper;
                }
            }
            calculatableWrapper.EngineResult = engineResult;
            calculatableWrapper.Commit();
        }
        if (EngineResult.Errors.IsNotEmpty())
        {
            EngineResult.Errors.Each((EngineError err) =>
            {
                err.ErrorSource = this;
            });
        }
        ResultManager.DispatchResult(this);
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

    public override string GetBlockEquationTypeAttributeName(IWhiteboardItem item)
    {
        return string.Empty;
    }

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

    protected override Size MeasureOverride(Size constraint)
    {
        return _oleBaseViewModel.Measure(base.MeasureOverride(constraint));
    }

    public static readonly DependencyProperty ErrorProperty;

    private NodeMessageAdorner _errorAdorner;

    private RubberBandHighlighter _errorFrame;

    public override EngineError Error => (EngineError)((DependencyObject)this).GetValue(ErrorProperty);

    private static void ErrorChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
        OleBaseComponentControl oleBaseComponentControl = obj as OleBaseComponentControl;
        if (DebugTools.AssertNotNull(oleBaseComponentControl, null).Failed)
        {
            return;
        }
        oleBaseComponentControl.ErrorChanged();
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

    private void FireCalcErrorChanged()
    {
        FirePropertyChanged(global::Quire.Calculation.Core.ErrorChangedEventArgs.GetArgs());
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

    public void UpdateAllErrors()
    {
        _oleBaseModel.UpdateAllErrors();
    }

    public override IWhiteboardItem[] WhiteboardItemsForXamlSerialization
    {
        get
        {
            return base.WhiteboardItemsForXamlSerialization;
        }
        set
        {
            foreach (IWhiteboardItem obj in value)
            {
                DependencyObject val = obj as DependencyObject;
                if (val != null && AttachedProperties.PreviousLocationHasLocalValue(val))
                {
                    Point previousLocation = AttachedProperties.GetPreviousLocation(val);
                    AttachedProperties.ClearPreviousLocation(val);
                    val.SetValue(OleComponentLocationLeftProperty, (object)previousLocation.X);
                    val.SetValue(OleComponentLocationTopProperty, (object)previousLocation.Y);
                }
            }
            base.WhiteboardItemsForXamlSerialization = value;
        }
    }

    public void SetPrintingZoom(bool set, double? newPrintingZoom)
    {
        OleContainerItem.SetPrintingZoom(set, newPrintingZoom);
    }

    public bool IsPrintingZoomSet => OleContainerItem.IsPrintingZoomSet;

    public IResizeData StoreResizeData(Size oldSize, Size newSize)
    {
        return _oleBaseViewModel.StoreResizeData(oldSize, newSize);
    }

    public void RestoreResizeData(IResizeData resizeData)
    {
        _oleBaseViewModel.RestoreResizeData(resizeData);
    }

    public Size CoerceNewSize(Size newSize, IResizeData resizeData)
    {
        return newSize;
    }

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

    public void CloseOle()
    {
        if (IsOleOpen)
        {
            OleContainerItem.StartOleCloseOperation();
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

    private int _componentCookie;

    private readonly object _componentCookieLock = new object();

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
