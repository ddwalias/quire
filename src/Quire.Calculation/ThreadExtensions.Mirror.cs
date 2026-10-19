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
            catch (InvalidOperationException invalidOperationException)
            {
                DebugTools.FailAssertion(invalidOperationException);
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
        catch (ArgumentNullException argumentNullException)
        {
            DebugTools.FailAssertion(argumentNullException);
        }
        catch (InvalidOperationException invalidOperationException)
        {
            DebugTools.FailAssertion(invalidOperationException);
        }
        catch (Exception exception)
        {
            DebugTools.FailAssertion(exception);
        }
    }
}
