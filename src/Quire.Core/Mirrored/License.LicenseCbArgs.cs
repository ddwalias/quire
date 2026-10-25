#nullable disable

namespace Quire.License;

public partial class LicenseCbArgs
{
    private readonly string _message;
    private readonly bool? _licenseIsValid;
    private readonly LicenseMessageName _messageName;

    public LicenseCbArgs(LicenseMessageName messageName, string message, bool? licenseIsValid)
    {
        _messageName = messageName;
        _message = message;
        _licenseIsValid = licenseIsValid;
    }

    public string Message => _message;

    public bool? LicenseIsValid => _licenseIsValid;

    public LicenseMessageName MessageName => _messageName;
}
