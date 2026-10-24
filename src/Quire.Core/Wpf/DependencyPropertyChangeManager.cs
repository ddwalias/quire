#nullable disable
using System.Windows;
using System.Windows.Data;
using System.Windows;
using System.Collections.Generic;
using Quire.Mirror;
using Quire.Mirror;

namespace Quire.Wpf;

public static partial class DependencyPropertyChangeManager
{
    private static IDictionary<DependencyProperty, DependencyPropertyChangeNotifier> GetPropertyChangeNotifiers(DependencyObject dObj, bool create)
    {
        if (DebugTools.AssertNotNull(dObj, null).Failed)
        {
            return null;
        }
        IDictionary<DependencyProperty, DependencyPropertyChangeNotifier> dictionary = dObj.GetValue(_propertyChangeNotifiersKey.DependencyProperty) as IDictionary<DependencyProperty, DependencyPropertyChangeNotifier>;
        if ((dictionary == null) & create)
        {
            dictionary = new Dictionary<DependencyProperty, DependencyPropertyChangeNotifier>();
            dObj.SetValue(_propertyChangeNotifiersKey, (object)dictionary);
        }
        return dictionary;
    }

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
