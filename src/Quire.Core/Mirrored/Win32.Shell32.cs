#nullable disable

namespace Quire.Win32;

public static partial class Shell32
{
    [System.Runtime.InteropServices.DllImport("shell32.dll", ExactSpelling = true)]
    internal static extern void ILFree(nint pidlList);

    [System.Runtime.InteropServices.DllImport("shell32.dll", CharSet = System.Runtime.InteropServices.CharSet.Unicode, ExactSpelling = true)]
    internal static extern nint ILCreateFromPathW(string pszPath);

    [System.Runtime.InteropServices.DllImport("shell32.dll", ExactSpelling = true)]
    internal static extern int SHOpenFolderAndSelectItems(nint pidlList, uint cild, nint children, uint dwFlags);

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
