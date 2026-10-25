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

    private void SetIsLicenseDataModified(bool value)
    {
        ViewModel.IsLicenseDataModified = value;
    }

    private void InitConfiguration()
    {
        SetIsLicenseDataModified(false);
        UpdateDataFromLicenseManager();
    }

    private void UpdateDataFromLicenseManager()
    {
        if (LicenseManager != null)
        {
            LicenseServerAddress = LicenseManager.LicenseSourceInfo.Host;
            LicenseServerPort = LicenseManager.LicenseSourceInfo.Port.ToString();
            LicenseFilePath = LicenseManager.LicenseSourceInfo.FilePath;
            LicenseURIType = LicenseManager.LicenseSourceInfo.Type;
            LicenseSourceList = LicenseManager.LicenseSourceInfo.SourceList;
        }
    }

    private void ConnectViewModel()
    {
        ViewModel = new global::Quire.License.ViewModel.LicenseOptionsPageViewModel(LicenseManager);
    }

    public void InitializeComponent()
    {
        if (_contentLoaded)
        {
            return;
        }

        _contentLoaded = true;
        global::System.Uri resourceLocator = new(
            "/Quire;component/license/licenseoptionspage.xaml",
            global::System.UriKind.Relative);
        global::System.Windows.Application.LoadComponent(this, resourceLocator);
    }

    private void server_Checked(object sender, global::System.Windows.RoutedEventArgs e) => SetIsLicenseDataModified(true);

    private void file_Checked(object sender, global::System.Windows.RoutedEventArgs e) => SetIsLicenseDataModified(true);

    private void sourceListLicensePath_Checked(object sender, global::System.Windows.RoutedEventArgs e) => SetIsLicenseDataModified(true);

    private void filePathTextBox_TextChanged(object sender, global::System.Windows.Controls.TextChangedEventArgs e) => SetIsLicenseDataModified(true);

    private void serverPortTextBox_TextChanged(object sender, global::System.Windows.Controls.TextChangedEventArgs e) => SetIsLicenseDataModified(true);

    private void serverAddressTextBox_TextChanged(object sender, global::System.Windows.Controls.TextChangedEventArgs e) => SetIsLicenseDataModified(true);

    private void extendedLicensePathTextBox_TextChanged(object sender, global::System.Windows.Controls.TextChangedEventArgs e) => SetIsLicenseDataModified(true);

    private void GetLicense_Clicked(object sender, global::System.Windows.RoutedEventArgs e)
    {
        ViewModel.LaunchLicenseWizard();
        InitConfiguration();
    }

    private void CopyAll_Clicked(object sender, global::System.Windows.RoutedEventArgs e)
    {
        CopyToClipboard();
    }

    private void ShowLicenseErrorInfo(object sender, global::System.Windows.RoutedEventArgs e)
    {
        LicenseErrorInfoPopup.IsOpen = true;
    }
}
