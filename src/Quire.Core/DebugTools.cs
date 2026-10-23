#nullable disable
using System.Diagnostics;
using System.Collections.Generic;
using System.Diagnostics;
using Quire.Mirror;
using Quire.Mirror;

namespace Quire;

public static partial class DebugTools
{
    [MirrorVerified("Release IL")]
    public const string DEBUG = "DEBUG";

    [MirrorVerified("Release IL")]
    private static readonly object _loggingLockObj;

    [MirrorVerified("Release IL")]
    private static List<string> _loggingData;

    [MirrorVerified("Release IL")]
    private static DateTime? _lastLogTime;

    [Conditional("DEBUG")]
    [MirrorVerified("Release IL")]
    public static void LogData(string line, ref DateTime? refTime)
    {
        if (_loggingLockObj == null)
        {
            return;
        }
        if (_loggingData == null)
        {
            return;
        }
        lock (_loggingLockObj)
        {
            if (_loggingData != null)
            {
                DateTime now = DateTime.Now;
                if (!refTime.HasValue)
                {
                    refTime = now;
                }
                else
                {
                    line += string.Format(" ({0} ms elapsed)", (now - refTime.Value).TotalMilliseconds.Round());
                }
                _loggingData.Add(now.ToString("yyyy'-'MM'-'dd' 'HH':'mm':'ss.fff") + "\t" + (now - (_lastLogTime ?? now)).ToString("mm':'ss'.'fff") + "\t" + line);
                _lastLogTime = now;
            }
        }
    }

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

    [MirrorVerified("Release IL")]
    private static void FailAssertion(int framesToSkip, DebugTools.AssertionModifiers modifiers, string message = default, params object[] messageArgs) => DevAssert(false, framesToSkip + 1, modifiers, message, messageArgs);

    [MirrorVerified("Release IL")]
    public static AssertionResult AssertNotNull(object objectToCheck, string message = default, params object[] messageArgs) => DevAssert(objectToCheck != null, 1, AssertionModifiers.None, message, messageArgs);

    private static bool NotCalledFromApplicationLogger(StackTrace stackTrace)
    {
        IEnumerable<StackFrame> frames = stackTrace.GetFrames();
        if (frames == null || frames.Count() <= 0)
        {
            return true;
        }
        Type applicationLoggerType = typeof(global::Quire.Services.IApplicationLogger);
        return !frames.Any((StackFrame frame) => applicationLoggerType.IsAssignableFrom(frame.GetMethod().DeclaringType));
    }

    [MirrorVerified("Release IL")]
    public static AssertionResult AssertNotNullDoNotThrow(object objectToCheck, string message = default, params object[] messageArgs) => DevAssert(objectToCheck != null, 1, AssertionModifiers.DoNotThrow, message, messageArgs);

    [MirrorVerified("Release IL")]
    public static TException AssertThrows<TException>(Action action) where TException : Exception
    {
        try
        {
            action();
        }
        catch (TException ex)
        {
            return ex;
        }
        FailAssertion("Expected exception was not thrown");
        return default;
    }

    [MirrorVerified("Release IL")]
    public static void FailAssertion(Exception ex) => FailAssertion(1, AssertionModifiers.None, "{0}", ex);

    [MirrorVerified("Release IL")]
    public static void FailAssertionDoNotThrow(Exception ex) => FailAssertion(1, AssertionModifiers.DoNotThrow, "{0}", ex);

    [MirrorVerified("Release IL")]
    public static AssertionResult LogAssert(bool value, string message = default)
    {
        if (!value)
        {
            LogError(message ?? "Assertion Failed");
        }
        return AssertionResult.Get(value);
    }

    [MirrorVerified("Release IL")]
    public static void StartLogging(ref List<string> prevData)
    {
        if (_loggingLockObj == null)
        {
            return;
        }
        lock (_loggingLockObj)
        {
            prevData = _loggingData;
            _loggingData = new List<string>();
        }
    }

    [Conditional("DEBUG")]
    [MirrorVerified("Release IL")]
    public static void StopLogging(ref List<string> prevData)
    {
        if (_loggingLockObj == null)
        {
            return;
        }
        lock (_loggingLockObj)
        {
            prevData = _loggingData;
            _loggingData = null;
        }
    }

    [MirrorVerified("Release IL")]
    public static DateTime? LogData(string line) => null;

    [MirrorVerified("Release IL")]
    public static void AttemptEnablingDebugVerifications(bool value)
    {
        DebugVerificationsAllowed = value && (IsDebug || ReplaceAssertionWithException);
    }

}
