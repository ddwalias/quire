#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Quire.Wpf;
using System.Windows.Threading;
using Quire.Controls.Core.AttachedBehavior;
using Quire.Core;
using Quire.Mirror;
using Quire.PersistentData;
using Quire.Serialization;

namespace Quire.Controls.Core.Ole;

public abstract partial class OleContainer<TViewModel, TModel, TIPublicModel, TPersistentData>
{
    private static readonly Type _typeOfThis;

    private const DispatcherPriority OleForceClosePriority = (DispatcherPriority)4;

    public static readonly RoutedEvent ContentChangedEvent;

    public static readonly DependencyProperty IsInPrintingProperty;

    public static readonly DependencyProperty IsInCopyProperty;

    static OleContainer()
    {
        _typeOfThis = typeof(OleContainer<TViewModel, TModel, TIPublicModel, TPersistentData>);
        FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(_typeOfThis, new FrameworkPropertyMetadata(_typeOfThis));
        WorksheetPropertiesManager.RegisterToWorksheetPropertyNotifications(_typeOfThis);
        IsInPrintingProperty = DocumentAttachedProperties.IsInPrintingProperty.AddOwner(_typeOfThis, new FrameworkPropertyMetadata(OnIsInPrintingPropertyChanged));
        IsInCopyProperty = DocumentAttachedProperties.IsInCopyProperty.AddOwner(_typeOfThis, new FrameworkPropertyMetadata(OnIsInCopyPropertyChanged));
        ContentChangedEvent = CommonRoutedEvents.ContentChangedEvent.AddOwner(_typeOfThis);
    }

    private static void OnIsInPrintingPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((OleContainer<TViewModel, TModel, TIPublicModel, TPersistentData>)d).OnIsInPrintingPropertyChanged(e);
    }

    private static void OnIsInCopyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((OleContainer<TViewModel, TModel, TIPublicModel, TPersistentData>)d).OnIsInCopyPropertyChanged(e);
    }

    public TViewModel ViewModel { get; private set; }

    IOleContainerViewModel IOleContainer.ViewModel => ViewModel;

    public TIPublicModel PublicModel => ViewModel.PublicModel;

    private DispatcherOperation OleForceCloseOperation { get; set; }

    public Size MinSizeForXamlSerialization
    {
        get => ViewModel.MinSizeValue;
        set => ViewModel.MinSize = value;
    }

    protected override bool IsOleCreated => ViewModel.IsCreated;

    protected virtual bool IsZoomAware => false;

    public bool IsInPrinting => (bool)GetValue(IsInPrintingProperty);

    public bool IsInCopy => (bool)GetValue(IsInCopyProperty);

    public IOleSizeData SizeDataForXamlSerialization
    {
        get => ViewModel.SizeDataForXamlSerialization;
        set => ViewModel.SizeDataForXamlSerialization = value;
    }

    protected bool CanPrint => !AttachedProperties.GetContainedInCollapsedWhiteboard(this);

    public string ImageFileNameForPrintXamlSerialization
    {
        get
        {
            if (!IsInPrinting)
            {
                return null;
            }
            return TrySavingBitmapForPrint();
        }
        set
        {
            if (value == null)
            {
                return;
            }
            DebugTools.DevAssert(!ViewModel.IsCreated, null);
            Bitmap = PublicModel.LoadBitmap(value);
        }
    }

    public bool? IsOleOpenForXamlSerializationInPrinting
    {
        get
        {
            if (!IsInPrinting)
            {
                return null;
            }
            return ViewModel.GrayVisibility == Visibility.Visible;
        }
        set
        {
            if (value.HasValue && value.Value)
            {
                ViewModel.ForceGrayVisibility();
            }
        }
    }

    public MessageList MessageListForPrint
    {
        get
        {
            if (!IsInPrinting)
            {
                return null;
            }
            return ViewModel.MessageList;
        }
        set
        {
            if (value != null)
            {
                foreach (MessageItem item in value)
                {
                    ViewModel.SetMessage(item.MessageCategory, item.MessageType, item.MessageResourceKey);
                }
            }
        }
    }

    public bool SiteCreated => ViewModel.SiteCreated;

    public bool IsOutOfPlaceActive => ViewModel.IsOutOfPlaceActive;

    public bool IsStorageEmpty => PublicModel.IsStorageEmpty;

    public ISnapToPixelHelper SnappingHelper => _snappableHelper;

    public double Zoom => DocumentAttachedProperties.GetZoom(this);

    public bool IsPrintingZoomSet => _prePrintingZoom.HasValue;

    protected bool HasEffectivePrintingZoom
    {
        get
        {
            if (_prePrintingZoom.HasValue)
            {
                return _prePrintingZoom.Value != Zoom;
            }
            return false;
        }
    }

    protected override void OnZoomChanged()
    {
        if (ViewModel.IsOleAppSupportZoom)
        {
            InvalidateVisual();
        }
    }

    protected virtual void OnIsInPrintingPropertyChanged(DependencyPropertyChangedEventArgs e)
    {
        DebugTools.DevAssert(IsOleCreated && IsStorageEmpty == (!CanPrint || (bool)e.NewValue), null);
        ClearStorageOnPropertyChanged(e, deleteFile: true);
    }

    protected virtual void OnIsInCopyPropertyChanged(DependencyPropertyChangedEventArgs e)
    {
        DebugTools.DevAssert(IsStorageEmpty.Equals(e.NewValue), null);
        ClearStorageOnPropertyChanged(e, deleteFile: false);
    }

    protected void ClearStorageOnPropertyChanged(DependencyPropertyChangedEventArgs e, bool deleteFile)
    {
        if (!(bool)e.NewValue)
        {
            CleanStorage(deleteFile);
        }
    }

    protected virtual void HandleBeforeActiveStateToggled()
    {
    }

    protected override bool ShouldSerializeProperty(DependencyProperty dp)
    {
        if (dp == UIElement.RenderTransformProperty)
        {
            return false;
        }
        return base.ShouldSerializeProperty(dp);
    }

    protected virtual IValueConverter GetDimensionsConverter()
    {
        return null;
    }

    protected virtual void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs args)
    {
        if (args.PropertyName == "AutoResize" || args.PropertyName == "Width" || args.PropertyName == "Height")
        {
            RaiseEvent(new RoutedEventArgs(ContentChangedEvent));
        }
    }

    protected virtual void OnViewChangedSync()
    {
        Bitmap = null;
        if (!HasEffectivePrintingZoom)
        {
            InvalidateVisual();
        }
    }

    protected virtual void OnViewChangedOrSavedAsync()
    {
        RaiseEvent(new RoutedEventArgs(ContentChangedEvent));
    }

    public void AbortOleCloseOperation()
    {
        if (OleForceCloseOperation != null)
        {
            DebugTools.DevAssert(OleForceCloseOperation.Abort(), null);
        }
        ViewModel.AbortDeactivate();
    }

    private void OleForceClose()
    {
        ViewModel.OleForceClose();
    }

    protected virtual void OnCreationError(bool isNew)
    {
        if (isNew)
        {
            ThrowExceptionWithMessage("server_busy_create_error");
        }
        else
        {
            ThrowExceptionWithMessage("server_busy_create_error_from_file");
        }
    }

    protected virtual void OnCreationError()
    {
    }

    public void CreateWindowlessOle()
    {
        if (!ViewModel.CreateWindowlessOle())
        {
            OnCreationError();
        }
    }

    protected void ThrowExceptionWithMessage(string messageKey)
    {
        string text = TryFindResource(messageKey) as string;
        if (DebugTools.AssertNotNull(text, null).Failed)
        {
            text = messageKey;
        }
        throw new ApplicationException(text);
    }

    public void CleanStorage(bool deleteFile = true)
    {
        PublicModel.CleanStorage(deleteFile);
    }

    public virtual IList<IRegionPersistentData> SetOlePersistentData(IDeS11NProvider d11NProvider, TPersistentData data)
    {
        return ViewModel.SetRegionPersistentData(d11NProvider, data);
    }

    public void OnWorksheetPropertiesChanged(string[] propertyNames, IWorksheetProperties oldProperties, IWorksheetProperties newProperties)
    {
        ViewModel.OnWorksheetPropertiesChanged(propertyNames, oldProperties, newProperties);
    }

    public void Dispose()
    {
        _snappableHelper.Dispose();
        AbortOleCloseOperation();
        ViewModel.Dispose();
    }

    public void SetPrintingZoom(bool set, double? newPrintingZoom)
    {
        if (!set)
        {
            DebugTools.DevAssert(!newPrintingZoom.HasValue, null);
        }
        bool hasValue = _prePrintingZoom.HasValue;
        if (set && !CanPrint)
        {
            DebugTools.DevAssert(!hasValue, null);
            return;
        }
        double num = (set ? newPrintingZoom : _prePrintingZoom) ?? Zoom;
        if (hasValue != set)
        {
            _prePrintingZoom = set ? new double?(Zoom) : null;
            ApplyPrintingZoom(set ? new double?(num) : null);
        }
        DebugTools.DevAssert(Zoom == num, null);
    }

    protected virtual void ApplyPrintingZoom(double? zoom)
    {
        if (zoom.HasValue)
        {
            SetValue(DocumentAttachedProperties.ZoomProperty, zoom.Value);
            return;
        }
        if (CanPrint)
        {
            DebugTools.AssertNotNull(TrySavingBitmapForPrint(), null);
        }
        ClearValue(DocumentAttachedProperties.ZoomProperty);
    }

    protected OleContainer(TViewModel viewModel, ResourceDictionary dictionary)
    {
        if (dictionary != null)
        {
            Resources.MergedDictionaries.Add(dictionary);
        }
        ViewModel = viewModel;
        ViewModel.ViewChangedSync += OnViewChangedSync;
        ViewModel.ViewChangedAsync += OnViewChangedOrSavedAsync;
        ViewModel.SavedAsync += OnViewChangedOrSavedAsync;
        ViewModel.BeforeActiveStateToggled += HandleBeforeActiveStateToggled;
        ViewModel.PropertyChanged += OnViewModelPropertyChanged;
        DataContext = ViewModel;
        IValueConverter dimensionsConverter = GetDimensionsConverter();
        SetBinding(FrameworkElement.WidthProperty, new Binding
        {
            Path = new PropertyPath("Width"),
            Mode = BindingMode.TwoWay,
            Converter = dimensionsConverter,
            ConverterParameter = dimensionsConverter != null ? (object)true : null
        });
        SetBinding(FrameworkElement.HeightProperty, new Binding
        {
            Path = new PropertyPath("Height"),
            Mode = BindingMode.TwoWay,
            Converter = dimensionsConverter,
            ConverterParameter = dimensionsConverter != null ? (object)false : null
        });
        SetBinding(FrameworkElement.MaxWidthProperty, new Binding
        {
            Path = new PropertyPath("MaxWidth"),
            Mode = BindingMode.TwoWay
        });
        SetBinding(FrameworkElement.MaxHeightProperty, new Binding
        {
            Path = new PropertyPath("MaxHeight"),
            Mode = BindingMode.TwoWay
        });
        _snappableHelper = SnappableItemHelper.Create(this, isPageDescendant: true);
        _snappableHelper.RenderTarget = this;
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
        base.OnRender(drawingContext);
        ImageSource bitmap = Bitmap;
        if (bitmap != null)
        {
            Size scale;
            Int32Rect renderRectInPixels = GetRenderRectInPixels(Size.Empty, out scale);
            Rect rectangle;
            // Release IL constructs the Rect in place, so it is assigned from new in each branch.
            if (DebugTools.DevAssert(!renderRectInPixels.IsEmpty, null).Passed)
            {
                rectangle = new Rect(new Size((double)renderRectInPixels.Width / scale.Width, (double)renderRectInPixels.Height / scale.Height));
            }
            else
            {
                rectangle = new Rect(RenderSize);
            }
            drawingContext.DrawImage(bitmap, rectangle);
        }
        else
        {
            drawingContext.DrawRectangle(Brushes.White, null, new Rect(RenderSize));
        }
    }

    protected override BitmapSource CreateBitmap(Int32Rect rectInPixels, HwndSource hwndSource)
    {
        DebugTools.DevAssert(!HasEffectivePrintingZoom, null);
        nint hBitmap = CreateHBitmap(rectInPixels, hwndSource);
        return CreateBitmapSource(hBitmap, rectInPixels);
    }

    protected override Int32Rect GetRenderRectInPixels(out HwndSource hwndSource)
    {
        Size scale;
        return GetRenderRectInPixels(ViewModel.AutomaticSize, out scale, out hwndSource);
    }

    protected Int32Rect GetRenderRectInPixels(Size renderSize, out Size scale)
    {
        HwndSource hwndSource;
        return GetRenderRectInPixels(renderSize, out scale, out hwndSource);
    }

    protected Int32Rect GetRenderRectInPixels(Size renderSize, out Size scale, out HwndSource hwndSource)
    {
        scale = ImageUtils.DpiScaleForVisual(this, out hwndSource);
        if (scale.IsEmpty)
        {
            return Int32Rect.Empty;
        }
        double value = IsZoomAware ? DocumentAttachedProperties.GetZoom(this) : 1.0;
        scale = new Size(Math.Round(scale.Width, 2) * Math.Round(value, 2), Math.Round(scale.Height, 2) * Math.Round(value, 2));
        if (renderSize.IsEmpty)
        {
            renderSize = RenderSize;
        }
        return new Int32Rect(0, 0, (Math.Round(renderSize.Width, 5) * scale.Width).IntRound(), (Math.Round(renderSize.Height, 5) * scale.Height).IntRound());
    }

    protected nint CreateHBitmap(Int32Rect rectInPixels, HwndSource hwndSource)
    {
        if (!rectInPixels.IsEmpty)
        {
            return ViewModel.CreateHBitmap(rectInPixels, hwndSource.Handle);
        }
        return IntPtr.Zero;
    }

    protected BitmapSource CreateBitmapSource(nint hBitmap, Int32Rect rectInDpi)
    {
        if (hBitmap == IntPtr.Zero)
        {
            return null;
        }
        DebugTools.LogData("OleContainer.CreateBitmapSource: Enter");
        BitmapSource bitmapSource = null;
        try
        {
            bitmapSource = Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, rectInDpi, BitmapSizeOptions.FromEmptyOptions());
            ViewModel.ClearMessage(MessageCategory.Render);
        }
        catch (Exception ex)
        {
            string messageKey;
            if (ex is OutOfMemoryException)
            {
                GC.Collect();
                messageKey = "out_of_memory_error";
            }
            else
            {
                messageKey = ex.Message;
            }
            ViewModel.SetMessage(MessageCategory.Render, NodeMessageAdornerType.Error, messageKey);
        }
        FormatConvertedBitmap formatConvertedBitmap = null;
        if (bitmapSource != null)
        {
            formatConvertedBitmap = new FormatConvertedBitmap();
            formatConvertedBitmap.BeginInit();
            formatConvertedBitmap.Source = bitmapSource;
            formatConvertedBitmap.DestinationFormat = PixelFormats.Bgr32;
            formatConvertedBitmap.EndInit();
        }
        return formatConvertedBitmap;
    }

    protected override void AcquireBitmapSource()
    {
        if (ViewModel.IsOleAppSupportZoom)
        {
            double zoom = DocumentAttachedProperties.GetZoom(this);
            _bitmap = CreateBitmap(new Int32Rect(0, 0, (RenderSize.Width * zoom).IntRound(), (RenderSize.Height * zoom).IntRound()), PresentationSource.FromVisual(this) as HwndSource);
        }
        else if (SiteCreated)
        {
            base.AcquireBitmapSource();
        }
    }
}
