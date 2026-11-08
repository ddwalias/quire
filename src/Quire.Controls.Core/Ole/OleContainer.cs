#nullable disable
using System;
using System.Windows;
using System.Windows.Threading;
using Quire.Controls.Core.AttachedBehavior;
using Quire.Core;
using Quire.Mirror;
using Quire.PersistentData;

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
}
