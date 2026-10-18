#nullable disable
using System.Windows;
using Quire.Mirror;

namespace Quire.Controls.Core.AttachedBehavior;

public static partial class AttachedBehavior
{
    [MirrorVerified("PtcControlsCoreAttachedBehaviorAttachedBehaviorMechanicalDifferentialTests")]
    public static readonly DependencyProperty ParentPropertyValueKeepersProperty;

    [MirrorVerified("PtcControlsCoreAttachedBehaviorAttachedBehaviorMechanicalDifferentialTests")]
    public static void SetParentPropertyValueKeepers(FrameworkElement elem, ParentPropertyValueKeepers value) => elem.SetValue(ParentPropertyValueKeepersProperty, value);

    [MirrorVerified("PtcControlsCoreAttachedBehaviorAttachedBehaviorMechanicalDifferentialTests")]
    public static readonly DependencyProperty ChildPropertyValueFindersProperty;

    [MirrorVerified("PtcControlsCoreAttachedBehaviorAttachedBehaviorMechanicalDifferentialTests")]
    public static void SetChildPropertyValueFinders(FrameworkElement elem, ChildPropertyValueFinders value) => elem.SetValue(ChildPropertyValueFindersProperty, value);

    [MirrorVerified("PtcControlsCoreAttachedBehaviorAttachedBehaviorMechanicalDifferentialTests")]
    public static readonly DependencyProperty PropertyBindersProperty;

    [MirrorVerified("PtcControlsCoreAttachedBehaviorAttachedBehaviorMechanicalDifferentialTests")]
    public static void SetPropertyBinders(FrameworkElement elem, object value) => elem.SetValue(PropertyBindersProperty, value);

    [MirrorVerified("PtcControlsCoreAttachedBehaviorAttachedBehaviorMechanicalDifferentialTests")]
    public static readonly DependencyProperty RoutedEventBindingsProperty;

    [MirrorVerified("PtcControlsCoreAttachedBehaviorAttachedBehaviorMechanicalDifferentialTests")]
    public static void SetRoutedEventBindings(FrameworkElement element, RoutedEventBindingCollection value) => element.SetValue(RoutedEventBindingsProperty, value);

    [MirrorVerified("PtcControlsCoreAttachedBehaviorAttachedBehaviorMechanicalDifferentialTests")]
    public static readonly DependencyProperty MessagesDecoratorProperty;

    [MirrorVerified("PtcControlsCoreAttachedBehaviorAttachedBehaviorMechanicalDifferentialTests")]
    public static void SetMessagesDecorator(FrameworkElement elem, MessagesDecorator decorator) => elem.SetValue(MessagesDecoratorProperty, decorator);
}
