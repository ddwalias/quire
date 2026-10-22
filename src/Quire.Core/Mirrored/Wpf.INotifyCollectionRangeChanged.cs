#nullable disable
#pragma warning disable

namespace Quire.Wpf;

partial interface INotifyCollectionRangeChanged
{
    event global::System.Collections.Specialized.NotifyCollectionChangedEventHandler RangeChanging;
    event global::System.Collections.Specialized.NotifyCollectionChangedEventHandler RangeChanged;
    int Count();
    bool IsEmpty();
}
