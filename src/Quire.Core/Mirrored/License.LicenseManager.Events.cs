#nullable disable

namespace Quire.License;

public sealed partial class LicenseManager
{
    private void OnLicenseTypeChanged(global::System.EventArgs args)
    {
        if (_licenseTypeChanged != null)
        {
            _licenseTypeChanged.Invoke([this, args]);
        }
    }
}
