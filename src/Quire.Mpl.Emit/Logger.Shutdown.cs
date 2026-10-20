namespace System.Reflection.Emit.Quire.Mpl;

public static partial class Logger
{
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
