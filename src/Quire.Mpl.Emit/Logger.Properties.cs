namespace System.Reflection.Emit.Quire.Mpl;

public partial class Logger
{
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
}
