// File-name prefix 0 sets the compile order of Logger's partials so _logThread precedes Logger.AProperties.cs and preserves closure ordinal (<.cctor>b__15_0).
using System.Threading;

namespace System.Reflection.Emit.Quire.Mpl;

public static partial class Logger
{
    private static readonly Thread _logThread;
}
