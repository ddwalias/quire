#nullable disable

namespace Quire;

public partial class SpiritMainWindow
{
    private void OpenDocumentsListChanged(object sender, global::System.Collections.Specialized.NotifyCollectionChangedEventArgs eventArgs)
    {
        if (eventArgs.Action == global::System.Collections.Specialized.NotifyCollectionChangedAction.Move)
        {
            return;
        }
        if (eventArgs.OldItems != null)
        {
            foreach (global::Quire.Controls.Core.IEngineeringDocument oldItem in eventArgs.OldItems)
            {
                oldItem.PropertyChanged -= DocumentPropertyChanged;
            }
        }
        if (eventArgs.NewItems != null)
        {
            foreach (global::Quire.Controls.Core.IEngineeringDocument newItem in eventArgs.NewItems)
            {
                newItem.PropertyChanged += DocumentPropertyChanged;
            }
        }
        AnyDocumentRequestToUpdateInputsEnabledCheck();
        global::Quire.DebugTools.DevAssert(eventArgs.OldItems != null || eventArgs.NewItems != null, null);
    }

    private static void SpiritOptionPropertyChanged(object sender, global::System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName.Equals("EnableGettingStartedTab"))
        {
            global::Quire.Properties.Settings.Default.EnableGettingstartedTab = global::Quire.SpiritOptions.Instance.EnableGettingStartedTab;
        }
        global::Quire.Properties.Settings.Default.Save();
    }

    private void HookupLicense(bool isEmbedding)
    {
        global::Quire.License.LicenseManager.Instance.LicenseTypeChanged += OnLicenseStatusChanged;
        global::Quire.License.LicenseManager.Instance.PropertyChanged += OnLicensePropertyChanged;
        if (!isEmbedding)
        {
            global::Quire.License.LicenseManager.Instance.LaunchLicenseWizardAtStartupIfNeeded();
        }
        UpdateShowLicenseErrorProperty();
    }

    private void UnhookLicense()
    {
        global::Quire.License.LicenseManager.Instance.LicenseTypeChanged -= OnLicenseStatusChanged;
        global::Quire.License.LicenseManager.Instance.PropertyChanged -= OnLicensePropertyChanged;
    }

    private void OnLicensePropertyChanged(object sender, global::System.ComponentModel.PropertyChangedEventArgs args)
    {
        if (args.PropertyName == "DaysLeft")
        {
            UpdateShowLicenseErrorProperty();
        }
    }

    private void OnLicenseStatusChanged(object sender, global::System.EventArgs eventArgs)
    {
        UpdateShowLicenseErrorProperty();
    }

    private void UpdateShowLicenseErrorProperty()
    {
        global::Quire.License.LicenseType licenseType = global::Quire.License.LicenseManager.Instance.LicenseType;
        int daysLeft = global::Quire.License.LicenseManager.Instance.DaysLeft ?? -1;
        ((global::System.Windows.DependencyObject)this).SetValue(
            ShowLicenseErrorProperty,
            licenseType == global::Quire.License.LicenseType.NoLicense ||
            licenseType == global::Quire.License.LicenseType.Trial ||
            (daysLeft >= 0 && daysLeft < 30));
    }

    private static bool? IsReadonly(string fileName)
    {
        try
        {
            return (global::System.IO.File.GetAttributes(fileName) & global::System.IO.FileAttributes.ReadOnly) != 0;
        }
        catch
        {
            return null;
        }
    }

    private static void HandleOpenInputOutputDesignationWindow(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs args)
    {
        if (sender is SpiritMainWindow)
        {
            (sender as SpiritMainWindow).OpenWorksheetInputOutputWindow();
        }
    }
}
