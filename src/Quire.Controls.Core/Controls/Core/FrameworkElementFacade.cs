#nullable disable
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Threading;
using Quire.Mirror;
using Quire.Wpf;

namespace Quire.Controls.Core;

public partial class FrameworkElementFacade
{
    public bool IsItemLoaded(object obj)
    {
        return GetFrameworkElement(obj).IsLoaded;
    }

    public void SetSize(object obj, Size size, DragMode dragMode)
    {
        FrameworkElement resizableElement = GetResizableElement(obj, dragMode);
        resizableElement.Width = size.Width;
        resizableElement.Height = size.Height;
        resizableElement.RaiseEvent(new RoutedEventArgs(CommonRoutedEvents.ContentChangedEvent));
    }

    private FrameworkElement GetResizableElement(object obj, DragMode dragMode = DragMode.None)
    {
        bool preserveAspectRatio;
        return GetResizableElement(GetFrameworkElement(obj), dragMode, out preserveAspectRatio);
    }

    public Size GetSize(object obj)
    {
        var element = GetResizableElement(obj);
        return new Size(element.Width, element.Height);
    }

    public void SetHeight(object obj, double height)
    {
        FrameworkElement resizableElement = GetResizableElement(obj);
        resizableElement.Height = height;
        resizableElement.RaiseEvent(new RoutedEventArgs(CommonRoutedEvents.ContentChangedEvent));
    }

    public void SetWidth(object obj, double width)
    {
        FrameworkElement resizableElement = GetResizableElement(obj);
        resizableElement.Width = width;
        resizableElement.RaiseEvent(new RoutedEventArgs(CommonRoutedEvents.ContentChangedEvent));
    }

    public void SetMaxSize(object obj, Size maxSize)
    {
        FrameworkElement resizableElement = GetResizableElement(obj);
        resizableElement.MaxWidth = maxSize.Width;
        resizableElement.MaxHeight = maxSize.Height;
    }

    public void ClearMaxSize(object obj)
    {
        FrameworkElement resizableElement = GetResizableElement(obj);
        WpfUtils.ClearValuePreserveBinding(resizableElement, FrameworkElement.MaxWidthProperty, double.PositiveInfinity);
        WpfUtils.ClearValuePreserveBinding(resizableElement, FrameworkElement.MaxHeightProperty, double.PositiveInfinity);
    }

    public void SetItemName(object item, string name)
    {
        GetFrameworkElement(item).Name = name;
    }

    public string GetItemName(object item)
    {
        return GetFrameworkElement(item).Name;
    }

    public void ExecuteOnLoad(object obj, Action action)
    {
        WpfUtils.ExecuteOnLoad(GetFrameworkElement(obj), action, false);
    }

    public void ExecuteOnUnload(object obj, Action action)
    {
        FrameworkElement element = GetFrameworkElement(obj);
        RoutedEventHandler handler = null;
        handler = delegate
        {
            action();
            element.Unloaded -= handler;
        };
        if (element.IsLoaded)
        {
            element.Unloaded += handler;
        }
        else
        {
            action();
        }
    }

    public void ExecuteInBackground(Action action, bool asynch = false)
    {
        if (Application.Current == null)
        {
            action();
            return;
        }
        if (asynch)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, action);
            return;
        }
        Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, action);
    }

    protected static FrameworkElement GetFrameworkElement(object obj)
    {
        var element = obj as FrameworkElement;
        if (element == null)
        {
            throw new ApplicationException("Logic error! Item should be a framework element!");
        }
        return element;
    }

    public object GetParent(object control)
    {
        return GetFrameworkElement(control).Parent;
    }

    public void SetHandleChangesEnabled(object item, bool value)
    {
        var dependencyObject = item as DependencyObject;
        if (DebugTools.AssertNotNull(dependencyObject).Failed)
            return;
        MathFormatAttachedProperties.SetHandleChangesEnabled(dependencyObject, value);
    }

    public void ClearHandleChangesEnabled(object item)
    {
        var dependencyObject = item as DependencyObject;
        if (DebugTools.AssertNotNull(dependencyObject).Failed)
            return;
        MathFormatAttachedProperties.ClearHandleChangesEnabled(dependencyObject);
    }

    public static FrameworkElement GetResizableElement(FrameworkElement elem, DragMode dragMode, out bool preserveAspectRatio)
    {
        var info = elem as IResizableElementInfo;
        if (info != null)
        {
            preserveAspectRatio = info.PreserveAspectRatio;
            var subElement = info.SubElementToResizeData;
            if (subElement != null)
            {
                elem = subElement.Element;
                if (dragMode == DragMode.None)
                    DebugTools.DevAssert(subElement.IsFullSize);
            }
        }
        else
        {
            preserveAspectRatio = false;
        }
        return elem;
    }

    public void SetOverrideLabelColor(object item, bool overideLabelColor)
    {
        var dependencyObject = item as DependencyObject;
        if (DebugTools.AssertNotNull(dependencyObject).Failed)
            return;
        MathFormatAttachedProperties.SetOverrideLabelColor(dependencyObject, overideLabelColor);
    }

    public void SetLabeledIdFormatProperty(object item, object labelIdFormatDictionary)
    {
        var dependencyObject = item as DependencyObject;
        if (DebugTools.AssertNotNull(dependencyObject).Failed)
            return;
        dependencyObject.SetValue(AttachedProperties.LabeledIdFormatProperty, labelIdFormatDictionary);
    }

    public bool GetOverrideLabelColor(object equationVisualElement)
    {
        return MathFormatAttachedProperties.GetOverrideLabelColor(GetFrameworkElement(equationVisualElement));
    }

    public IDictionary<IdLabels, ExtendedFont> GetLabeledIdFormatProperty(object equationVisualElement)
    {
        return (IDictionary<IdLabels, ExtendedFont>)GetFrameworkElement(equationVisualElement).GetValue(AttachedProperties.LabeledIdFormatProperty);
    }

    [MirrorVerified("PtcControlsCoreFrameworkElementFacadeMechanicalDifferentialTests")]
    public FrameworkElementFacade() { }
}
