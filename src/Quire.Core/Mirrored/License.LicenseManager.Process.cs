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

    private void LaunchExternalLicenseProcess(string relativePath, string args)
    {
        string filePath = null;
        try
        {
            filePath = global::System.IO.Path.Combine(
                global::Quire.IO.FileUtils.GetAssemblyFolder(typeof(LicenseManager).Assembly),
                relativePath);
            global::System.Diagnostics.Process process = new();
            process.EnableRaisingEvents = true;
            process.StartInfo.FileName = filePath;
            process.StartInfo.Arguments = args;
            process.Start();
            process.WaitForExit();
            InitLicense();
        }
        catch (global::System.Exception exception)
        {
            global::Quire.DebugTools.FailAssertion("Error running \"{0}\" :\n{1}", filePath, exception.Message);
        }
    }
}
