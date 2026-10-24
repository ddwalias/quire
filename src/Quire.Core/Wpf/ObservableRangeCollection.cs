#nullable disable
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Specialized;
using Quire.Mirror;
using Quire.Mirror;

namespace Quire.Wpf;

public partial class ObservableRangeCollection<T>
{
    private bool _fireRangeChangesAllowed = true;

    public ObservableRangeCollection() { }

    public ObservableRangeCollection(IEnumerable<T> collection) : base(collection) { }

    [MirrorVerified("PtcWpfObservableRangeCollection1MechanicalDifferentialTests")]
    public event NotifyCollectionChangedEventHandler RangeChanging;

    [MirrorVerified("PtcWpfObservableRangeCollection1MechanicalDifferentialTests")]
    public event NotifyCollectionChangedEventHandler RangeChanged;

    int INotifyCollectionRangeChanged.Count()
    {
        return Count;
    }

    private void FireRangeChanged(NotifyCollectionChangedEventArgs args)
    {
        if (_fireRangeChangesAllowed)
        {
            OnRangeChanged(args);
        }
    }

    protected virtual void OnRangeChanged(NotifyCollectionChangedEventArgs args)
    {
        if (RangeChanged != null)
        {
            RangeChanged(this, args);
        }
    }

    private void FireRangeChanging(NotifyCollectionChangedEventArgs args)
    {
        if (_fireRangeChangesAllowed)
        {
            OnRangeChanging(args);
        }
    }

    protected virtual void OnRangeChanging(NotifyCollectionChangedEventArgs args)
    {
        if (RangeChanging != null)
        {
            RangeChanging(this, args);
        }
    }

    [MirrorVerified("Release IL")]
    public bool IsEmpty() => base.Count == 0;

    public void AddRange(IEnumerable<T> itemsToAdd)
    {
        if (global::Quire.DebugTools.AssertNotNull(itemsToAdd, null).Failed)
        {
            return;
        }
        List<T> list = itemsToAdd.ToList();
        int count = Count;
        if (list.Count > 0)
        {
            FireRangeChanging(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, list, count));
        }
        using (new RangeChangeLock(this))
        {
            list.Each(Add);
        }
        if (list.Count > 0)
        {
            FireRangeChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, list, count));
        }
    }

    public int RemoveRange(IEnumerable<T> itemsToRemove)
    {
        if (global::Quire.DebugTools.AssertNotNull(itemsToRemove, null).Failed)
        {
            return 0;
        }
        if (!itemsToRemove.Any())
        {
            return 0;
        }
        List<T> remainingItems = Items.ToList();
        itemsToRemove.Each((T item) =>
        {
            remainingItems.Remove(item);
        });
        int removedCount = Count - remainingItems.Count;
        if (removedCount != 0)
        {
            List<T> removedItems = new List<T>();
            List<T> currentRemovedItems;
            int itemIndex = 0;
            int remainingIndex = 0;
            for (; itemIndex < Items.Count && remainingIndex <= remainingItems.Count; itemIndex++)
            {
                if (remainingIndex < remainingItems.Count && object.Equals(Items[itemIndex], remainingItems[remainingIndex]))
                {
                    remainingIndex++;
                }
                else
                {
                    removedItems.Add(Items[itemIndex]);
                }
            }
            currentRemovedItems = new List<T>();
            int currentIndex = 0;
            int removedIndex = 0;
            for (; currentIndex <= Items.Count && removedIndex <= removedItems.Count; currentIndex++)
            {
                if (removedIndex < removedItems.Count && object.Equals(Items[currentIndex], removedItems[removedIndex]))
                {
                    currentRemovedItems.Add(Items[currentIndex]);
                    removedIndex++;
                }
                else
                {
                    if (currentRemovedItems.Count <= 0)
                    {
                        continue;
                    }
                    currentIndex -= currentRemovedItems.Count;
                    FireRangeChanging(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, currentRemovedItems, currentIndex));
                    using (new RangeChangeLock(this))
                    {
                        for (int removeCount = currentRemovedItems.Count; removeCount > 0; removeCount--)
                        {
                            RemoveAt(currentIndex);
                        }
                    }
                    FireRangeChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, currentRemovedItems, currentIndex));
                    currentRemovedItems = new List<T>();
                }
            }
            global::Quire.DebugTools.DevAssert(Count == remainingItems.Count, null);
        }
        return removedCount;
    }

    [MirrorVerified("Release IL")]
    protected override void InsertItem(int index, T item)
    {
        FireRangeChanging(new NotifyCollectionChangedEventArgs((NotifyCollectionChangedAction)0, item, index));
        base.InsertItem(index, item);
    }

    [MirrorVerified("Release IL")]
    protected override void MoveItem(int oldIndex, int newIndex)
    {
        FireRangeChanging(new NotifyCollectionChangedEventArgs((NotifyCollectionChangedAction)3, base[oldIndex], oldIndex, newIndex));
        base.MoveItem(oldIndex, newIndex);
    }

    [MirrorVerified("Release IL")]
    protected override void RemoveItem(int index)
    {
        FireRangeChanging(new NotifyCollectionChangedEventArgs((NotifyCollectionChangedAction)1, base[index], index));
        base.RemoveItem(index);
    }

    [MirrorVerified("Release IL")]
    protected override void SetItem(int index, T item)
    {
        FireRangeChanging(new NotifyCollectionChangedEventArgs((NotifyCollectionChangedAction)2, item, base[index], index));
        base.SetItem(index, item);
    }

    [MirrorVerified("Release IL")]
    protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
    {
        base.OnCollectionChanged(args);
        FireRangeChanged(args);
    }

    protected override void ClearItems()
    {
        List<T> list = Items.ToList();
        if (list.Count > 0)
        {
            FireRangeChanging(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, list, 0));
        }
        using (new RangeChangeLock(this))
        {
            base.ClearItems();
        }
        if (list.Count > 0)
        {
            FireRangeChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, list, 0));
        }
    }

    partial class RangeChangeLock
    {
        [MirrorVerified("Release IL")]
        private readonly bool _oldAllowed;

        [MirrorVerified("Release IL")]
        private readonly ObservableRangeCollection<T> _collecton;

        [MirrorVerified("Release IL")]
        public RangeChangeLock(ObservableRangeCollection<T> collection)
        {
            _collecton = collection;
            _oldAllowed = collection._fireRangeChangesAllowed;
            _collecton._fireRangeChangesAllowed = false;
        }

        [MirrorVerified("Release IL")]
        public void Dispose() => _collecton._fireRangeChangesAllowed = _oldAllowed;
    }
}
