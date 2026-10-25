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

    [global::System.Runtime.ExceptionServices.HandleProcessCorruptedStateExceptions]
    private bool IsValidLicense()
    {
        try
        {
            return _provider.HasLicense(_licenseSource);
        }
        catch (global::System.Exception exception)
        {
            global::Quire.DebugTools.DevAssert(
                exception is global::System.AccessViolationException,
                "Exception of unknown type occured in license validation.");
            OnDisplayLicenseMessage(new LicenseCbArgs(LicenseMessageName.LicenseRequestFailedE, null, null));
            return false;
        }
    }

    public bool SetLicenseSourceList(string licenseSourceList)
    {
        if (!global::System.String.IsNullOrWhiteSpace(licenseSourceList))
        {
            licenseSourceList = licenseSourceList.Trim();
            if (licenseSourceList.Split(semicolon, global::System.StringSplitOptions.RemoveEmptyEntries)
                .All(source => validServerPortRegex.IsMatch(source) || global::System.IO.File.Exists(source)))
            {
                ClearLicenseProperties();
                _licenseSource.SourceList = licenseSourceList;
                UpdateLicenseStatus();
                return true;
            }
        }

        OnDisplayLicenseMessage(new LicenseCbArgs(LicenseMessageName.ApplyInvalidInputE, null, null));
        return false;
    }
}
