#nullable disable

namespace Quire.License;

public sealed partial class LicenseManager
{
    public event global::System.ComponentModel.PropertyChangedEventHandler PropertyChanged
    {
        add => _propertyChanged.Combine(value);
        remove => _propertyChanged.Remove(value);
    }

    public event global::System.EventHandler LicenseTypeChanged
    {
        add => _licenseTypeChanged.Combine(value);
        remove => _licenseTypeChanged.Remove(value);
    }

    private void OnLicenseTypeChanged(global::System.EventArgs args)
    {
        if (_licenseTypeChanged != null)
        {
            _licenseTypeChanged.Invoke([this, args]);
        }
    }
}
