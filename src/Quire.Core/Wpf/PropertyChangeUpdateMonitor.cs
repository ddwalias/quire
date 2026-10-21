#nullable disable
using Quire.Mirror;

namespace Quire.Wpf;

public partial class PropertyChangeUpdateMonitor
{
    [MirrorVerified("Release IL")]
    public PropertyChangeUpdateMonitor(
        System.ComponentModel.INotifyPropertyChanged objectToMonitor,
        System.Action<System.Collections.Generic.IDictionary<string, System.ValueTuple<object, object>>> actionToPerform)
    {
        _objectToMonitor = objectToMonitor;
        _objectToMonitor.PropertyChanged += OnPropertyChanged;
        _propertiesUpdated = new System.Collections.Generic.Dictionary<string, System.ValueTuple<object, object>>();
        _actionToPerform = actionToPerform;
    }

    [MirrorVerified("Release IL")]
    public void Dispose()
    {
        _objectToMonitor.PropertyChanged -= OnPropertyChanged;
        if (_propertiesUpdated != null)
        {
            _actionToPerform(_propertiesUpdated);
        }
        _propertiesUpdated = null;
    }
}
