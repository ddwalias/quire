#nullable disable
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Quire;
using Quire.Core;
using Quire.Mirror;
using Quire.Wpf;

namespace Quire.Controls.Core.AttachedBehavior;

internal partial class FocusActivator : IAttachedBehavior
{
    private FrameworkElement _element;

    [MirrorVerified("PtcControlsCoreAttachedBehaviorFocusActivatorMechanicalDifferentialTests")]
    private IActivator _activator;

    public void Attach(FrameworkElement element)
    {
        if ((AssertionE)DebugTools.DevAssert(element != null, null) == AssertionE.Failed)
        {
            return;
        }
        HookupToElementEvents(element);
        HookupToActivatorEvents(element.DataContext as IActivator);
    }

    public void Detach(FrameworkElement element)
    {
        DebugTools.DevAssert(element == _element, null);
        UnhookFromActivatorEvents();
        UnhookFromElementEvents();
    }

    private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        UnhookFromActivatorEvents();
        HookupToActivatorEvents(e.NewValue as IActivator);
    }

    [MirrorVerified("PtcControlsCoreAttachedBehaviorFocusActivatorMechanicalDifferentialTests")]
    private void ElementGotFocus(object sender, RoutedEventArgs e) => _activator.IsActive = true;

    private void ElementLostFocus(object sender, RoutedEventArgs e)
    {
        if (_activator == null)
        {
            return;
        }
        DependencyObject element = (DependencyObject)_element.GetFocusedElementInSameScope();
        if (!WpfUtils.IsAncestorOf(_element, element))
        {
            _activator.IsActive = false;
        }
    }

    private void HookupToElementEvents(FrameworkElement element)
    {
        if (element != null)
        {
            _element = element;
            _element.DataContextChanged += OnDataContextChanged;
            _element.AddHandler(FocusManager.LostFocusEvent, new RoutedEventHandler(ElementLostFocus), handledEventsToo: true);
            _element.AddHandler(FocusManager.GotFocusEvent, new RoutedEventHandler(ElementGotFocus), handledEventsToo: true);
        }
    }

    private void HookupToActivatorEvents(IActivator activator)
    {
        if (activator != null)
        {
            _activator = activator;
            _activator.Activated += HandleMoveFocus;
            if (_activator.IsActive && !_element.IsFocused)
            {
                HandleMoveFocus(_activator, new EventArgs());
            }
        }
    }

    private void UnhookFromElementEvents()
    {
        if (_element != null)
        {
            _element.RemoveHandler(FocusManager.GotFocusEvent, new RoutedEventHandler(ElementGotFocus));
            _element.RemoveHandler(FocusManager.LostFocusEvent, new RoutedEventHandler(ElementLostFocus));
            _element.DataContextChanged -= OnDataContextChanged;
            _element = null;
        }
    }

    private void UnhookFromActivatorEvents()
    {
        if (_activator != null)
        {
            _activator.Activated -= HandleMoveFocus;
            _activator = null;
        }
    }

    private void HandleMoveFocus(object sender, EventArgs e)
    {
        _element.Dispatcher.BeginInvoke((DispatcherPriority)6, (Delegate)new Action(ActivateElement));
    }

    private void ActivateElement()
    {
        if (!_element.Focusable && _element is IDelegateFocus)
        {
            ((IDelegateFocus)_element).DelegateFocus();
        }
        else
        {
            _element.Focus();
        }
    }

    [MirrorVerified("PtcControlsCoreAttachedBehaviorFocusActivatorMechanicalDifferentialTests")]
    public FocusActivator() { }
}
