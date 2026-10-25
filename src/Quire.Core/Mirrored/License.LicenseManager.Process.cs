#nullable disable

namespace Quire.License;

public sealed partial class LicenseManager
{
    public void LaunchLicenseWizard()
    {
        string args = global::System.String.Format(
            "/culture:{0} /hostId:{1} {2}",
            global::System.Globalization.CultureInfo.CurrentUICulture.Name,
            HostId,
            LicenseType == LicenseType.Trial ? " /t-" : " /l:trial");
        LaunchExternalLicenseProcess("LicenseWizard.exe", args);
    }

    public bool RefreshLicense()
    {
        bool refreshed = _provider.RefreshLicense();
        if (global::Quire.Services.ApplicationVariant.IsEngNbPlus == true && !refreshed)
        {
            IsEngNotebookLostConnection = true;
        }

        return refreshed;
    }

    public void ReleaseLicense(bool isClosingWindow)
    {
        ILicenseProvider provider = _provider;
        if (isClosingWindow)
        {
            _provider = null;
        }

        UpdateLicenseStatusAndRefreshTimer(false);
        provider.ReleaseLicense();
        _provider = provider;
    }

    private void DispatchedRefreshCallback(object state)
    {
        _uiThreadDispatcher.BeginInvoke(
            new global::Quire.DelegateFunction0<bool>(RefreshLicense),
            (global::System.Windows.Threading.DispatcherPriority)4,
            global::System.Array.Empty<object>());
    }
}
