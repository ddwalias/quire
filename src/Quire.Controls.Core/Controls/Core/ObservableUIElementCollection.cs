#nullable disable
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using Quire.Mirror;

namespace Quire.Controls.Core;

public partial class ObservableUIElementCollection
{
    [MirrorVerified("PtcControlsCoreObservableUIElementCollectionMechanicalDifferentialTests")]
    public ObservableUIElementCollection(UIElement visualParent, FrameworkElement logicalParent) : base(visualParent, logicalParent) { }

    [MirrorVerified("PtcControlsCoreObservableUIElementCollectionMechanicalDifferentialTests")]
    private void OnItemChanged() => OnPropertyChanged("Item[]");

    [MirrorVerified("PtcControlsCoreObservableUIElementCollectionMechanicalDifferentialTests")]
    public event NotifyCollectionChangedEventHandler CollectionChanged;

    [MirrorVerified("PtcControlsCoreObservableUIElementCollectionMechanicalDifferentialTests")]
    public event PropertyChangedEventHandler PropertyChanged;

    [MirrorVerified("PtcControlsCoreObservableUIElementCollectionMechanicalDifferentialTests")]
    public event NotifyCollectionChangedEventHandler RangeChanging;

    [MirrorVerified("PtcControlsCoreObservableUIElementCollectionMechanicalDifferentialTests")]
    public event NotifyCollectionChangedEventHandler RangeChanged;
}
