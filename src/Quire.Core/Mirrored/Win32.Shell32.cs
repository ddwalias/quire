#nullable disable

namespace Quire.Win32;

public static partial class Shell32
{
    public static bool OpenContainingFolderAndSelectFile(string filePath)
    {
        nint pidl = ILCreateFromPathW(filePath);
        int result = 0;
        if (pidl != global::System.IntPtr.Zero)
        {
            result = SHOpenFolderAndSelectItems(pidl, 0u, global::System.IntPtr.Zero, 0u);
            ILFree(pidl);
        }
        return result >= 0;
    }
}
