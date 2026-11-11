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
    [MirrorVerified("PtcControlsCoreFrameworkElementFacadeMechanicalDifferentialTests")]
    public FrameworkElementFacade() { }

    public bool IsItemLoaded(object obj)
    {
        return GetFrameworkElement(obj).IsLoaded;
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

    public void ExecuteOnLoad(object obj, Action action)
    {
        WpfUtils.ExecuteOnLoad(GetFrameworkElement(obj), action, false);
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

    public void SetItemName(object item, string name)
    {
        GetFrameworkElement(item).Name = name;
    }

    public string GetItemName(object item)
    {
        return GetFrameworkElement(item).Name;
    }
}
