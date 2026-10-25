#nullable disable

namespace Quire.License;

public sealed partial class LicenseManager
{
    private int _licenseRefreshInterval = 3600000;
    private string _licenseMessageText = string.Empty;
    private readonly global::System.Collections.Generic.List<string> _licenseMessages = new();
    private readonly global::Quire.Wpf.WeakMultiCastDelegate<global::System.ComponentModel.PropertyChangedEventHandler> _propertyChanged = new();
    private readonly global::Quire.Wpf.WeakMultiCastDelegate<global::System.EventHandler> _licenseTypeChanged = new();

    private LicenseManager(ILicenseProvider provider, global::System.Windows.ResourceDictionary licenseMessageDictionary)
    {
        _licenseMessageDictionary = licenseMessageDictionary;
        _provider = provider;
        _provider.DisplayLicenseMessage += new global::Quire.Delegate1<LicenseCbArgs>(OnDisplayLicenseMessage);
        _isUsingReleaseFeature = _provider.IsUsingReleaseFeature();
        _uiThreadDispatcher = global::System.Windows.Threading.Dispatcher.CurrentDispatcher;
        _licenseSource = new LicenseSource();
    }
}
