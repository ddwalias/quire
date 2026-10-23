#nullable disable
using System.Runtime.InteropServices;

namespace Quire.AutomationPrivate;

[global::Quire.Mirror.MirrorOf("MathcadAutomationPrivate.ISequentialStream, interop.mathcadautomationprivate")]
[ComImport]
[Guid("0C733A30-2A1C-11CE-ADE5-00AA0044773D")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface ISequentialStream
{
    void RemoteRead(out byte pv, [In] uint cb, out uint pcbRead);

    void RemoteWrite([In] ref byte pv, [In] uint cb, out uint pcbWritten);
}
