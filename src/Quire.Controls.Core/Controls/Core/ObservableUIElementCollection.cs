#nullable disable
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using Quire.Mirror;
using Quire.Wpf;

namespace Quire.Controls.Core;

public partial class ObservableUIElementCollection
{
    [MirrorVerified("PtcControlsCoreObservableUIElementCollectionMechanicalDifferentialTests")]
    public ObservableUIElementCollection(UIElement visualParent, FrameworkElement logicalParent) : base(visualParent, logicalParent) { }

    public override UIElement this[int index]
    {
        get => base[index];
        set
        {
            UIElement oldItem = base[index];
            var e = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, oldItem, value, index);
            OnRangeChanging(e);
            base[index] = value;
            OnItemChanged();
            OnCollectionChanged(e);
        }
    }

    public override int Add(UIElement element)
    {
        int index = base.Count;
        var e = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, element, index);
        OnRangeChanging(e);
        int result = base.Add(element);
        OnChanged(e);
        return result;
    }

    public override void Remove(UIElement element)
    {
        var e = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, element);
        OnRangeChanging(e);
        base.Remove(element);
        OnChanged(e);
    }

    public override void Clear()
    {
        var removed = new ArrayList(this);
        NotifyCollectionChangedEventArgs e = removed.Count > 0
            ? new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, removed)
            : null;
        if (e != null)
        {
            OnRangeChanging(e);
        }
        base.Clear();
        OnChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset), range: false);
        if (e != null)
        {
            OnRangeChanged(e);
        }
    }

    public override bool Equals(object obj)
    {
        // Release IL tests the type with `is`, then casts with `as` in a separate branch and keeps one result flag.
        bool flag = obj is ObservableUIElementCollection;
        if (flag)
        {
            ObservableUIElementCollection other = obj as ObservableUIElementCollection;
            flag = Count == other.Count;
            if (flag)
            {
                for (int i = 0; i < Count; i++)
                {
                    flag = this[i].Equals(other[i]);
                    if (!flag)
                    {
                        break;
                    }
                }
            }
        }
        return flag;
    }

    public override int GetHashCode()
    {
        int hash = 0;
        foreach (object item in this)
        {
            hash |= item.GetHashCode();
        }
        return hash;
    }

    public override void Insert(int index, UIElement element)
    {
        var e = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, element, index);
        OnRangeChanging(e);
        base.Insert(index, element);
        OnChanged(e);
    }

    public override void RemoveAt(int index)
    {
        UIElement removed = this[index];
        var e = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, removed);
        OnRangeChanging(e);
        base.RemoveAt(index);
        OnChanged(e);
    }

    public override void RemoveRange(int index, int count)
    {
        var removed = new List<UIElement>(count);
        for (int i = 0; i < count; i++)
        {
            removed.Add(this[index + i]);
        }
        var e = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, removed, index);
        OnRangeChanging(e);
        base.RemoveRange(index, count);
        OnChanged(e);
    }

    private void OnChanged(NotifyCollectionChangedEventArgs e, bool range = true)
    {
        OnPropertiesChanged();
        OnCollectionChanged(e, range);
    }

    [MirrorVerified("PtcControlsCoreObservableUIElementCollectionMechanicalDifferentialTests")]
    private void OnItemChanged() => OnPropertyChanged("Item[]");

    private void OnPropertiesChanged()
    {
        OnPropertyChanged("Count");
        OnItemChanged();
    }

    private void OnPropertyChanged(string propertyName)
    {
        OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
    }

    protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        if (PropertyChanged != null)
        {
            PropertyChanged(this, e);
        }
    }

    protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e, bool range = true)
    {
        if (range && RangeChanging != null)
        {
            RangeChanging(this, e);
        }
        if (CollectionChanged != null)
        {
            CollectionChanged(this, e);
        }
        if (range && RangeChanged != null)
        {
            RangeChanged(this, e);
        }
    }

    protected virtual void OnRangeChanging(NotifyCollectionChangedEventArgs e)
    {
        if (RangeChanging != null)
        {
            RangeChanging(this, e);
        }
    }

    protected virtual void OnRangeChanged(NotifyCollectionChangedEventArgs e)
    {
        if (RangeChanged != null)
        {
            RangeChanged(this, e);
        }
    }

    int INotifyCollectionRangeChanged.Count() => Count;

    public bool IsEmpty() => Count == 0;

    [MirrorVerified("PtcControlsCoreObservableUIElementCollectionMechanicalDifferentialTests")]
    public event NotifyCollectionChangedEventHandler CollectionChanged;

    [MirrorVerified("PtcControlsCoreObservableUIElementCollectionMechanicalDifferentialTests")]
    public event PropertyChangedEventHandler PropertyChanged;

    [MirrorVerified("PtcControlsCoreObservableUIElementCollectionMechanicalDifferentialTests")]
    public event NotifyCollectionChangedEventHandler RangeChanging;

    [MirrorVerified("PtcControlsCoreObservableUIElementCollectionMechanicalDifferentialTests")]
    public event NotifyCollectionChangedEventHandler RangeChanged;
}
