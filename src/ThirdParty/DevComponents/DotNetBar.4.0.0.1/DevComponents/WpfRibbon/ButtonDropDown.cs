#nullable disable
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Quire.Mirror;

namespace Quire.DevComponents.WpfRibbon;

public partial class ButtonDropDown
{
    [MirrorVerified("Release IL")]
    private void OnIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e) => UpdateEnabledState();

    [MirrorVerified("Release IL")]
    private void UpdateEnabledState()
    {
        UpdateImageEnabledState();
        UpdateBorderRenderState();
    }

    [MirrorVerified("Release IL")]
    private void Popup_Opened(object sender, EventArgs e) => UpdateKeyTipsState();

    [MirrorVerified("Release IL")]
    public static readonly RoutedEvent PopupClosedEvent;

    [MirrorVerified("Release IL")]
    private void InternalPopupClosed(object source, EventArgs e)
    {
        UpdateKeyTipsState();
        OnPopupClosed(new RoutedEventArgs(PopupClosedEvent, this));
    }

    [MirrorVerified("Release IL")]
    protected virtual void OnPopupClosed(RoutedEventArgs e) => base.RaiseEvent(e);

    [MirrorVerified("Release IL")]
    protected virtual void OnPopupOpened(RoutedEventArgs e) => base.RaiseEvent(e);

    [MirrorVerified("Release IL")]
    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);
        UpdateInactiveButtonPart();
    }

    [MirrorVerified("Release IL")]
    protected override void OnMouseLeave(MouseEventArgs e)
    {
        base.OnMouseLeave(e);
        RemoveButtonHighlight();
    }

    [MirrorVerified("Release IL")]
    public static readonly RoutedEvent ClickEvent;

    [MirrorVerified("Release IL")]
    protected virtual void OnClick() => base.RaiseEvent(new RoutedEventArgs(ClickEvent, this));

    [MirrorVerified("Release IL")]
    private static void OnPopupTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((ButtonDropDown)d).UpdatePopupTypeTemplate();

    [MirrorVerified("Release IL")]
    private static void OnExpandVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((ButtonDropDown)d).UpdateExpandVisibility();

    [MirrorVerified("Release IL")]
    private static void OnInlineExpandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((ButtonDropDown)d).UpdateExpandVisibility();

    [MirrorVerified("Release IL")]
    protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
    {
        UpdateExpandVisibility();
        base.OnItemsChanged(e);
    }

    [MirrorVerified("Release IL")]
    private void AccessTextSizeChanged(object sender, SizeChangedEventArgs e) => UpdateInlineExpandParent();

    [MirrorVerified("Release IL")]
    private static void OnIsHighlightedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((ButtonDropDown)d).OnIsHighlightedChanged();

    [MirrorVerified("Release IL")]
    private void OnIsHighlightedChanged() => UpdateBorderRenderState();

    [MirrorVerified("Release IL")]
    protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
    {
        base.OnGotKeyboardFocus(e);
        SetButtonHighlight();
    }

    [MirrorVerified("Release IL")]
    protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
    {
        base.OnLostKeyboardFocus(e);
        RemoveButtonHighlight();
    }

    [MirrorVerified("Release IL")]
    protected override void OnLostFocus(RoutedEventArgs e)
    {
        base.OnLostFocus(e);
        UpdateBorderRenderState();
    }

    [MirrorVerified("Release IL")]
    private static void OnPopupPlacementChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((ButtonDropDown)d).UpdateExpandTransform();

    [MirrorVerified("Release IL")]
    private static void OnIsCheckedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((ButtonDropDown)d).OnIsCheckedChanged(e);

    [MirrorVerified("Release IL")]
    protected virtual void OnChecked(RoutedEventArgs e) => base.RaiseEvent(e);

    [MirrorVerified("Release IL")]
    protected virtual void OnUnchecked(RoutedEventArgs e) => base.RaiseEvent(e);

    [MirrorVerified("Release IL")]
    private static void OnInputGestureTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) { }

    [MirrorVerified("Release IL")]
    protected override AutomationPeer OnCreateAutomationPeer() => new ButtonDropDownAutomationPeer(this);

    [MirrorVerified("Release IL")]
    protected virtual void ClearChildMouseHighlight() => ClearChildMouseHighlight(base.Items);

    [MirrorVerified("Release IL")]
    public virtual ButtonDropDown Copy(bool deepCopy) => CloningMachine.Clone(this, deepCopy);

    [MirrorVerified("Release IL")]
    private void CanExecuteChanged(object sender, EventArgs e) => UpdateCanExecute();

    [MirrorVerified("Release IL")]
    protected virtual bool RaiseLivePreview => Ribbon.GetLivePreviewEnabled(this);

    [MirrorVerified("Release IL")]
    private bool IsOnContextMenu => false;

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty IsPressedProperty;

    [MirrorVerified("Release IL")]
    public bool IsPressed => (bool)base.GetValue(IsPressedProperty);

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty RoleProperty;

    [MirrorVerified("Release IL")]
    public eButtonRole Role { get => (eButtonRole)base.GetValue(RoleProperty); set => base.SetValue(RoleProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty PartVisibilityProperty;

    [MirrorVerified("Release IL")]
    public eButtonPartVisibility PartVisibility { get => (eButtonPartVisibility)base.GetValue(PartVisibilityProperty); set => base.SetValue(PartVisibilityProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty PopupTypeProperty;

    [MirrorVerified("Release IL")]
    public eDropDownType PopupType { get => (eDropDownType)base.GetValue(PopupTypeProperty); set => base.SetValue(PopupTypeProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty PopupPlacementProperty;

    [MirrorVerified("Release IL")]
    public ePopupPlacement PopupPlacement { get => (ePopupPlacement)base.GetValue(PopupPlacementProperty); set => base.SetValue(PopupPlacementProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty PopupPlacementTargetProperty;

    [MirrorVerified("Release IL")]
    public UIElement PopupPlacementTarget { get => (UIElement)base.GetValue(PopupPlacementTargetProperty); set => base.SetValue(PopupPlacementTargetProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty IsSimpleHeaderProperty;

    [MirrorVerified("Release IL")]
    private static readonly DependencyPropertyKey IsSimpleHeaderPropertyKey;

    [MirrorVerified("Release IL")]
    public bool IsSimpleHeader { get => (bool)base.GetValue(IsSimpleHeaderProperty); internal set => base.SetValue(IsSimpleHeaderPropertyKey, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty IsExpandVisibleProperty;

    [MirrorVerified("Release IL")]
    public Visibility IsExpandVisible => (Visibility)base.GetValue(IsExpandVisibleProperty);

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty ExpandVisibilityProperty;

    [MirrorVerified("Release IL")]
    public eExpandVisibility ExpandVisibility { get => (eExpandVisibility)base.GetValue(ExpandVisibilityProperty); set => base.SetValue(ExpandVisibilityProperty, value); }

    [MirrorVerified("Release IL")]
    internal static readonly DependencyProperty IsSelectedProperty;

    [MirrorVerified("Release IL")]
    internal bool IsSelected { get => (bool)base.GetValue(IsSelectedProperty); set => base.SetValue(IsSelectedProperty, value); }

    [MirrorVerified("Release IL")]
    private static readonly DependencyProperty ContentExpandsProperty;

    [MirrorVerified("Release IL")]
    public bool ContentExpands { get => (bool)base.GetValue(ContentExpandsProperty); set => base.SetValue(ContentExpandsProperty, value); }

    [MirrorVerified("Release IL")]
    private Popup m_Popup;

    [MirrorVerified("Release IL")]
    public Popup Popup => m_Popup;

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty IsPopupOpenProperty;

    [MirrorVerified("Release IL")]
    public bool IsPopupOpen { get => (bool)base.GetValue(IsPopupOpenProperty); set => base.SetValue(IsPopupOpenProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty IsCheckableProperty;

    [MirrorVerified("Release IL")]
    public bool IsCheckable { get => (bool)base.GetValue(IsCheckableProperty); set => base.SetValue(IsCheckableProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty IsCheckedProperty;

    [MirrorVerified("Release IL")]
    public bool IsChecked { get => (bool)base.GetValue(IsCheckedProperty); set => base.SetValue(IsCheckedProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty ExpandPositionProperty;

    [MirrorVerified("Release IL")]
    public eExpandPosition ExpandPosition { get => (eExpandPosition)base.GetValue(ExpandPositionProperty); set => base.SetValue(ExpandPositionProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty WrapLabelProperty;

    [MirrorVerified("Release IL")]
    public bool WrapLabel { get => (bool)base.GetValue(WrapLabelProperty); set => base.SetValue(WrapLabelProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty InlineExpandProperty;

    [MirrorVerified("Release IL")]
    public bool InlineExpand { get => (bool)base.GetValue(InlineExpandProperty); set => base.SetValue(InlineExpandProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty FocusChangeKeepsPopupOpenProperty;

    [MirrorVerified("Release IL")]
    public bool FocusChangeKeepsPopupOpen { get => (bool)base.GetValue(FocusChangeKeepsPopupOpenProperty); set => base.SetValue(FocusChangeKeepsPopupOpenProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty RenderImageProperty;

    [MirrorVerified("Release IL")]
    private static readonly DependencyPropertyKey RenderImagePropertyKey;

    [MirrorVerified("Release IL")]
    public object RenderImage { get => base.GetValue(RenderImageProperty); internal set => base.SetValue(RenderImagePropertyKey, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty ImageProperty;

    [MirrorVerified("Release IL")]
    public object Image { get => base.GetValue(ImageProperty); set => base.SetValue(ImageProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty ImageSmallProperty;

    [MirrorVerified("Release IL")]
    public object ImageSmall { get => base.GetValue(ImageSmallProperty); set => base.SetValue(ImageSmallProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty UseSmallImageProperty;

    [MirrorVerified("Release IL")]
    public bool UseSmallImage { get => (bool)base.GetValue(UseSmallImageProperty); set => base.SetValue(UseSmallImageProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty ImagePositionProperty;

    [MirrorVerified("Release IL")]
    public eButtonImagePosition ImagePosition { get => (eButtonImagePosition)base.GetValue(ImagePositionProperty); set => base.SetValue(ImagePositionProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty IsHighlightedProperty;

    [MirrorVerified("Release IL")]
    public bool IsHighlighted => (bool)base.GetValue(IsHighlightedProperty);

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty StaysOpenOnClickProperty;

    [MirrorVerified("Release IL")]
    public bool StaysOpenOnClick { get => (bool)base.GetValue(StaysOpenOnClickProperty); set => base.SetValue(StaysOpenOnClickProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty IsPopupAnimationSuspendedProperty;

    [MirrorVerified("Release IL")]
    public bool IsPopupAnimationSuspended { get => (bool)base.GetValue(IsPopupAnimationSuspendedProperty); set => base.SetValue(IsPopupAnimationSuspendedProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty InputGestureTextProperty;

    [MirrorVerified("Release IL")]
    public string InputGestureText { get => (string)base.GetValue(InputGestureTextProperty); set => base.SetValue(InputGestureTextProperty, value); }

    [MirrorVerified("Release IL")]
    private ElementAutoSizeBag m_AutoSizeBag;

    [MirrorVerified("Release IL")]
    internal ElementAutoSizeBag AutoSizeBag { get => m_AutoSizeBag; set => m_AutoSizeBag = value; }

    [MirrorVerified("Release IL")]
    private FrameworkElement m_ButtonImage;

    [MirrorVerified("Release IL")]
    internal bool HasImage => m_ButtonImage != null;

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty ColorClassProperty;

    [MirrorVerified("Release IL")]
    public string ColorClass { get => (string)base.GetValue(ColorClassProperty); set => base.SetValue(ColorClassProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty OptionGroupProperty;

    [MirrorVerified("Release IL")]
    public string OptionGroup { get => (string)base.GetValue(OptionGroupProperty); set => base.SetValue(OptionGroupProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty CornerRadiusProperty;

    [MirrorVerified("Release IL")]
    public CornerRadius CornerRadius { get => (CornerRadius)base.GetValue(CornerRadiusProperty); set => base.SetValue(CornerRadiusProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty CommandProperty;

    [MirrorVerified("Release IL")]
    public ICommand Command { get => (ICommand)base.GetValue(CommandProperty); set => base.SetValue(CommandProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty CommandParameterProperty;

    [MirrorVerified("Release IL")]
    public object CommandParameter { get => base.GetValue(CommandParameterProperty); set => base.SetValue(CommandParameterProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty CommandTargetProperty;

    [MirrorVerified("Release IL")]
    public IInputElement CommandTarget { get => (IInputElement)base.GetValue(CommandTargetProperty); set => base.SetValue(CommandTargetProperty, value); }

    [MirrorVerified("Release IL")]
    private bool IsDesignMode => DesignerProperties.GetIsInDesignMode(this);
}
