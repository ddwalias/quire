using System;
using System.Collections.Concurrent;
using System.IO;
using System.Runtime.CompilerServices;
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
    private static readonly Thread _logThread;
    private static volatile bool _isRunning = true;

    public static bool IsEnabled
    {
        get => _isEnabled;
        set => _isEnabled = value;
    }

    public static string LogFileName
    {
        get => _logFileName;
        set
        {
            lock (_lock)
            {
                _logFileName = value;
            }
        }
    }

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

    private static void ProcessLogQueue()
    {
        while (_isRunning)
        {
            _logSignal.WaitOne();
            while (_logQueue.TryDequeue(out string result))
            {
                try
                {
                    File.AppendAllText(_logFileName, result);
                }
                catch (Exception)
                {
                }
            }
        }
    }

    public static void Shutdown()
    {
        if (_isRunning)
        {
            _isRunning = false;
            _logSignal.Set();
            _logThread?.Join();
        }
    }
}
