#nullable disable

namespace Quire.License;

public sealed partial class LicenseManager
{
    private void ClearLicenseProperties(bool keepLicenseLocation = false)
    {
        _licenseMessages.Clear();
        HasValidLicense = false;
        LicenseType = LicenseType.NoLicense;
        IssueDate = null;
        ExpirationDate = null;
        LicenseBorrowStatus = LicenseBorrowStatus.CannotBorrow;
        BorrowedUntil = null;
        ProductCode = null;
        CustomerNumber = null;
        SalesOrderNumber = null;
        IsEducationalLicense = false;
        if (!keepLicenseLocation)
        {
            _licenseSource.Clear();
        }
    }

}
