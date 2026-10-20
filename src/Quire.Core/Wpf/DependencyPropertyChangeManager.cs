#nullable disable
using System.Windows;
using System.Windows.Data;
using System.Windows;
using Quire.Mirror;
using Quire.Mirror;

namespace Quire.Wpf;

public static partial class DependencyPropertyChangeManager
{
    partial class DependencyPropertyChangeNotifier
    {
        [MirrorVerified("PtcWpfDependencyPropertyChangeManagerMechanicalDifferentialTests")]
        public event DependencyPropertyChangedEventHandler SourcePropertyChanged;
    }

    partial class DependencyPropertyChangeNotifier
    {
        [MirrorVerified("Release IL")]
        public bool HasSourcePropertyChangedHandlers() => SourcePropertyChanged != null;

        [MirrorVerified("Release IL")]
        public static readonly DependencyProperty ValueProperty;

        [MirrorVerified("Release IL")]
        public void Dispose()
        {
            SourcePropertyChanged = null;
            BindingOperations.ClearBinding(this, ValueProperty);
        }
    }
}
