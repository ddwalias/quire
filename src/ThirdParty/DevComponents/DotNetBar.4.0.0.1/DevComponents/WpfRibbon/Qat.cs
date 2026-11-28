#nullable disable
using System.Windows;
using Quire.Mirror;

namespace Quire.DevComponents.WpfRibbon;

public partial class Qat
{
    [MirrorVerified("Release IL")]
    private QatOverflowPanel m_OverflowPanel;

    [MirrorVerified("Release IL")]
    public override void OnApplyTemplate()
    {
        m_OverflowPanel = base.GetTemplateChild("PART_OverflowPanel") as QatOverflowPanel;
        base.OnApplyTemplate();
    }

    [MirrorVerified("Release IL")]
    internal static readonly DependencyPropertyKey IsOverflowItemPropertyKey;

    [MirrorVerified("Release IL")]
    internal static void SetIsOverflowItem(DependencyObject element, object value) => element.SetValue(IsOverflowItemPropertyKey, value);

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty IsOverflowOpenProperty;

    [MirrorVerified("Release IL")]
    private object OpenPopupOnLoad(object arg)
    {
        base.CoerceValue(IsOverflowOpenProperty);
        return null;
    }

    [MirrorVerified("Release IL")]
    public Qat() { }

    [MirrorVerified("Release IL")]
    internal QatOverflowPanel OverflowPanel => m_OverflowPanel;

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty IsAboveRibbonProperty;

    [MirrorVerified("Release IL")]
    internal static readonly DependencyPropertyKey IsAboveRibbonPropertyKey;

    [MirrorVerified("Release IL")]
    public bool IsAboveRibbon { get => (bool)base.GetValue(IsAboveRibbonProperty); internal set => base.SetValue(IsAboveRibbonPropertyKey, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty HasOverflowItemsProperty;

    [MirrorVerified("Release IL")]
    internal static readonly DependencyPropertyKey HasOverflowItemsPropertyKey;

    [MirrorVerified("Release IL")]
    public bool HasOverflowItems { get => (bool)base.GetValue(HasOverflowItemsProperty); internal set => base.SetValue(HasOverflowItemsPropertyKey, value); }

    [MirrorVerified("Release IL")]
    public bool IsOverflowOpen { get => (bool)base.GetValue(IsOverflowOpenProperty); set => base.SetValue(IsOverflowOpenProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty IsActiveProperty;

    [MirrorVerified("Release IL")]
    public bool IsActive { get => (bool)base.GetValue(IsActiveProperty); set => base.SetValue(IsActiveProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty IsGlassEnabledProperty;

    [MirrorVerified("Release IL")]
    public bool IsGlassEnabled { get => (bool)base.GetValue(IsGlassEnabledProperty); set => base.SetValue(IsGlassEnabledProperty, value); }
}
