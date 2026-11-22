using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Quire.Controls.Core;
using Quire.Core;

namespace Quire.Controls.Worksheet;

public partial class HeaderFooterZombie
{
    private static readonly Type _typeOfThis = typeof(HeaderFooterZombie);

    public static readonly DependencyProperty PageNumberProperty = DependencyProperty.Register("PageNumber", typeof(int), _typeOfThis);

    private readonly IHeaderFooterZombieViewModel _viewModel;

    private SnappableZombieItemHelper _snappableHelper;

    public HeaderFooterZombie(IHeaderFooterControl headerFooter)
    {
        InitializeComponent();
        _viewModel = new HeaderFooterZombieViewModel(headerFooter);
        DataContext = _viewModel;
        _snappableHelper = SnappableZombieItemHelper.Create(this, true);
    }

    public int PageNumber
    {
        get => (int)GetValue(PageNumberProperty);
        set => SetValue(PageNumberProperty, value);
    }

    public bool IsAssociatedWithPage(IWorksheetPage worksheetPage)
    {
        if (worksheetPage == null)
        {
            return false;
        }

        if (worksheetPage.HeaderZombie != this && worksheetPage.FooterZombie != this && worksheetPage.FirstPageHeaderZombie != this)
        {
            return worksheetPage.FirstPageFooterZombie == this;
        }

        return true;
    }

    public UIElement RenderTarget
    {
        get => _snappableHelper.RenderTarget;
        private set => _snappableHelper.RenderTarget = value;
    }

    public ISnapToPixelHelper SnappingHelper => _snappableHelper;

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        RenderTarget = (UIElement)Template.FindName("PART_Content", this);
    }

    public void Dispose()
    {
        _snappableHelper.Dispose();
        _viewModel.Dispose();
    }

    public GeneralTransform ItemToZombieTransform => _viewModel.ItemToZombieTransform;

    public IMaybeHasPointThatSnapsToPixel SnapsToPixelItem => _viewModel.SnapsToPixelItem;

    public void ResetVisualBrush()
    {
        _viewModel.ResetVisualBrush();
    }

    public void RestoreVisualBrush()
    {
        _viewModel.RestoreVisualBrush();
    }

    public void Show()
    {
        Visibility = Visibility.Visible;
    }

    public void Collapse()
    {
        Visibility = Visibility.Collapsed;
    }

    public void SubscribeToZombieIsInitialized(Action handler)
    {
        if (_viewModel != null)
        {
            _viewModel.zombieIsInitialized += handler;
        }
    }

    public void UnsubscribeFromZombieIsInitialized(Action handler)
    {
        if (_viewModel != null)
        {
            _viewModel.zombieIsInitialized -= handler;
        }
    }
}
