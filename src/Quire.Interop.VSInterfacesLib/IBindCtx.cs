#nullable disable
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Quire.VSINTERFACESLib;

[global::Quire.Mirror.MirrorOf("VSINTERFACESLib.IBindCtx, interop.vsinterfaceslib")]
[ComImport]
[Guid("0000000E-0000-0000-C000-000000000046")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface IBindCtx
{
	void RegisterObjectBound([In][MarshalAs(UnmanagedType.IUnknown)] object punk);

	void RevokeObjectBound([In][MarshalAs(UnmanagedType.IUnknown)] object punk);

	void ReleaseBoundObjects();

	void RemoteSetBindOptions([In] ref tagBIND_OPTS2 pbindopts);

	void RemoteGetBindOptions([In][Out] ref tagBIND_OPTS2 pbindopts);

	void GetRunningObjectTable([MarshalAs(UnmanagedType.Interface)] out IRunningObjectTable pprot);

	void RegisterObjectParam([In][MarshalAs(UnmanagedType.LPWStr)] string pszKey, [In][MarshalAs(UnmanagedType.IUnknown)] object punk);

	void GetObjectParam([In][MarshalAs(UnmanagedType.LPWStr)] string pszKey, [MarshalAs(UnmanagedType.IUnknown)] out object ppunk);

	void EnumObjectParam([MarshalAs(UnmanagedType.Interface)] out IEnumString ppenum);

	void RevokeObjectParam([In][MarshalAs(UnmanagedType.LPWStr)] string pszKey);
}
