#nullable disable
using System.Runtime.InteropServices;

namespace Quire.AutomationPrivate;

[global::Quire.Mirror.MirrorOf("MathcadAutomationPrivate.IStream, interop.mathcadautomationprivate")]
[ComImport]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid("0000000C-0000-0000-C000-000000000046")]
public interface IStream : ISequentialStream
{
    new void RemoteRead(out byte pv, [In] uint cb, out uint pcbRead);

    new void RemoteWrite([In] ref byte pv, [In] uint cb, out uint pcbWritten);

    void RemoteSeek([In] _LARGE_INTEGER dlibMove, [In] uint dwOrigin, out _ULARGE_INTEGER plibNewPosition);

    void SetSize([In] _ULARGE_INTEGER libNewSize);

    void RemoteCopyTo([In, MarshalAs(UnmanagedType.Interface)] IStream pstm, [In] _ULARGE_INTEGER cb,
        out _ULARGE_INTEGER pcbRead, out _ULARGE_INTEGER pcbWritten);

    void Commit([In] uint grfCommitFlags);

    void Revert();

    void LockRegion([In] _ULARGE_INTEGER libOffset, [In] _ULARGE_INTEGER cb, [In] uint dwLockType);

    void UnlockRegion([In] _ULARGE_INTEGER libOffset, [In] _ULARGE_INTEGER cb, [In] uint dwLockType);

    void Stat(out tagSTATSTG pstatstg, [In] uint grfStatFlag);

    void Clone([MarshalAs(UnmanagedType.Interface)] out IStream ppstm);
}
