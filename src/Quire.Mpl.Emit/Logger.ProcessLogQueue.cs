using System;
using System.IO;

namespace System.Reflection.Emit.Quire.Mpl;

public static partial class Logger
{
    private static void ProcessLogQueue()
    {
        goto OuterCondition;
    OuterLoop:
        _logSignal.WaitOne();
        string result;
    InnerLoop:
        if (_logQueue.TryDequeue(out result))
        {
            try
            {
                File.AppendAllText(_logFileName, result);
                goto InnerLoop;
            }
            catch (Exception)
            {
                goto InnerLoop;
            }
        }
        else
        {
            goto OuterCondition;
        }
    OuterCondition:
        if (_isRunning)
        {
            goto OuterLoop;
        }
    }
}
