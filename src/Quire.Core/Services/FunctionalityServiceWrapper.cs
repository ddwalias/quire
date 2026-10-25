#nullable disable
using System;
using Quire.FunctionalitiesLimitation;
using Quire.Mirror;

namespace Quire.Services;

public partial class FunctionalityServiceWrapper
{
    private global::Quire.License.LicenseType _currentAppLicenseType;

    [MirrorVerified("PtcServicesFunctionalityServiceWrapperMechanicalDifferentialTests")]
    private readonly FunctionalityService _funcService;

    private readonly global::Quire.Wpf.WeakMultiCastDelegate<EventHandler> _functionalityServiceLicenseTypeChanged = new();

    [MirrorVerified("PtcServicesFunctionalityServiceWrapperMechanicalDifferentialTests")]
    public bool IsFunctionalityEnabled(EnumFunctionalityKey functionality) => _funcService.IsFunctionalityEnabled(functionality);

    public global::Quire.License.LicenseType CurrentAppLicenseType
    {
        get => _currentAppLicenseType;
        set
        {
            _currentAppLicenseType = value;
            FireFunctionalityServiceLicenseTypeChanged(EventArgs.Empty);
        }
    }

    private FunctionalityServiceWrapper()
    {
        _funcService = FunctionalityService.Instance;
        if (global::Quire.License.LicenseManager.Instance != null)
        {
            CurrentAppLicenseType = global::Quire.License.LicenseManager.Instance.LicenseType;
            global::Quire.License.LicenseManager.Instance.LicenseTypeChanged += OnLicenseTypeChanged;
        }
    }

    private void OnLicenseTypeChanged(object sender, EventArgs e)
    {
        CurrentAppLicenseType = global::Quire.License.LicenseManager.Instance.LicenseType;
    }

    public EnumFunctionalityBehaviorKey GetFunctionalityBehaviorKey(EnumFunctionalityKey functionality)
    {
        if (_funcService.IsFunctionalityEnabled(functionality))
        {
            if (_funcService.IsFunctionalityHasUnsupportedLicenseTypes(functionality)
                && CurrentAppLicenseType == global::Quire.License.LicenseType.Trial)
            {
                return EnumFunctionalityBehaviorKey.TrialBehavior;
            }

            return EnumFunctionalityBehaviorKey.FullBehavior;
        }

        return EnumFunctionalityBehaviorKey.FreemiumBehavior;
    }

    public event EventHandler FunctionalityServiceLicenseTypeChanged
    {
        add => _functionalityServiceLicenseTypeChanged.Combine(value);
        remove => _functionalityServiceLicenseTypeChanged.Remove(value);
    }

    private void FireFunctionalityServiceLicenseTypeChanged(EventArgs args)
    {
        if (_functionalityServiceLicenseTypeChanged != null)
        {
            _functionalityServiceLicenseTypeChanged.Invoke(new object[2] { this, args });
        }
    }
}
