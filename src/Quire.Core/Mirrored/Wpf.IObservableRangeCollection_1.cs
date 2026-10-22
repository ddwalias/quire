#nullable disable
#pragma warning disable

namespace Quire.Wpf;

partial interface IObservableRangeCollection<T>
{
    void AddRange(global::System.Collections.Generic.IEnumerable<T> itemsToAdd);
    int RemoveRange(global::System.Collections.Generic.IEnumerable<T> itemsToRemove);
}
