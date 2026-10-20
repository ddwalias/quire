using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;

namespace System.Reflection.Emit.Quire.Mpl;

public static partial class Logger
{
    public static void Log(string message, [CallerFilePath] string filePath = "", [CallerMemberName] string memberName = "", [CallerLineNumber] int lineNumber = 0)
    {
        if (!_isEnabled)
        {
            return;
        }
        string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
        string value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffffff");
        int managedThreadId = Thread.CurrentThread.ManagedThreadId;
        uint currentThreadId = GetCurrentThreadId();
        string item = $"{value} | Thread[{managedThreadId}] (OS:{currentThreadId}) | {fileNameWithoutExtension}.{memberName} | {message}{Environment.NewLine}";
        _logQueue.Enqueue(item);
        _logSignal.Set();
    }
}
