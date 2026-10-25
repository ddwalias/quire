#nullable disable
using System;
using System.Windows;
using Quire;
using Quire.Mirror;

namespace Quire.Controls.Core.AttachedBehavior;

public partial class PropertyValueCollection : ResourceDictionary, IAttachedBehavior
{
    public void Attach(FrameworkElement element)
    {
        element.Loaded += OnLoaded;
        element.Unloaded += OnUnloaded;
        if (element.IsLoaded)
        {
            OnLoaded(element);
        }
    }

    public void Detach(FrameworkElement element)
    {
        element.Loaded -= OnLoaded;
        element.Unloaded -= OnUnloaded;
        if (element.IsLoaded)
        {
            OnUnloaded(element);
        }
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        OnLoaded(sender as FrameworkElement);
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        OnUnloaded(sender as FrameworkElement);
    }

    [MirrorVerified("PtcControlsCoreAttachedBehaviorPropertyValueCollectionMechanicalDifferentialTests")]
    protected virtual void OnLoaded(FrameworkElement element) { }

    [MirrorVerified("PtcControlsCoreAttachedBehaviorPropertyValueCollectionMechanicalDifferentialTests")]
    protected virtual void OnUnloaded(FrameworkElement element) { }

    protected void AddValuesToResources(FrameworkElement element)
    {
        element.Resources.MergedDictionaries.Add(this);
    }

    protected void RemoveValuesFromResources(FrameworkElement element)
    {
        element.Resources.MergedDictionaries.Remove(this);
    }

    protected void ForEachDictionary(Action<ResourceDictionary> action)
    {
        action(this);
        foreach (ResourceDictionary mergedDictionary in MergedDictionaries)
        {
            action(mergedDictionary);
            DebugTools.DevAssert(mergedDictionary.MergedDictionaries.IsNullOrEmpty(), null);
        }
    }

    [MirrorVerified("PtcControlsCoreAttachedBehaviorPropertyValueCollectionMechanicalDifferentialTests")]
    public PropertyValueCollection() { }
}
