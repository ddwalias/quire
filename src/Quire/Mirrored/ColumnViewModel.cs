#nullable disable

using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;

namespace Quire;

public partial class ColumnViewModel
{
    public static readonly DependencyProperty CollectionProperty = DependencyProperty.Register("Collection", typeof(ListCollectionView), typeof(ColumnViewModel), new PropertyMetadata((PropertyChangedCallback)OnCollectionChanged));

    private void HandleCollectionSortOrderCleared(object sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Reset)
        {
            NotifySortOrderChanged();
        }
    }

    private static void OnCollectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ColumnViewModel columnViewModel = d as ColumnViewModel;

        // Release IL branches on each assertion separately; a combined || emits different branches.
        if (DebugTools.AssertNotNull(columnViewModel, null).Failed)
        {
            return;
        }

        if (DebugTools.AssertNotNull(e.NewValue, null).Failed)
        {
            return;
        }

        columnViewModel._isCollectionInitialized = true;
        columnViewModel.NotifySortOrderChanged();
        if (e.OldValue != null)
        {
            INotifyCollectionChanged sortDescriptions = (INotifyCollectionChanged)((ListCollectionView)e.OldValue).SortDescriptions;
            if (sortDescriptions != null)
            {
                sortDescriptions.CollectionChanged -= columnViewModel.HandleCollectionSortOrderCleared;
            }
        }

        if (e.NewValue != null)
        {
            INotifyCollectionChanged sortDescriptions2 = (INotifyCollectionChanged)((ListCollectionView)e.NewValue).SortDescriptions;
            if (sortDescriptions2 != null)
            {
                sortDescriptions2.CollectionChanged += columnViewModel.HandleCollectionSortOrderCleared;
            }
        }
    }

    public ListCollectionView Collection
    {
        get => (ListCollectionView)GetValue(CollectionProperty);
        set => SetValue(CollectionProperty, value);
    }

    public string ColumnDataName { get; set; }

    public void NotifySortOrderChanged()
    {
        FirePropertyChanged("CurrentDirection");
    }

    public void SortByNextOrder()
    {
        ListSortDirection? nextSortDirection = GetNextSortDirection();
        INotifyCollectionChanged sortDescriptions = (INotifyCollectionChanged)Collection.SortDescriptions;
        if (sortDescriptions != null)
        {
            sortDescriptions.CollectionChanged -= HandleCollectionSortOrderCleared;
        }

        DebugTools.DevAssert(Collection.SortDescriptions.Count < 2, null);
        Collection.SortDescriptions.Clear();
        if (sortDescriptions != null)
        {
            sortDescriptions.CollectionChanged += HandleCollectionSortOrderCleared;
        }

        if (nextSortDirection.HasValue)
        {
            Collection.SortDescriptions.Add(new SortDescription(ColumnDataName, nextSortDirection.Value));
        }

        NotifySortOrderChanged();
    }

    public ListSortDirection? CurrentDirection
    {
        get
        {
            if (_isCollectionInitialized && DebugTools.AssertNotNull(Collection, "Unexpected situation, where collection was set, but still is null.").Passed)
            {
                if (Collection.SortDescriptions.Count != 0)
                {
                    // Release IL compares the name with != and returns the direction on the else path.
                    if (Collection.SortDescriptions[0].PropertyName != ColumnDataName)
                    {
                    }
                    else
                    {
                        return Collection.SortDescriptions[0].Direction;
                    }
                }

                return null;
            }

            return null;
        }
    }

    private ListSortDirection? GetNextSortDirection()
    {
        switch (CurrentDirection)
        {
            case ListSortDirection.Ascending:
                return ListSortDirection.Descending;
            case ListSortDirection.Descending:
                return null;
            case null:
                return ListSortDirection.Ascending;
            default:
                DebugTools.FailAssertion("Unexpected value of 'CurrentDirection' property.");
                break;
        }

        return null;
    }

    public event PropertyChangedEventHandler PropertyChanged;

    private void FirePropertyChanged(string propertyName)
    {
        if (PropertyChanged != null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public ColumnViewModel()
    {
    }
}
