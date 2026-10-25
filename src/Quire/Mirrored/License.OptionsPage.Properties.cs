#nullable disable

namespace Quire.License;

public partial class LicenseOptionsPage
{
    public global::System.Windows.Window ParentOwner { get; set; }

    public global::Quire.License.LicenseUriType LicenseURIType
    {
        get => (global::Quire.License.LicenseUriType)((global::System.Windows.DependencyObject)this).GetValue(LicenseURITypeProperty);
        set => ((global::System.Windows.DependencyObject)this).SetValue(LicenseURITypeProperty, value);
    }

    public string LicenseServerAddress
    {
        get => (string)((global::System.Windows.DependencyObject)this).GetValue(LicenseServerAddressProperty);
        set => ((global::System.Windows.DependencyObject)this).SetValue(LicenseServerAddressProperty, value);
    }

    public string LicenseServerPort
    {
        get => (string)((global::System.Windows.DependencyObject)this).GetValue(LicenseServerPortProperty);
        set => ((global::System.Windows.DependencyObject)this).SetValue(LicenseServerPortProperty, value);
    }

    public string LicenseFilePath
    {
        get => (string)((global::System.Windows.DependencyObject)this).GetValue(LicenseFilePathProperty);
        set => ((global::System.Windows.DependencyObject)this).SetValue(LicenseFilePathProperty, value);
    }

    public string LicenseSourceList
    {
        get => (string)((global::System.Windows.DependencyObject)this).GetValue(LicenseSourceListProperty);
        set => ((global::System.Windows.DependencyObject)this).SetValue(LicenseSourceListProperty, value);
    }

    public global::Quire.License.ViewModel.ILicenseOptionsPageViewModel ViewModel { get; private set; }

    public global::Quire.License.ILicenseManager LicenseManager => global::Quire.License.LicenseManager.Instance;

    public LicenseOptionsPage()
    {
        ConnectViewModel();
        InitializeComponent();
    }

    public override void EndInit()
    {
        InitConfiguration();
        base.EndInit();
    }
}
