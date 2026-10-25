#nullable disable
using System;

namespace Quire.License;

public partial interface ILicenseProvider
{
    event global::Quire.Delegate1<LicenseCbArgs> DisplayLicenseMessage;

    void SetFastRefresh(bool fast);

    bool HasBorrows();

    DateTime? GetIssueDate();

    DateTime? GetExpirationDate();

    string GetHostId();

    string GetCustomerNumber();

    bool HasLicense(LicenseSource licenseSource);

    bool BorrowLicense(DateTime borrowUntil, LicenseSource licenseSource);

    bool ReturnLicense();

    DateTime? BorrowExpirationDate();

    bool ReleaseLicense();

    bool RefreshLicense();

    bool IsAcademicLicense();

    bool IsHomeUseLicense();

    bool IsSubscriptionLicense();

    bool IsUsingReleaseFeature();

    byte[] GetEncryptionKey(EncryptorVersion encryptorVersion);

    byte[] GetEncryptionInitVector(EncryptorVersion encryptorVersion);

    int UpdateApplicationVariant();
}
