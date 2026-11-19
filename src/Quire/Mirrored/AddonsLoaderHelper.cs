using System.Runtime.InteropServices;

namespace Quire;

internal static partial class AddonsLoaderHelper
{
    [DllImport("kernel32.dll")]
    public static extern nint LoadLibrary(string dllToLoadPath);
    [DllImport("kernel32.dll")]
    public static extern nint GetProcAddress(nint hModule, string procedureName);
    [DllImport("kernel32.dll")]
    public static extern bool FreeLibrary(nint hModule);
}
