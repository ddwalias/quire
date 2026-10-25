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

    private void UpdateLicenseManager()
    {
        bool licenseUpdated = false;
        switch (LicenseURIType)
        {
            case global::Quire.License.LicenseUriType.ServerAndPort:
                if (!global::System.UInt32.TryParse(LicenseServerPort, out uint port))
                {
                    port = 0;
                    LicenseServerPort = "0";
                }

                licenseUpdated = global::Quire.License.LicenseManager.Instance
                    .SetEnterpriseLicenseServer(LicenseServerAddress, port);
                break;
            case global::Quire.License.LicenseUriType.FilePath:
                licenseUpdated = global::Quire.License.LicenseManager.Instance.SetLicenseFile(LicenseFilePath);
                break;
            case global::Quire.License.LicenseUriType.SourceList:
                licenseUpdated = global::Quire.License.LicenseManager.Instance.SetLicenseSourceList(LicenseSourceList);
                break;
        }

        if (licenseUpdated)
        {
            SetIsLicenseDataModified(!global::Quire.License.LicenseManager.Instance.HasValidLicense);
        }
    }

    private void Apply_Clicked(object sender, global::System.Windows.RoutedEventArgs args)
    {
        if (global::System.Windows.Input.Keyboard.FocusedElement != _applyButton)
        {
            _applyButton.Focus();
        }

        global::System.Windows.Input.Cursor cursor = Cursor;
        Cursor = global::System.Windows.Input.Cursors.Wait;
        try
        {
            UpdateLicenseManager();
        }
        finally
        {
            Cursor = cursor;
        }
    }

    private void Borrow_Click(object sender, global::System.Windows.RoutedEventArgs e)
    {
        global::System.Windows.Input.Cursor cursor = Cursor;
        Cursor = global::System.Windows.Input.Cursors.Wait;
        try
        {
            if (global::Quire.License.LicenseManager.Instance.LicenseBorrowStatus == global::Quire.License.LicenseBorrowStatus.CanBorrow)
            {
                global::Quire.License.LicenseManager.Instance.BorrowLicense(
                    global::System.DateTime.Now + global::System.TimeSpan.FromDays(7));
            }
        }
        finally
        {
            Cursor = cursor;
        }
    }

    private void Return_Clicked(object sender, global::System.Windows.RoutedEventArgs e)
    {
        global::System.Windows.Input.Cursor cursor = Cursor;
        Cursor = global::System.Windows.Input.Cursors.Wait;
        try
        {
            if (global::Quire.License.LicenseManager.Instance.LicenseBorrowStatus == global::Quire.License.LicenseBorrowStatus.Borrowed)
            {
                global::Quire.License.LicenseManager.Instance.ReturnLicense();
            }
        }
        finally
        {
            Cursor = cursor;
        }
    }

    private void Browse_Clicked(object sender, global::System.Windows.RoutedEventArgs e)
    {
        global::Microsoft.Win32.OpenFileDialog openFileDialog = new()
        {
            Title = (string)Resources["BrowseForLicense"],
            Filter = (string)Resources["LicenseFilesFilter"] + " (*.*)|*.*",
            InitialDirectory = LicenseFilePath
        };
        if (global::Quire.Services.AppServiceRegistry.GetService<global::Quire.Wpf.IDialogManager>()
            .ShowDialog(openFileDialog) == true)
        {
            LicenseFilePath = openFileDialog.FileName;
            LicenseURIType = global::Quire.License.LicenseUriType.FilePath;
            SetIsLicenseDataModified(true);
        }
    }

    private void HyperlinkNavigate(object sender, global::System.Windows.Navigation.RequestNavigateEventArgs e)
    {
        global::Quire.DebugTools.AssertNotNull(ParentOwner, null);
        try
        {
            global::Quire.AppCommands.DisplayHyperlink.Execute(e.Uri.OriginalString, ParentOwner);
        }
        catch (global::System.Exception exception)
        {
            global::Quire.DebugTools.FailAssertion(exception);
        }
    }

    public void CopyToClipboard()
    {
        global::System.Text.StringBuilder stringBuilder = new();
        bool appended = false;
        AppendLine(licenseStatusLabel, licenseStatusTextBox, stringBuilder, ref appended);
        AppendLine(licenseTypeLabel, licenseTypeTextBox, stringBuilder, ref appended);
        AppendLine(licenseIssueDateLabel, licenseIssueDateTextBox, stringBuilder, ref appended);
        AppendLine(licenseExpirationDateLabel, licenseExpirationDateTextBox, stringBuilder, ref appended);
        AppendLine(licenseHostIdLabel, licenseHostIdTextBox, stringBuilder, ref appended);
        if (appended)
        {
            stringBuilder.AppendLine();
        }

        appended = false;
        AppendLine(serviceContractLabel, serviceContractBox, stringBuilder, ref appended);
        AppendLine(customerNumberLabel, customerNumberBox, stringBuilder, ref appended);
        AppendLine(saledOrderLabel, salesOrderBox, stringBuilder, ref appended);
        AppendLine(productCodeLabel, productCodeBox, stringBuilder, ref appended);
        if (appended)
        {
            stringBuilder.AppendLine();
        }

        if (licenseBorrowedUntilDateLabel.Visibility == global::System.Windows.Visibility.Visible
            && licenseBorrowedUntilDateLabel.Text != null)
        {
            stringBuilder.AppendLine(licenseBorrowedUntilDateLabel.Text);
        }

        // Mathcad emits String.Format calls here; interpolation handlers change the Release IL.
        if (serverRadioButton.IsChecked == true)
        {
            stringBuilder.AppendLine(global::System.String.Format(
                "{0} {1}:{2}",
                serverRadioButton.Content,
                serverAddressTextBox.Text,
                serverPortTextBox.Text));
        }
        else if (filePathRadioButton.IsChecked == true)
        {
            stringBuilder.AppendLine(global::System.String.Format(
                "{0} {1}",
                filePathRadioButton.Content,
                filePathTextBox.Text));
        }

        stringBuilder.AppendLine();
        stringBuilder.AppendLine(global::System.String.Format("{0}", licenseStatusMessageTextBox.Text));
        global::Quire.WpfClipboard.Default.SetText(stringBuilder.ToString());
    }

    private static void AppendLine(
        global::System.Windows.Controls.TextBlock label,
        global::System.Windows.Controls.TextBox text,
        global::System.Text.StringBuilder sb,
        ref bool appended)
    {
        if (label.Visibility != global::System.Windows.Visibility.Visible)
        {
            return;
        }

        // Mathcad emits String.Format calls here; interpolation handlers change the Release IL.
        sb.AppendLine(global::System.String.Format("{0} {1}", label.Text, text.Text));
        appended = true;
    }
}
