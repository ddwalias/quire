#nullable disable
using Quire.Mirror;
using Quire.Properties;


namespace Quire.License;

public sealed partial class LicenseManager
{
    public static void CreateInstance(ILicenseProvider provider, global::System.Windows.ResourceDictionary licenseMessageDictionary)
    {
        _instance = new LicenseManager(provider, licenseMessageDictionary);
    }

    public static ILicenseManager Instance => _instance;

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

    public int? DaysLeft
    {
        get
        {
            if (!ExpirationDate.HasValue)
            {
                return null;
            }

            return (ExpirationDate.Value - global::System.DateTime.Now).Days;
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

    private void FireLicenseStatusChanged() => LicenseStatusChanged?.Invoke();

    public void FireEngNotebookConnectionStatusChanged() => EngNotebookConnectionStatusChanged?.Invoke();

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

    private static bool IsValidFileName(string fileName) => !string.IsNullOrEmpty(fileName);

    [MirrorVerified("Release IL")]
    public void SetNoLicense()
    {
        ClearLicenseProperties(false);
        UpdateLicenseStatus();
    }

    [MirrorVerified("Release IL")]
    private readonly LicenseSource _licenseSource;

    [MirrorVerified("Release IL")]
    public void InitLicense()
    {
        ClearLicenseProperties(false);
        UpdateApplicationVariant();
        _licenseSource.Load();
        UpdateLicenseStatus();
    }

    [MirrorVerified("Release IL")]
    public void ReapplyLicense()
    {
        ClearLicenseProperties(true);
        UpdateLicenseStatus();
    }

    [MirrorVerified("Release IL")]
    public void ResetLicenseSettings()
    {
        Settings.Default.Reset();
        InitLicense();
    }

    [MirrorVerified("Release IL")]
    private ILicenseProvider _provider;

    [MirrorVerified("Release IL")]
    public byte[] GetEncryptionKey(EncryptorVersion ver) => _provider.GetEncryptionKey(ver);

    [MirrorVerified("Release IL")]
    public byte[] GetEncryptionInitVector(EncryptorVersion ver) => _provider.GetEncryptionInitVector(ver);

    [MirrorVerified("Release IL")]
    public event Action LicenseStatusChanged;

    [MirrorVerified("Release IL")]
    public event Action EngNotebookConnectionStatusChanged;


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



}
