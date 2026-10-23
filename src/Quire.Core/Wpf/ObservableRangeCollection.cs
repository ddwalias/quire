#nullable disable
using System.Collections.Specialized;
using System.Collections.Generic;
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

    [MirrorVerified("Release IL")]
    public bool IsEmpty() => base.Count == 0;

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
