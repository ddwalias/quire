#nullable disable
using System.Diagnostics;
using Quire.Mirror;

namespace Quire;

public static partial class DebugTools
{
    [MirrorVerified("PtcDebugToolsMechanicalDifferentialTests")]
    public static void FailAssertion(string message = default, params object[] messageArgs) => FailAssertion(1, AssertionModifiers.None, message, messageArgs);

    [MirrorVerified("PtcDebugToolsMechanicalDifferentialTests")]
    public static void FailAssertionDoNotThrow(string message = default, params object[] messageArgs) => FailAssertion(1, AssertionModifiers.DoNotThrow, message, messageArgs);

    [MirrorVerified("PtcDebugToolsMechanicalDifferentialTests")]
    public static AssertionResult DevAssert(bool condition, string message = default, params object[] messageArgs) => DevAssert(condition, 1, AssertionModifiers.None, message, messageArgs);

    [MirrorVerified("PtcDebugToolsMechanicalDifferentialTests")]
    private static readonly Action<bool, string> _assertDelegate;

    [MirrorVerified("PtcDebugToolsMechanicalDifferentialTests")]
    private static void ReportAssertionFailureOnAnotherThread(string formattedMessageWithStack) => _assertDelegate.Invoke(false, formattedMessageWithStack);

    [MirrorVerified("PtcDebugToolsMechanicalDifferentialTests")]
    public static void LogError(string message) => Trace.TraceWarning(message);

    [MirrorVerified("PtcDebugToolsMechanicalDifferentialTests")]
    public static void LogMessage(string message) => LogError(message);

    [MirrorVerified("PtcDebugToolsMechanicalDifferentialTests")]
    public static bool ReplaceAssertionWithException { get; set; }

    [MirrorVerified("PtcDebugToolsMechanicalDifferentialTests")]
    public static bool IsDebug => false;

    [MirrorVerified("PtcDebugToolsMechanicalDifferentialTests")]
    public static bool DebugVerificationsAllowed { get; private set; }
}
