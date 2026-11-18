using System.Windows;
using System.Windows.Controls;
using Quire.Controls.Core;
using Quire.Core;

namespace Quire.Controls.Worksheet;

public partial class HeaderFooterZombie
{
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
}
