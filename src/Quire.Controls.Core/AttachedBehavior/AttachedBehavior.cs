#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using Quire.Core;
using Quire.Mirror;

namespace Quire.Controls.Core.AttachedBehavior;

public static partial class AttachedBehavior
{
    private static readonly Type _typeOfThis = typeof(AttachedBehavior);

    public static readonly DependencyProperty CanBeActivatedProperty = DependencyProperty.RegisterAttached("CanBeActivated", typeof(object), _typeOfThis, new PropertyMetadata(false, OnAttachedBehaviorValueChanged, CoerceBooleanToAttachedBehavior<FocusActivator>));

    [MirrorVerified("PtcControlsCoreAttachedBehaviorAttachedBehaviorMechanicalDifferentialTests")]
    public static readonly DependencyProperty ParentPropertyValueKeepersProperty = DependencyProperty.RegisterAttached("ParentPropertyValueKeepers", typeof(ParentPropertyValueKeepers), _typeOfThis, new PropertyMetadata(null, OnAttachedBehaviorValueChanged));

    [MirrorVerified("PtcControlsCoreAttachedBehaviorAttachedBehaviorMechanicalDifferentialTests")]
    public static readonly DependencyProperty ChildPropertyValueFindersProperty = DependencyProperty.RegisterAttached("ChildPropertyValueFinders", typeof(ChildPropertyValueFinders), _typeOfThis, new PropertyMetadata(null, OnAttachedBehaviorValueChanged));

    [MirrorVerified("PtcControlsCoreAttachedBehaviorAttachedBehaviorMechanicalDifferentialTests")]
    public static readonly DependencyProperty PropertyBindersProperty = DependencyProperty.RegisterAttached("PropertyBinders", typeof(object), _typeOfThis, new PropertyMetadata(null, OnAttachedBehaviorValueChanged, CoerceItemToCollection<PropertiesBinder, PropertyBindersCollection>));

    [MirrorVerified("PtcControlsCoreAttachedBehaviorAttachedBehaviorMechanicalDifferentialTests")]
    public static readonly DependencyProperty RoutedEventBindingsProperty = DependencyProperty.RegisterAttached("RoutedEventBindings", typeof(object), _typeOfThis, new PropertyMetadata(null, OnAttachedBehaviorValueChanged, CoerceAttachedBehavior<RoutedEventHandlersBinder, RoutedEventBindingCollection>));

    [MirrorVerified("PtcControlsCoreAttachedBehaviorAttachedBehaviorMechanicalDifferentialTests")]
    public static readonly DependencyProperty MessagesDecoratorProperty = DependencyProperty.RegisterAttached("MessagesDecorator", typeof(MessagesDecorator), _typeOfThis, new PropertyMetadata(null, OnAttachedBehaviorValueChanged));

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public static bool GetCanBeActivated(FrameworkElement elem) => (bool)elem.GetValue(CanBeActivatedProperty);

    public static void SetCanBeActivated(FrameworkElement elem, bool value) => elem.SetValue(CanBeActivatedProperty, value);

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public static ParentPropertyValueKeepers GetParentPropertyValueKeepers(FrameworkElement elem) => (ParentPropertyValueKeepers)elem.GetValue(ParentPropertyValueKeepersProperty);

    [MirrorVerified("PtcControlsCoreAttachedBehaviorAttachedBehaviorMechanicalDifferentialTests")]
    public static void SetParentPropertyValueKeepers(FrameworkElement elem, ParentPropertyValueKeepers value) => elem.SetValue(ParentPropertyValueKeepersProperty, value);

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public static ChildPropertyValueFinders GetChildPropertyValueFinders(FrameworkElement elem) => (ChildPropertyValueFinders)elem.GetValue(ChildPropertyValueFindersProperty);

    [MirrorVerified("PtcControlsCoreAttachedBehaviorAttachedBehaviorMechanicalDifferentialTests")]
    public static void SetChildPropertyValueFinders(FrameworkElement elem, ChildPropertyValueFinders value) => elem.SetValue(ChildPropertyValueFindersProperty, value);

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public static PropertyBindersCollection GetPropertyBinders(FrameworkElement elem) => (PropertyBindersCollection)elem.GetValue(PropertyBindersProperty);

    [MirrorVerified("PtcControlsCoreAttachedBehaviorAttachedBehaviorMechanicalDifferentialTests")]
    public static void SetPropertyBinders(FrameworkElement elem, object value) => elem.SetValue(PropertyBindersProperty, value);

    [MirrorVerified("PtcControlsCoreAttachedBehaviorAttachedBehaviorMechanicalDifferentialTests")]
    public static void SetRoutedEventBindings(FrameworkElement element, RoutedEventBindingCollection value) => element.SetValue(RoutedEventBindingsProperty, value);

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public static RoutedEventBindingCollection GetRoutedEventBindings(FrameworkElement element) => (RoutedEventBindingCollection)element.GetValue(RoutedEventBindingsProperty);

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public static MessagesDecorator GetMessagesDecorator(FrameworkElement elem) => (MessagesDecorator)elem.GetValue(MessagesDecoratorProperty);

    [MirrorVerified("PtcControlsCoreAttachedBehaviorAttachedBehaviorMechanicalDifferentialTests")]
    public static void SetMessagesDecorator(FrameworkElement elem, MessagesDecorator decorator) => elem.SetValue(MessagesDecoratorProperty, decorator);

    private static object CoerceAttachedBehavior<TAttachedBehavior, TValueType>(DependencyObject dObj, object basevalue)
        where TAttachedBehavior : IAttachedBehavior<TValueType>, new()
        where TValueType : class
    {
        if (basevalue == null)
        {
            return null;
        }
        if (basevalue is TValueType)
        {
            return new TAttachedBehavior
            {
                OriginalValue = (basevalue as TValueType)
            };
        }
        DebugTools.FailAssertion(null, Array.Empty<object>());
        return null;
    }

    private static object CoerceBooleanToAttachedBehavior<TAttachedBehavior>(DependencyObject dObj, object basevalue)
        where TAttachedBehavior : class, IAttachedBehavior, new()
    {
        if (bool.TryParse(basevalue.ToString(), out var result))
        {
            if (result)
            {
                return new TAttachedBehavior();
            }
            return result;
        }
        DebugTools.FailAssertion(null, Array.Empty<object>());
        return false;
    }

    private static object CoerceItemToCollection<TItem, TCollection>(DependencyObject dObj, object basevalue)
        where TItem : class
        where TCollection : class, ICollection<TItem>, new()
    {
        if (basevalue == null || basevalue is TCollection)
        {
            return basevalue;
        }
        if (DebugTools.DevAssert(basevalue is TItem, null).Failed)
        {
            return null;
        }
        return new TCollection { basevalue as TItem };
    }

    private static void OnAttachedBehaviorValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        FrameworkElement frameworkElement = sender as FrameworkElement;
        if (DebugTools.AssertNotNull(frameworkElement, null).Failed)
        {
            return;
        }
        if (e.OldValue is IAttachedBehavior)
        {
            (e.OldValue as IAttachedBehavior).Detach(frameworkElement);
        }
        if (e.NewValue is IAttachedBehavior)
        {
            (e.NewValue as IAttachedBehavior).Attach(frameworkElement);
        }
    }
}
