#nullable disable

namespace Quire.License;

public sealed partial class LicenseManager
{
    // Compile-order device: these 11 properties follow SetLicenseSourceList to preserve Mathcad lambda ordinal 112.

    public bool HasValidLicense
    {
        get => _hasValidLicense.GetValueOrDefault();
        private set
        {
            if (!(_hasValidLicense.HasValue && _hasValidLicense == value))
            {
                _hasValidLicense = value;
                FirePropertyChanged("HasValidLicense");
                FireLicenseStatusChanged();
            }

            if (HasValidLicense && DebugTools.AssertNotNull(_provider).Passed)
            {
                UpdateLicenseBorrowStatus();
                UpdateIssueDate();
                UpdateExpirationDate();
                UpdateIsEducationalLicense();
                UpdateProductCodeAndDerivedValues();
                UpdateCustomerNumber();
            }

            UpdateHostId();
            LicenseType = DetermineLicenseType(HasValidLicense);
        }
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


}
