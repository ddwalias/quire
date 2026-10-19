#nullable disable
using System.Globalization;
using System.Threading;
using Quire.Mirror;

namespace Quire.Calculation;

public static partial class ThreadExtensions
{
    [MirrorVerified("PtcCalculationThreadExtensions_MechanicalDifferentialTests")]
    public static void CaptureUiThreadCulture()
    {
        if (appCulture == null)
        {
            try
            {
                appCulture = Thread.CurrentThread.CurrentCulture;
                appCultureUI = Thread.CurrentThread.CurrentUICulture;
            }
            catch (InvalidOperationException ex)
            {
                DebugTools.FailAssertion(ex);
            }
        }
    }

    [MirrorVerified("PtcCalculationThreadExtensions_MechanicalDifferentialTests")]
    internal static void SyncWithUICulture(this Thread thread)
    {
        if (appCulture == null || appCultureUI == null)
            return;

        try
        {
            thread.CurrentCulture = appCulture;
            thread.CurrentUICulture = appCultureUI;
        }
        catch (ArgumentNullException ex)
        {
            DebugTools.FailAssertion(ex);
        }
        catch (InvalidOperationException ex2)
        {
            DebugTools.FailAssertion(ex2);
        }
        catch (Exception ex3)
        {
            DebugTools.FailAssertion(ex3);
        }
    }
}
