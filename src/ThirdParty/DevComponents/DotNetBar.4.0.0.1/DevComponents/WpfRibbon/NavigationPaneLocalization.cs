#nullable disable
using System.Windows;
using Quire.Mirror;

namespace Quire.DevComponents.WpfRibbon;

public partial class NavigationPaneLocalization
{
    [MirrorVerified("Release IL")]
    public NavigationPaneLocalization() { }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty ShowMoreButtonsProperty;

    [MirrorVerified("Release IL")]
    public string ShowMoreButtons { get => (string)base.GetValue(ShowMoreButtonsProperty); set => base.SetValue(ShowMoreButtonsProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty ShowFewerButtonsProperty;

    [MirrorVerified("Release IL")]
    public string ShowFewerButtons { get => (string)base.GetValue(ShowFewerButtonsProperty); set => base.SetValue(ShowFewerButtonsProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty NavigationPaneOptionsProperty;

    [MirrorVerified("Release IL")]
    public string NavigationPaneOptions { get => (string)base.GetValue(NavigationPaneOptionsProperty); set => base.SetValue(NavigationPaneOptionsProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty AddRemoveButtonsProperty;

    [MirrorVerified("Release IL")]
    public string AddRemoveButtons { get => (string)base.GetValue(AddRemoveButtonsProperty); set => base.SetValue(AddRemoveButtonsProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty CollapsedPaneTextProperty;

    [MirrorVerified("Release IL")]
    public string CollapsedPaneText { get => (string)base.GetValue(CollapsedPaneTextProperty); set => base.SetValue(CollapsedPaneTextProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty CustomizeDialogTitleProperty;

    [MirrorVerified("Release IL")]
    public string CustomizeDialogTitle { get => (string)base.GetValue(CustomizeDialogTitleProperty); set => base.SetValue(CustomizeDialogTitleProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty CustomizeDialogLabelProperty;

    [MirrorVerified("Release IL")]
    public string CustomizeDialogLabel { get => (string)base.GetValue(CustomizeDialogLabelProperty); set => base.SetValue(CustomizeDialogLabelProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty CustomizeDialogButtonMoveUpProperty;

    [MirrorVerified("Release IL")]
    public string CustomizeDialogButtonMoveUp { get => (string)base.GetValue(CustomizeDialogButtonMoveUpProperty); set => base.SetValue(CustomizeDialogButtonMoveUpProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty CustomizeDialogButtonMoveDownProperty;

    [MirrorVerified("Release IL")]
    public string CustomizeDialogButtonMoveDown { get => (string)base.GetValue(CustomizeDialogButtonMoveDownProperty); set => base.SetValue(CustomizeDialogButtonMoveDownProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty CustomizeDialogButtonResetProperty;

    [MirrorVerified("Release IL")]
    public string CustomizeDialogButtonReset { get => (string)base.GetValue(CustomizeDialogButtonResetProperty); set => base.SetValue(CustomizeDialogButtonResetProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty CustomizeDialogButtonOkProperty;

    [MirrorVerified("Release IL")]
    public string CustomizeDialogButtonOk { get => (string)base.GetValue(CustomizeDialogButtonOkProperty); set => base.SetValue(CustomizeDialogButtonOkProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty CustomizeDialogButtonCancelProperty;

    [MirrorVerified("Release IL")]
    public string CustomizeDialogButtonCancel { get => (string)base.GetValue(CustomizeDialogButtonCancelProperty); set => base.SetValue(CustomizeDialogButtonCancelProperty, value); }
}
