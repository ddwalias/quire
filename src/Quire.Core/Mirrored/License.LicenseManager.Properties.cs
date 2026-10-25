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
        private set
        {
            global::System.DateTime? currentValue = _issueDate;
            global::System.DateTime? newValue = value;
            if (currentValue.HasValue == newValue.HasValue
                    ? (currentValue.HasValue
                        ? currentValue.GetValueOrDefault() != newValue.GetValueOrDefault()
                        : false)
                    : true)
            {
                _issueDate = value;
                FirePropertyChanged("IssueDate");
            }
        }
    }

    public global::System.DateTime? ExpirationDate
    {
        get => _expirationDate;
        private set
        {
            global::System.DateTime? currentValue = _expirationDate;
            global::System.DateTime? newValue = value;
            if (currentValue.HasValue == newValue.HasValue
                    ? (currentValue.HasValue
                        ? currentValue.GetValueOrDefault() != newValue.GetValueOrDefault()
                        : false)
                    : true)
            {
                _expirationDate = value;
                FirePropertyChanged("ExpirationDate");
                FirePropertyChanged("DaysLeft");
            }
        }
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

    public bool IsNeededDisableLicenseTab
    {
        get
        {
            bool? isEngineeringNotebook = global::Quire.Services.ApplicationVariant.IsEngNb;
            if (!isEngineeringNotebook.GetValueOrDefault())
            {
                isEngineeringNotebook = global::Quire.Services.ApplicationVariant.IsEngNbPlus;
                return isEngineeringNotebook.GetValueOrDefault();
            }

            return true;
        }
    }

    public LicenseSource LicenseSourceInfo => _licenseSource;

    public string LicenseMessageText
    {
        get => _licenseMessageText;
        private set
        {
            if (value != _licenseMessageText)
            {
                _licenseMessageText = value;
                FirePropertyChanged("LicenseMessageText");
            }
        }
    }

    public LicenseBorrowStatus LicenseBorrowStatus
    {
        get => _licenseBorrowStatus;
        private set
        {
            if (_licenseBorrowStatus != value)
            {
                _licenseBorrowStatus = value;
                FirePropertyChanged("LicenseBorrowStatus");
            }

            UpdateBorrowedUntil(value);
        }
    }

    public global::System.DateTime? BorrowedUntil
    {
        get => _borrowedUntil;
        private set
        {
            global::System.DateTime? currentValue = _borrowedUntil;
            global::System.DateTime? newValue = value;
            if (currentValue.HasValue == newValue.HasValue
                    ? (currentValue.HasValue
                        ? currentValue.GetValueOrDefault() != newValue.GetValueOrDefault()
                        : false)
                    : true)
            {
                _borrowedUntil = value;
                FirePropertyChanged("BorrowedUntil");
            }
        }
    }

    public bool IsEducationalLicense
    {
        get => _isEducationalLicense;
        private set
        {
            if (_isEducationalLicense != value)
            {
                _isEducationalLicense = value;
                FirePropertyChanged("IsEducationalLicense");
            }
        }
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

    private void UpdateIssueDate() => IssueDate = _provider.GetIssueDate();

    private void UpdateExpirationDate() => ExpirationDate = _provider.GetExpirationDate();

    private void UpdateHostId() => HostId = _provider?.GetHostId();

    private void UpdateCustomerNumber() => CustomerNumber = _provider.GetCustomerNumber();

    private void UpdateIsEducationalLicense() => IsEducationalLicense = _provider.IsAcademicLicense();

    private void UpdateLicenseBorrowStatus()
    {
        LicenseBorrowStatus status = LicenseBorrowStatus.CannotBorrow;
        if (HasBorrows())
        {
            status = LicenseBorrowStatus.Borrowed;
        }
        else if (_licenseSource.Type == LicenseUriType.ServerAndPort
            || _licenseSource.Type == LicenseUriType.SourceList)
        {
            status = LicenseBorrowStatus.CanBorrow;
        }

        LicenseBorrowStatus = status;
    }

    private void UpdateLicenseStatus()
    {
        UpdateLicenseStatusAndRefreshTimer(IsValidLicense());
        _licenseSource.Save(LicenseType);
    }

    private void UpdateLicenseStatusAndRefreshTimer(bool hasValidLicense)
    {
        HasValidLicense = hasValidLicense;
        if (HasValidLicense && DebugTools.AssertNotNull(_provider).Passed)
        {
            if (_refreshTimer == null)
            {
                _refreshTimer = new global::System.Threading.Timer(DispatchedRefreshCallback);
            }

            ApplyLicenseRefreshInterval();
            return;
        }

        if (_refreshTimer != null)
        {
            _refreshTimer.Dispose();
            _refreshTimer = null;
        }
    }

    private void FirePropertyChanged(string propertyName)
    {
        if (_propertyChanged != null)
        {
            _propertyChanged.Invoke(
                [this, new global::System.ComponentModel.PropertyChangedEventArgs(propertyName)]);
        }
    }

    private void UpdateProductCodeAndDerivedValues()
    {
        ProductCode productCode;
        string productCodeValue;
        string customerNumber;
        string salesOrderNumber;
        if (LicenseSourceInfo.Type == LicenseUriType.FilePath)
        {
            try
            {
                string filePath = LicenseSourceInfo.FilePath;
                string fileProductCode = global::System.IO.File.Exists(filePath)
                    ? LicenseConfigProvider.GetProductCode(GetLicenseHashCodesFromFile(filePath))
                    : null;
                productCode = global::Quire.EnumerableExtensions.IsNullOrEmpty<char>(fileProductCode)
                    ? null
                    : new ProductCode { Code = fileProductCode };
            }
            catch (global::System.Exception exception)
            {
                productCode = null;
                DebugTools.FailAssertion(exception.Message);
            }
        }
        else
        {
            productCode = null;
        }

        productCodeValue = null;
        customerNumber = null;
        salesOrderNumber = null;
        if (productCode != null && DebugTools.DevAssert(productCode.IsValid).Passed)
        {
            productCodeValue = productCode.ToString();
            salesOrderNumber = productCode.SON;
            customerNumber = productCode.CustomerNumber;
        }

        ProductCode = productCodeValue;
        SalesOrderNumber = salesOrderNumber;
        CustomerNumber = customerNumber;
    }

    private static string GetLicensePathSettingFromHostAndPort(string licenseHost, uint? licensePort)
    {
        if (!licensePort.HasValue || string.IsNullOrEmpty(licenseHost))
        {
            return string.Empty;
        }

        uint? port = licensePort;
        return string.Concat(port.ToString(), "@", licenseHost);
    }

    private void UpdateBorrowedUntil(LicenseBorrowStatus borrowStatus)
    {
        BorrowedUntil = borrowStatus == LicenseBorrowStatus.Borrowed
            ? _provider.BorrowExpirationDate()
            : null;
    }
}
