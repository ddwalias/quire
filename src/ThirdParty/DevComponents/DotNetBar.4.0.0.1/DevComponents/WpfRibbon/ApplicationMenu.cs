#nullable disable
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Quire.Mirror;

namespace Quire.DevComponents.WpfRibbon;

public partial class ApplicationMenu
{
    [MirrorVerified("Release IL")]
    protected override void ClearChildMouseHighlight()
    {
        base.ClearChildMouseHighlight();
        ClearChildMouseHighlight(AppItems);
        ClearChildMouseHighlight(MruItems);
    }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty MruItemsProperty;

    [MirrorVerified("Release IL")]
    public ObservableCollection<object> MruItems => (ObservableCollection<object>)base.GetValue(MruItemsProperty);

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty AppItemsProperty;

    [MirrorVerified("Release IL")]
    public ObservableCollection<object> AppItems => (ObservableCollection<object>)base.GetValue(AppItemsProperty);

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty HasAppItemsProperty;

    [MirrorVerified("Release IL")]
    public bool HasAppItems => (bool)base.GetValue(HasAppItemsProperty);

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty HasMruItemsProperty;

    [MirrorVerified("Release IL")]
    public bool HasMruItems => (bool)base.GetValue(HasMruItemsProperty);

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty MruItemTemplateProperty;

    [MirrorVerified("Release IL")]
    public DataTemplate MruItemTemplate { get => (DataTemplate)base.GetValue(MruItemTemplateProperty); set => base.SetValue(MruItemTemplateProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty AppItemTemplateProperty;

    [MirrorVerified("Release IL")]
    public DataTemplate AppItemTemplate { get => (DataTemplate)base.GetValue(AppItemTemplateProperty); set => base.SetValue(AppItemTemplateProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty MruItemTemplateSelectorProperty;

    [MirrorVerified("Release IL")]
    public DataTemplateSelector MruItemTemplateSelector { get => (DataTemplateSelector)base.GetValue(MruItemTemplateSelectorProperty); set => base.SetValue(MruItemTemplateSelectorProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty AppItemTemplateSelectorProperty;

    [MirrorVerified("Release IL")]
    public DataTemplateSelector AppItemTemplateSelector { get => (DataTemplateSelector)base.GetValue(AppItemTemplateSelectorProperty); set => base.SetValue(AppItemTemplateSelectorProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty DoubleClickCloseProperty;

    [MirrorVerified("Release IL")]
    public bool DoubleClickClose { get => (bool)base.GetValue(DoubleClickCloseProperty); set => base.SetValue(DoubleClickCloseProperty, value); }
}
