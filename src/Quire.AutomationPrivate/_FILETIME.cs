#nullable disable
using System.Runtime.InteropServices;

namespace Quire.AutomationPrivate;

[global::Quire.Mirror.MirrorOf("MathcadAutomationPrivate._FILETIME, interop.mathcadautomationprivate")]
[StructLayout(LayoutKind.Sequential, Pack = 4)]
public struct _FILETIME
{
    public uint dwLowDateTime;
    public uint dwHighDateTime;
}
