using System.Globalization;
using System.Reflection;
using System.Threading;

namespace Quire.Differential.Tests;

public class PtcCalculationThreadExtensions_MechanicalDifferentialTests
{
    private const string Contract = "Ptc.Calculation.ThreadExtensions";
    private static Type Q => typeof(global::Quire.Calculation.ThreadExtensions);
    private static Type M => Mathcad.Type("Ptc.Calculation", Contract);

    [SkippableFact]
    public void CaptureUiThreadCulture_06000061()
    {
        Mathcad.Require();
        CultureInfo previousCulture = Thread.CurrentThread.CurrentCulture;
        CultureInfo previousUiCulture = Thread.CurrentThread.CurrentUICulture;
        try
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("fr-FR");
            MethodInfo qMethod = Q.GetMethod("CaptureUiThreadCulture", BindingFlags.Public | BindingFlags.Static)!;
            MethodInfo mMethod = M.GetMethod("CaptureUiThreadCulture", BindingFlags.Public | BindingFlags.Static)!;
            MethodComparison.Compare("CaptureUiThreadCulture", Contract, qMethod, null, [], mMethod, null, [],
                state => CaptureState(Q), state => CaptureState(M));
        }
        finally
        {
            Thread.CurrentThread.CurrentCulture = previousCulture;
            Thread.CurrentThread.CurrentUICulture = previousUiCulture;
        }
    }

    [SkippableFact]
    public void SyncWithUICulture_06000062()
    {
        Mathcad.Require();
        MethodInfo qCapture = Q.GetMethod("CaptureUiThreadCulture", BindingFlags.Public | BindingFlags.Static)!;
        MethodInfo mCapture = M.GetMethod("CaptureUiThreadCulture", BindingFlags.Public | BindingFlags.Static)!;
        MethodComparison.Compare("CaptureUiThreadCulture setup", Contract, qCapture, null, [], mCapture, null, [],
            state => CaptureState(Q), state => CaptureState(M));

        CultureInfo previousCulture = Thread.CurrentThread.CurrentCulture;
        CultureInfo previousUiCulture = Thread.CurrentThread.CurrentUICulture;
        try
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("de-DE");
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("ja-JP");
            Thread currentThread = Thread.CurrentThread;
            MethodInfo qMethod = Q.GetMethod("SyncWithUICulture", BindingFlags.NonPublic | BindingFlags.Static)!;
            MethodInfo mMethod = M.GetMethod("SyncWithUICulture", BindingFlags.NonPublic | BindingFlags.Static)!;
            MethodComparison.Compare("SyncWithUICulture applies captured cultures", Contract,
                qMethod, null, [currentThread], mMethod, null, [currentThread],
                state => ThreadState(Thread.CurrentThread), state => ThreadState(Thread.CurrentThread));
        }
        finally
        {
            Thread.CurrentThread.CurrentCulture = previousCulture;
            Thread.CurrentThread.CurrentUICulture = previousUiCulture;
        }
    }

    private static string CaptureState(Type type) =>
        $"{ReadCulture(type, "appCulture").Name}|{ReadCulture(type, "appCultureUI").Name}";

    private static CultureInfo ReadCulture(Type type, string name) =>
        (CultureInfo)type.GetField(name, BindingFlags.NonPublic | BindingFlags.Static)!.GetValue(null)!;

    private static string ThreadState(Thread thread) => $"{thread.CurrentCulture.Name}|{thread.CurrentUICulture.Name}";
}
