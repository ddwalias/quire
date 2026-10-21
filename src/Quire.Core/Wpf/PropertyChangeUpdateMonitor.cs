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
    private void OnPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs args)
    {
        string propertyName = args.PropertyName;
        object oldValue = null;
        object newValue = null;
        if (args is IPropertyChangedEventArgsExtended extendedArgs)
        {
            oldValue = extendedArgs.OldValue;
            newValue = extendedArgs.NewValue;
        }
        if (!_propertiesUpdated.ContainsKey(propertyName))
        {
            _propertiesUpdated.Add(propertyName, (oldValue, newValue));
            return;
        }
        DebugTools.DevAssert(
            _propertiesUpdated[propertyName].Item2 == null || object.Equals(_propertiesUpdated[propertyName].Item2, oldValue),
            null);
        if (_propertiesUpdated[propertyName].Item1 == null)
        {
            _propertiesUpdated[propertyName] = (oldValue, newValue);
        }
        else
        {
            _propertiesUpdated[propertyName] = (_propertiesUpdated[propertyName].Item1, newValue);
        }
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
