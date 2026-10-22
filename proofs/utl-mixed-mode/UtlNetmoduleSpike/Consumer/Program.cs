using System.Collections;
using System.Runtime.InteropServices;
using System.Text;
using Quire.utl_mgd;
using Quire.Engine.Misc;

Console.WriteLine($"mixed module consumer: {Str.RemoveWhite(" a b ")}");
var pair = new Pair<int, string>(7, "seven");
if (pair.First != 7 || pair.Second != "seven")
    return 1;
Console.WriteLine($"netmodule Pair: {pair.First} {pair.Second}");
if (StringUtils.Capitalize("seven") != "Seven")
    return 2;
Console.WriteLine("netmodule StringUtils body and static data loaded");
var logName = Encoding.UTF8.GetBytes("Quire mixed-mode proof");
var logMessage = Encoding.UTF8.GetBytes("native logging boundary loaded");
unsafe
{
    fixed (byte* name = logName)
    fixed (byte* message = logMessage)
        NativeLog.Write((nint)name, (nint)(name + logName.Length),
            (nint)message, (nint)(message + logMessage.Length), 1);
}
Console.WriteLine("native UtlNativeLogUtf8 export called");
var labeled = new LabeledDictionary();
labeled["item"] = "value";
if (!((IDictionary)labeled).GetEnumerator().MoveNext()
    || !((IEnumerable)labeled).GetEnumerator().MoveNext())
    return 3;
Console.WriteLine("LabeledDictionary explicit interface implementations loaded");
using var mutex = new SafeMutex($"UtlNetmoduleSpike-{Guid.NewGuid():N}", 0);
mutex.WaitOne();
mutex.ReleaseMutex();
var monitor = new object();
SafeMonitor.Enter(monitor);
if (!SafeMonitor.TryEnter(monitor))
    return 4;
SafeMonitor.Exit(monitor);
SafeMonitor.Exit(monitor);
Console.WriteLine("SafeMonitor critical-section methods loaded");
Console.WriteLine("managed consumer loaded C# netmodule types with the C++/CLI Utl port");
return 0;

internal static partial class NativeLog
{
    [LibraryImport("Quire.Engine.Utl.dll", EntryPoint = "UtlNativeLogUtf8")]
    internal static partial void Write(
        nint nameBegin, nint nameEnd, nint messageBegin, nint messageEnd, int appendNewline);
}
