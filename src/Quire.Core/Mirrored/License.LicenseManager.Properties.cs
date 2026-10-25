#nullable disable

namespace Quire.License;

public sealed partial class LicenseManager
{
    public static ILicenseManager Instance => _instance;

    public bool HasValidLicense
    {
        get => _hasValidLicense.GetValueOrDefault();
        private set => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    }

    public int LicenseRefreshInterval
    {
        get => _licenseRefreshInterval;
        set => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    }

    public LicenseType LicenseType
    {
        get => _licenseType;
        private set => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    }

    public global::System.DateTime? IssueDate
    {
        get => _issueDate;
        private set => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    }

    public global::System.DateTime? ExpirationDate
    {
        get => _expirationDate;
        private set => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    }

    public string HostId
    {
        get => _hostId;
        private set => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    }

    public string ProductCode
    {
        get => _productCode;
        private set => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    }

    public string SalesOrderNumber
    {
        get => _salesOrderNumber;
        private set => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    }

    public string CustomerNumber
    {
        get => _customerNumber;
        private set => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    }

    public bool IsEngNbPlus
    {
        get => _isEngNbPlus;
        private set => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    }

    public bool IsEngNb
    {
        get => _isEngNb;
        private set => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    }

    public bool IsEngNotebookLostConnection
    {
        get => _isEngNotebookLostConnection;
        private set => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    }

    public LicenseSource LicenseSourceInfo => _licenseSource;

    public string LicenseMessageText
    {
        get => _licenseMessageText;
        private set => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    }

    public LicenseBorrowStatus LicenseBorrowStatus
    {
        get => _licenseBorrowStatus;
        private set => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    }

    public global::System.DateTime? BorrowedUntil
    {
        get => _borrowedUntil;
        private set => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    }

    public bool IsEducationalLicense
    {
        get => _isEducationalLicense;
        private set => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    }

    public bool IsUsingReleaseFeature() => _isUsingReleaseFeature;
}
