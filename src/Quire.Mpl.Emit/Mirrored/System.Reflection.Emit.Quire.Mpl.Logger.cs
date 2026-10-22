#nullable disable
#pragma warning disable

namespace System.Reflection.Emit.Quire.Mpl;

static partial class Logger
{
    private static volatile bool _isRunning;
    [global::System.Runtime.InteropServices.DllImport("kernel32.dll")]
    private static extern uint GetCurrentThreadId();
}
