#nullable disable
using System;
using System.Diagnostics;
using Quire.Mirror;

namespace Quire;

// This filename keeps the DebugTools closure ordinals aligned with Mathcad's compile order.
public static partial class DebugTools
{
    [MirrorVerified("Release IL")]
    private static AssertionResult DevAssert(bool condition, int framesToSkip, DebugTools.AssertionModifiers modifiers, string message = default, params object[] messageArgs)
    {
        AssertionOutput assertionOutput = ReplaceAssertionWithException ? (modifiers.HasFlag(AssertionModifiers.DoNotThrow) ? AssertionOutput.Log : AssertionOutput.Exception) : AssertionOutput.UI;
        if (condition)
        {
            return AssertionResult.PassedResult;
        }
        string text = string.IsNullOrEmpty(message) ? null : (messageArgs.IsNullOrEmpty() ? message : string.Format(message, messageArgs));
        StackTrace stackTrace = new StackTrace(framesToSkip, fNeedFileInfo: true);
        string logContents = (string.IsNullOrEmpty(text) ? string.Empty : (text + Environment.NewLine)) + string.Format("Assertion output: {0}", assertionOutput) + Environment.NewLine + stackTrace;
        if ((IsDebug || global::Quire.Properties.Settings.Default.CreateAssertionLogs) && NotCalledFromApplicationLogger(stackTrace))
        {
            new global::Quire.Services.ApplicationLogger().WriteToLog("Assertion", logContents);
        }
        switch (assertionOutput)
        {
            case AssertionOutput.UI:
                {
                    global::Quire.License.ILicenseManager instance = global::Quire.License.LicenseManager.Instance;
                    // Release IL branches on the != form of the final test; the empty body is as in the original.
                    if (instance != null && !instance.IsUsingReleaseFeature() && instance.LicenseSourceInfo.Type != global::Quire.License.LicenseUriType.FilePath)
                    {
                    }
                    break;
                }
            case AssertionOutput.Exception:
                {
                    StackFrame frame = stackTrace.GetFrame(1);
                    logContents = string.Format("Assertion in method: {0}.{1}, file: {2}, line: {3}", (ReadOnlySpan<object>)[frame.GetMethod().DeclaringType, frame.GetMethod().Name, frame.GetFileName(), frame.GetFileLineNumber()]) + Environment.NewLine + text;
                    throw new DevAssertionException(logContents);
                }
            default:
                Console.WriteLine("DebugTools.DevAssert: unsupported output type '{0}'", assertionOutput);
                break;
            case AssertionOutput.Log:
                break;
        }
        return AssertionResult.FailedResult;
    }
}
