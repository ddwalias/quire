#nullable disable

namespace Quire.License;

public sealed partial class LicenseManager
{
    private LicenseType DetermineLicenseType(bool hasValidLicense)
    {
        LicenseType licenseType;
        bool hasNoExpirationDate;
        bool isHomeUseLicense;
        global::System.DateTime? expirationDate;
        global::System.DateTime? issueDate;

        if (!hasValidLicense)
        {
            return LicenseType.NoLicense;
        }

        expirationDate = ExpirationDate;
        hasNoExpirationDate = !expirationDate.HasValue;
        isHomeUseLicense = _provider.IsHomeUseLicense();
        if (_provider.IsSubscriptionLicense())
        {
            licenseType = IsEducationalLicense ? LicenseType.EducationSub : LicenseType.CommercialSub;
        }
        else if (isHomeUseLicense)
        {
            licenseType = IsEducationalLicense ? LicenseType.EducationTerm : LicenseType.CommercialTerm;
        }
        else if (hasNoExpirationDate)
        {
            licenseType = IsEducationalLicense ? LicenseType.Education : LicenseType.Commercial;
        }
        else
        {
            expirationDate = ExpirationDate;
            issueDate = IssueDate;
            if ((expirationDate.HasValue & issueDate.HasValue)
                ? expirationDate.GetValueOrDefault() >= issueDate.GetValueOrDefault()
                : false)
            {
                issueDate = ExpirationDate;
                expirationDate = IssueDate;
                if ((issueDate.HasValue & expirationDate.HasValue
                    ? issueDate.GetValueOrDefault() - expirationDate.GetValueOrDefault()
                    : (global::System.TimeSpan?)null) <= _oneMonth)
                {
                    licenseType = LicenseType.Trial;
                    goto ReturnLicenseType;
                }
            }

            global::Quire.DebugTools.FailAssertion(
                "IssueDate: {0}, ExpirationDate: {1}, isAcademic: {2}",
                IssueDate,
                ExpirationDate,
                IsEducationalLicense);
            licenseType = IsEducationalLicense ? LicenseType.EducationTerm : LicenseType.CommercialTerm;
        }

        // Keep the trial path on the same return join as Mathcad.
    ReturnLicenseType:
        return licenseType;
    }
}
