#nullable disable
using Quire.Services;

namespace Quire.Controls.Core;

public static partial class HasUIElementArrangeInfoProviderExtension
{
    private static IFrameworkElementFacade _facade;

    public static IUIElementArrangeInfoProvider ArrangeInfoProvider
    {
        get
        {
            // Release IL null-checks the cached facade with a conditional access, then returns it after the lookup branch.
            if (!(_facade?.IsConnected == true))
            {
                return _facade = AppServiceRegistry.GetService<IFrameworkElementFacade>();
            }

            return _facade;
        }
    }
}
