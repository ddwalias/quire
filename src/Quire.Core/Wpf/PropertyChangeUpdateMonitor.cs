#nullable disable
using Quire.Mirror;

namespace Quire.Wpf;

public partial class PropertyChangeUpdateMonitor
{
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
