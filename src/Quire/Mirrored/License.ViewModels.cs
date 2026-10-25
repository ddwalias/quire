#nullable disable

namespace Quire.License.ViewModel;

public partial class LicenseErrorViewModel
{
    public static string UpgradeLicenseSiteUrl
    {
        get
        {
            string safeUrl = global::Quire.VCard.VCardFactory.GetInstance().SafeUrl;
            if (global::System.String.IsNullOrEmpty(safeUrl))
            {
                return "https://www.ptc.com/go/mathcadexpressupgrade";
            }

            return safeUrl;
        }
    }

    public bool HasValidLicense => _licenseManager.HasValidLicense;

    public int? DaysLeft => _licenseManager.DaysLeft;

    public global::Quire.License.LicenseType LicenseType => _licenseManager.LicenseType;

    public global::System.Windows.Input.ICommand ActivateLicenseCommand =>
        _activateLicenseCommand ?? (_activateLicenseCommand = new global::Quire.Controls.Core.RelayCommand(ActivateLicense));

    public LicenseErrorViewModel()
    {
        _licenseManager = global::Quire.License.LicenseManager.Instance;
        if (_licenseManager != null)
        {
            _licenseManager.PropertyChanged += LicensePropertyChanged;
        }
    }

    private void LicensePropertyChanged(object sender, global::System.ComponentModel.PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case "HasValidLicense":
                OnPropertyChanged("HasValidLicense");
                break;
            case "DaysLeft":
                OnPropertyChanged("DaysLeft");
                break;
            case "LicenseType":
                OnPropertyChanged("LicenseType");
                break;
        }
    }

    private void ActivateLicense(object obj)
    {
        _licenseManager.LaunchLicenseWizard();
    }
}

internal partial class LicenseOptionsPageViewModel
{
    public bool IsLicenseDataModified
    {
        get => _isLicenseDataModified;
        set
        {
            if (_isLicenseDataModified != value)
            {
                _isLicenseDataModified = value;
                FirePropertyChanged("IsLicenseDataModified");
            }
        }
    }

    public LicenseOptionsPageViewModel(global::Quire.License.ILicenseManager licenseManager)
    {
        _licenseManager = licenseManager;
    }

    private void FirePropertyChanged(string propertyName)
    {
        OnPropertyChanged(propertyName);
    }

    public void LaunchLicenseWizard()
    {
        _licenseManager.LaunchLicenseWizard();
    }
}
