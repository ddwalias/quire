#nullable disable
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Quire.VSINTERFACESLib;

[global::Quire.Mirror.MirrorOf("VSINTERFACESLib.IMoniker, interop.vsinterfaceslib")]
[ComImport]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid("0000000F-0000-0000-C000-000000000046")]
public interface IMoniker : IPersistStream
{
	new void GetClassID(out Guid pClassID);

	new void IsDirty();

	new void Load([In][MarshalAs(UnmanagedType.Interface)] IStream pstm);

	new void Save([In][MarshalAs(UnmanagedType.Interface)] IStream pstm, [In] int fClearDirty);

	new void GetSizeMax(out _ULARGE_INTEGER pcbSize);

	void RemoteBindToObject([In][MarshalAs(UnmanagedType.Interface)] IBindCtx pbc, [In][MarshalAs(UnmanagedType.Interface)] IMoniker pmkToLeft, [In] ref Guid riidResult, [MarshalAs(UnmanagedType.IUnknown)] out object ppvResult);

	void RemoteBindToStorage([In][MarshalAs(UnmanagedType.Interface)] IBindCtx pbc, [In][MarshalAs(UnmanagedType.Interface)] IMoniker pmkToLeft, [In] ref Guid riid, [MarshalAs(UnmanagedType.IUnknown)] out object ppvObj);

	void Reduce([In][MarshalAs(UnmanagedType.Interface)] IBindCtx pbc, [In] uint dwReduceHowFar, [In][Out][MarshalAs(UnmanagedType.Interface)] ref IMoniker ppmkToLeft, [MarshalAs(UnmanagedType.Interface)] out IMoniker ppmkReduced);

	void ComposeWith([In][MarshalAs(UnmanagedType.Interface)] IMoniker pmkRight, [In] int fOnlyIfNotGeneric, [MarshalAs(UnmanagedType.Interface)] out IMoniker ppmkComposite);

	void Enum([In] int fForward, [MarshalAs(UnmanagedType.Interface)] out IEnumMoniker ppenumMoniker);

	void IsEqual([In][MarshalAs(UnmanagedType.Interface)] IMoniker pmkOtherMoniker);

	void Hash(out uint pdwHash);

	void IsRunning([In][MarshalAs(UnmanagedType.Interface)] IBindCtx pbc, [In][MarshalAs(UnmanagedType.Interface)] IMoniker pmkToLeft, [In][MarshalAs(UnmanagedType.Interface)] IMoniker pmkNewlyRunning);

	void GetTimeOfLastChange([In][MarshalAs(UnmanagedType.Interface)] IBindCtx pbc, [In][MarshalAs(UnmanagedType.Interface)] IMoniker pmkToLeft, out _FILETIME pfiletime);

	void Inverse([MarshalAs(UnmanagedType.Interface)] out IMoniker ppmk);

	void CommonPrefixWith([In][MarshalAs(UnmanagedType.Interface)] IMoniker pmkOther, [MarshalAs(UnmanagedType.Interface)] out IMoniker ppmkPrefix);

	void RelativePathTo([In][MarshalAs(UnmanagedType.Interface)] IMoniker pmkOther, [MarshalAs(UnmanagedType.Interface)] out IMoniker ppmkRelPath);

	void GetDisplayName([In][MarshalAs(UnmanagedType.Interface)] IBindCtx pbc, [In][MarshalAs(UnmanagedType.Interface)] IMoniker pmkToLeft, [MarshalAs(UnmanagedType.LPWStr)] out string ppszDisplayName);

	void ParseDisplayName([In][MarshalAs(UnmanagedType.Interface)] IBindCtx pbc, [In][MarshalAs(UnmanagedType.Interface)] IMoniker pmkToLeft, [In][MarshalAs(UnmanagedType.LPWStr)] string pszDisplayName, out uint pchEaten, [MarshalAs(UnmanagedType.Interface)] out IMoniker ppmkOut);

	void IsSystemMoniker(out uint pdwMksys);
}
