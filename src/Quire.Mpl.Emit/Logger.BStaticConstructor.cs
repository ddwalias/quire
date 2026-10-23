using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;

namespace System.Reflection.Emit.Quire.Mpl;

public static partial class Logger
{
    private static readonly object _lock = new object();
    private static readonly string? path1 = Environment.GetEnvironmentVariable("MS_LOGDIR");
    private static string _logFileName = Path.Combine(path1 ?? "", "Mplasb.log");
    private static bool _isEnabled = false;
    private static readonly ConcurrentQueue<string> _logQueue = new ConcurrentQueue<string>();
    private static readonly AutoResetEvent _logSignal = new AutoResetEvent(false);
    private static volatile bool _isRunning = true;

    [global::System.Runtime.InteropServices.DllImport("kernel32.dll")]
    private static extern uint GetCurrentThreadId();

    static Logger()
    {
        IsEnabled = false;
        if (IsEnabled)
        {
            _logThread = new Thread(ProcessLogQueue)
            {
                IsBackground = true
            };
            _logThread.Start();
            AppDomain.CurrentDomain.ProcessExit += (object? _, EventArgs __) => Shutdown();
        }
    }
}
