#nullable disable

namespace Quire.License;

public partial class LicenseOptionsPage
{
    static LicenseOptionsPage()
    {
        LicenseURITypeProperty = global::System.Windows.DependencyProperty.Register(
            "LicenseURIType",
            typeof(global::Quire.License.LicenseUriType),
            typeof(LicenseOptionsPage));
        LicenseServerAddressProperty = global::System.Windows.DependencyProperty.Register(
            "LicenseServerAddress",
            typeof(string),
            typeof(LicenseOptionsPage));
        LicenseServerPortProperty = global::System.Windows.DependencyProperty.Register(
            "LicenseServerPort",
            typeof(string),
            typeof(LicenseOptionsPage));
        LicenseFilePathProperty = global::System.Windows.DependencyProperty.Register(
            "LicenseFilePath",
            typeof(string),
            typeof(LicenseOptionsPage));
        LicenseSourceListProperty = global::System.Windows.DependencyProperty.Register(
            "LicenseSourceList",
            typeof(string),
            typeof(LicenseOptionsPage));
    }

    void global::System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target)
    {
        switch (connectionId)
        {
            case 1:
                _this = (LicenseOptionsPage)target;
                break;
            case 2:
                licenseInfoAndStatus = (global::System.Windows.Controls.Grid)target;
                break;
            case 3:
                leftColumnOfLicenseInfo = (global::System.Windows.Controls.Grid)target;
                break;
            case 4:
                licenseStatusLabel = (global::System.Windows.Controls.TextBlock)target;
                break;
            case 5:
                licenseStatus = (global::System.Windows.Controls.Grid)target;
                break;
            case 6:
                licenseStatusTextBox = (global::System.Windows.Controls.TextBox)target;
                break;
            case 7:
                infoButtonForInvalidLicense = (global::System.Windows.Controls.Button)target;
                infoButtonForInvalidLicense.Click += ShowLicenseErrorInfo;
                break;
            case 8:
                LicenseErrorInfoPopup = (global::System.Windows.Controls.Primitives.Popup)target;
                break;
            case 9:
                ((global::System.Windows.Documents.Hyperlink)target).RequestNavigate += HyperlinkNavigate;
                break;
            case 10:
                licenseTypeLabel = (global::System.Windows.Controls.TextBlock)target;
                break;
            case 11:
                licenseTypeTextBox = (global::System.Windows.Controls.TextBox)target;
                break;
            case 12:
                licenseIssueDateLabel = (global::System.Windows.Controls.TextBlock)target;
                break;
            case 13:
                licenseIssueDateTextBox = (global::System.Windows.Controls.TextBox)target;
                break;
            case 14:
                licenseExpirationDateLabel = (global::System.Windows.Controls.TextBlock)target;
                break;
            case 15:
                licenseExpirationDateTextBox = (global::System.Windows.Controls.TextBox)target;
                break;
            case 16:
                ((global::System.Windows.Controls.Button)target).Click += CopyAll_Clicked;
                break;
            case 17:
                rightColumnOfLicenseInfo = (global::System.Windows.Controls.Grid)target;
                break;
            case 18:
                serviceContractLabel = (global::System.Windows.Controls.TextBlock)target;
                break;
            case 19:
                serviceContractBox = (global::System.Windows.Controls.TextBox)target;
                break;
            case 20:
                customerNumberLabel = (global::System.Windows.Controls.TextBlock)target;
                break;
            case 21:
                customerNumberBox = (global::System.Windows.Controls.TextBox)target;
                break;
            case 22:
                saledOrderLabel = (global::System.Windows.Controls.TextBlock)target;
                break;
            case 23:
                salesOrderBox = (global::System.Windows.Controls.TextBox)target;
                break;
            case 24:
                leftOfbottomRowForLicenseInfo = (global::System.Windows.Controls.Grid)target;
                break;
            case 25:
                licenseHostIdLabel = (global::System.Windows.Controls.TextBlock)target;
                break;
            case 26:
                licenseHostIdTextBox = (global::System.Windows.Controls.TextBox)target;
                break;
            case 27:
                rightOfbottomRowForLicenseInfo = (global::System.Windows.Controls.Grid)target;
                break;
            case 28:
                productCodeLabel = (global::System.Windows.Controls.TextBlock)target;
                break;
            case 29:
                productCodeBox = (global::System.Windows.Controls.TextBox)target;
                break;
            case 30:
                licenseStatusMessageTextBox = (global::System.Windows.Controls.TextBox)target;
                break;
            case 31:
                licenseConfiguration = (global::System.Windows.Controls.Grid)target;
                break;
            case 32:
                configuration = (global::System.Windows.Controls.DockPanel)target;
                break;
            case 33:
                licenseBorrowedUntilDateLabel = (global::System.Windows.Controls.TextBox)target;
                break;
            case 34:
                serverRadioButton = (global::System.Windows.Controls.RadioButton)target;
                serverRadioButton.Checked += server_Checked;
                break;
            case 35:
                serverAddressTextBox = (global::System.Windows.Controls.TextBox)target;
                serverAddressTextBox.TextChanged += serverAddressTextBox_TextChanged;
                break;
            case 36:
                serverPortTextBox = (global::System.Windows.Controls.TextBox)target;
                serverPortTextBox.TextChanged += serverPortTextBox_TextChanged;
                break;
            case 37:
                ((global::System.Windows.Controls.Button)target).Click += Borrow_Click;
                break;
            case 38:
                ((global::System.Windows.Controls.Button)target).Click += Return_Clicked;
                break;
            case 39:
                sourceListLicensePathRadioButton = (global::System.Windows.Controls.RadioButton)target;
                sourceListLicensePathRadioButton.Checked += sourceListLicensePath_Checked;
                break;
            case 40:
                extendedLicensePathTextBox = (global::System.Windows.Controls.TextBox)target;
                extendedLicensePathTextBox.TextChanged += extendedLicensePathTextBox_TextChanged;
                break;
            case 41:
                filePathRadioButton = (global::System.Windows.Controls.RadioButton)target;
                filePathRadioButton.Checked += file_Checked;
                break;
            case 42:
                filePathTextBox = (global::System.Windows.Controls.TextBox)target;
                filePathTextBox.TextChanged += filePathTextBox_TextChanged;
                break;
            case 43:
                ((global::System.Windows.Controls.Button)target).Click += Browse_Clicked;
                break;
            case 44:
                _applyButton = (global::System.Windows.Controls.Button)target;
                _applyButton.Click += Apply_Clicked;
                break;
            case 45:
                licenseAcquisition = (global::System.Windows.Controls.Grid)target;
                break;
            case 46:
                getLicenseButton = (global::System.Windows.Controls.Button)target;
                getLicenseButton.Click += GetLicense_Clicked;
                break;
            default:
                _contentLoaded = true;
                break;
        }
    }
}
