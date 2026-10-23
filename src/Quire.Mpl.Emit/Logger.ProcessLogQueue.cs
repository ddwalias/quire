using System;
using System.IO;

namespace System.Reflection.Emit.Quire.Mpl;

public static partial class Logger
{
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
}
