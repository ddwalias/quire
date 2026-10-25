#nullable disable

namespace Quire.License;

public sealed partial class LicenseManager
{
    public bool BorrowLicense(global::System.DateTime returnDate)
    {
        if (LicenseBorrowStatus == LicenseBorrowStatus.CanBorrow)
        {
            UpdateLicenseStatusAndRefreshTimer(_provider.BorrowLicense(returnDate, _licenseSource));
            return true;
        }

        return false;
    }

    public bool ReturnLicense()
    {
        bool returned = false;
        if (LicenseBorrowStatus == LicenseBorrowStatus.Borrowed)
        {
            returned = _provider.ReturnLicense();
            if (returned)
            {
                UpdateLicenseStatus();
            }
        }

        return returned;
    }
}
