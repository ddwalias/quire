#nullable disable
using System.Windows;
using Quire.Mirror;

namespace Quire.Wpf;

public static partial class DependencyPropertyChangeManager
{
    partial class DependencyPropertyChangeNotifier
    {
        [MirrorVerified("PtcWpfDependencyPropertyChangeManagerMechanicalDifferentialTests")]
        public event DependencyPropertyChangedEventHandler SourcePropertyChanged;
    }
}
