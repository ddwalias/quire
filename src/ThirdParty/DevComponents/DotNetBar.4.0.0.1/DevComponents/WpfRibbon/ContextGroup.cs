#nullable disable
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Quire.Mirror;

namespace Quire.DevComponents.WpfRibbon;

public partial class ContextGroup
{
    [MirrorVerified("Release IL")]
    private ObservableCollection<RibbonTab> m_LinkedTabs;

    [MirrorVerified("Release IL")]
    public ContextGroup()
    {
        m_LinkedTabs = new ObservableCollection<RibbonTab>();
    }

    [MirrorVerified("Release IL")]
    private Label m_ContentLabel;

    [MirrorVerified("Release IL")]
    public override void OnApplyTemplate()
    {
        m_ContentLabel = base.GetTemplateChild("PART_LabelContent") as Label;
        UpdateLabelGlassEffect();
        base.OnApplyTemplate();
    }

    [MirrorVerified("Release IL")]
    private void OnStockColorChanged(eContextGroupColor c) => ApplyNewStockColor(c);

    [MirrorVerified("Release IL")]
    public ObservableCollection<RibbonTab> LinkedTabs => m_LinkedTabs;

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty StockColorProperty;

    [MirrorVerified("Release IL")]
    public eContextGroupColor StockColor { get => (eContextGroupColor)base.GetValue(StockColorProperty); set => base.SetValue(StockColorProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty IsSimpleContentProperty;

    [MirrorVerified("Release IL")]
    private static readonly DependencyPropertyKey IsSimpleContentPropertyKey;

    [MirrorVerified("Release IL")]
    public bool IsSimpleContent { get => (bool)base.GetValue(IsSimpleContentProperty); internal set => base.SetValue(IsSimpleContentPropertyKey, value); }

    [MirrorVerified("Release IL")]
    internal Label ContentLabel => m_ContentLabel;
}
