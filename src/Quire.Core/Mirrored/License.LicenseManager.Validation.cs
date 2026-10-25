#nullable disable

namespace Quire.License;

public sealed partial class LicenseManager
{
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
}
