// File-name letters A and B set the compile order of Logger's partials; Roslyn numbers the static constructor's closure
// (<.cctor>b__15_0) from it. Renaming them fails the Release IL gate (closure ordinals shift).
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
