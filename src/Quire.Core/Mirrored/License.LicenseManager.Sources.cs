#nullable disable

namespace Quire.License;

public sealed partial class LicenseManager
{
    public bool SetEnterpriseLicenseServer(string host, uint port)
    {
        if (global::System.String.IsNullOrEmpty(host) || port == 0)
        {
            OnDisplayLicenseMessage(new LicenseCbArgs(LicenseMessageName.ApplyInvalidInputE, null, null));
            return false;
        }

        ClearLicenseProperties();
        _licenseSource.Host = host;
        _licenseSource.Port = port;
        UpdateLicenseStatus();
        return true;
    }

    public bool SetLicenseFile(string fileName)
    {
        LicenseMessageName messageName = LicenseMessageName.ApplyInvalidInputE;
        bool isValidFileName = IsValidFileName(fileName);
        if (isValidFileName && !global::System.IO.File.Exists(fileName))
        {
            isValidFileName = false;
            messageName = LicenseMessageName.LicenseFileNotFoundE;
        }

        if (isValidFileName)
        {
            ClearLicenseProperties();
            _licenseSource.FilePath = fileName;
            UpdateLicenseStatus();
        }
        else
        {
            OnDisplayLicenseMessage(new LicenseCbArgs(messageName, null, null));
        }

        return isValidFileName;
    }
}
