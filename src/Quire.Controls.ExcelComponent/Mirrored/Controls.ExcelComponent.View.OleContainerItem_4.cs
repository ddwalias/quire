using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Quire.Calculation;
using Quire.Calculation.Core;
using Quire.Controls.Core;
using Quire.Controls.ExcelComponent.ViewModel;

namespace Quire.Controls.ExcelComponent.View;

public abstract partial class OleContainerItem<TViewModel, TModel, TIPublicModel, TPersistentData>
{
    public bool IsEmpty => false;

    public virtual void OnEngineError(global::Quire.Calculation.Core.EngineError error)
    {
    }

    public double VerticalAlignmentOffset => 0.0;

    public global::System.Nullable<global::System.Windows.Vector> NavigateIntoItem(global::Quire.Direction direction)
    {
        return null;
    }

    protected override void HandleBeforeActiveStateToggled()
    {
        global::Quire.Controls.Core.AttachedProperties.SetIsBringToView(this, true);
    }

    protected override IValueConverter GetDimensionsConverter()
    {
        return new DimensionsConverter(this);
    }

    [global::Quire.Mirror.MirrorOf("Ptc.Controls.ExcelComponent.View.OleContainerItem`4+DimensionsConverter, Ptc.Controls.ExcelComponent")]
    public partial class DimensionsConverter : IValueConverter
    {
        private readonly OleContainerItem<TViewModel, TModel, TIPublicModel, TPersistentData> _container;

        public DimensionsConverter(OleContainerItem<TViewModel, TModel, TIPublicModel, TPersistentData> oleContainer)
        {
            _container = oleContainer;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)parameter)
            {
                _container._expectedSize.Width = (double)value;
            }
            else
            {
                _container._expectedSize.Height = (double)value;
            }
            return value;
        }
    }

    public void Activate(bool isActive)
    {
        DebugTools.FailAssertion("Should not be called.");
    }

    public string GetHelpContext()
    {
        DebugTools.FailAssertion("OleContainer help context shouldn't be exposed.");
        return null;
    }

    private Size _expectedSize;

    public abstract ICalculationComponent Component { get; }

    IOleContainerItemViewModel IOleContainerItem.ViewModel => ViewModel;

    public virtual bool IsResizing
    {
        get
        {
            return ViewModel.IsResizing;
        }
        set
        {
            ViewModel.IsResizing = value;
        }
    }

    protected Size ExpectedSize
    {
        get
        {
            if (DebugTools.DevAssert(ViewModel.Width.IsValid() && ViewModel.Width.IsGreaterThan(0.0) && ViewModel.Height.IsValid() && ViewModel.Height.IsGreaterThan(0.0), null).Failed)
            {
                return Size.Empty;
            }
            return new Size(ViewModel.Width, ViewModel.Height);
        }
    }

    public event Action<IWhiteboardItem> OnGotFocusEvent;

    public event Action<IWhiteboardItem, Direction> RequestRemove;

    public event PropertyChangedEventHandler PropertyChanged;

    protected OleContainerItem(TViewModel viewModel, ResourceDictionary dictionary)
        : base(viewModel, dictionary)
    {
        _expectedSize = new Size(Width, Height);
    }

    public void SimulateMouseClick(Point location, int numberOfClicks)
    {
        DebugTools.FailAssertion("Should not be called.");
    }

    protected override void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs args)
    {
        base.OnViewModelPropertyChanged(sender, args);
        if (args.PropertyName == "Width" || args.PropertyName == "Height")
        {
            DebugTools.DevAssert(ViewModel.Width.IsEqualTo(_expectedSize.Width) && ViewModel.Height.IsEqualTo(_expectedSize.Height), "OLE Component doesn't support dimension change from ViewModel");
        }
    }

    protected override Size ArrangeOverride(Size arrangeBounds)
    {
        return ArrangeOverrideAux(arrangeBounds, updateOleSizeIfNeeded: true);
    }

    protected Size ArrangeOverrideAux(Size arrangeBounds, bool updateOleSizeIfNeeded)
    {
        Size val = base.ArrangeOverride(arrangeBounds);
        Size expectedSize = ExpectedSize;
        if (!expectedSize.IsEmpty && DebugTools.DevAssert(expectedSize.IsEqualTo(val), null).Failed)
        {
            val = new Size(expectedSize.Width, expectedSize.Height);
        }
        if (updateOleSizeIfNeeded && !val.IsEqualTo(RenderSize))
        {
            ViewModel.UpdateOleSizeIfNeeded();
        }
        return val;
    }

    protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
    {
        DebugTools.DevAssert(sizeInfo.NewSize.Width.IsEqualTo(ViewModel.Width) && sizeInfo.NewSize.Height.IsEqualTo(ViewModel.Height), null);
        base.OnRenderSizeChanged(sizeInfo);
    }
}
