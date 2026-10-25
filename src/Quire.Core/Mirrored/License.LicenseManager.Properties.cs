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
        set
        {
            if (value <= 0)
            {
                _licenseRefreshInterval = 3600000;
                _provider.SetFastRefresh(false);
            }
            else
            {
                _licenseRefreshInterval = value * 1000;
                _provider.SetFastRefresh(true);
            }

            ApplyLicenseRefreshInterval();
        }
    }

    public LicenseType LicenseType
    {
        get => _licenseType;
        private set
        {
            if (_licenseType != value)
            {
                _licenseType = value;
                FirePropertyChanged("LicenseType");
                OnLicenseTypeChanged(global::System.EventArgs.Empty);
            }
        }
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
        private set
        {
            if (_hostId != value)
            {
                _hostId = value;
                FirePropertyChanged("HostId");
            }
        }
    }

    public string ProductCode
    {
        get => _productCode;
        private set
        {
            if (_productCode != value)
            {
                _productCode = value;
                FirePropertyChanged("ProductCode");
            }
        }
    }

    public string SalesOrderNumber
    {
        get => _salesOrderNumber;
        private set
        {
            if (_salesOrderNumber != value)
            {
                _salesOrderNumber = value;
                FirePropertyChanged("SalesOrderNumber");
            }
        }
    }

    public string CustomerNumber
    {
        get => _customerNumber;
        private set
        {
            if (_customerNumber != value)
            {
                _customerNumber = value;
                FirePropertyChanged("CustomerNumber");
            }
        }
    }

    public bool IsEngNbPlus
    {
        get => _isEngNbPlus;
        private set
        {
            if (_isEngNbPlus != value)
            {
                _isEngNbPlus = value;
                FirePropertyChanged("IsEngNbPlus");
            }
        }
    }

    public bool IsEngNb
    {
        get => _isEngNb;
        private set
        {
            if (_isEngNb != value)
            {
                _isEngNb = value;
                FirePropertyChanged("IsEngNb");
            }
        }
    }

    public bool IsEngNotebookLostConnection
    {
        get => _isEngNotebookLostConnection;
        private set
        {
            if (_isEngNotebookLostConnection != value)
            {
                _isEngNotebookLostConnection = value;
                FireEngNotebookConnectionStatusChanged();
            }
        }
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

    private bool HasBorrows() => _provider.HasBorrows();

    private void ApplyLicenseRefreshInterval()
    {
        if (_refreshTimer != null)
        {
            _refreshTimer.Change(LicenseRefreshInterval, LicenseRefreshInterval);
        }
    }
}
