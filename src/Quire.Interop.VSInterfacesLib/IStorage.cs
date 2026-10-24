#nullable disable
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Quire.VSINTERFACESLib;

[global::Quire.Mirror.MirrorOf("VSINTERFACESLib.IStorage, interop.vsinterfaceslib")]
[ComImport]
[Guid("0000000B-0000-0000-C000-000000000046")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface IStorage
{
	void CreateStream([In][MarshalAs(UnmanagedType.LPWStr)] string pwcsName, [In] uint grfMode, [In] uint reserved1, [In] uint reserved2, [MarshalAs(UnmanagedType.Interface)] out IStream ppstm);

	void RemoteOpenStream([In][MarshalAs(UnmanagedType.LPWStr)] string pwcsName, [In] uint cbReserved1, [In] ref byte reserved1, [In] uint grfMode, [In] uint reserved2, [MarshalAs(UnmanagedType.Interface)] out IStream ppstm);

	void CreateStorage([In][MarshalAs(UnmanagedType.LPWStr)] string pwcsName, [In] uint grfMode, [In] uint reserved1, [In] uint reserved2, [MarshalAs(UnmanagedType.Interface)] out IStorage ppstg);

	void OpenStorage([In][MarshalAs(UnmanagedType.LPWStr)] string pwcsName, [In][MarshalAs(UnmanagedType.Interface)] IStorage pstgPriority, [In] uint grfMode, [In][ComAliasName("VSINTERFACESLib.wireSNB")] ref tagRemSNB snbExclude, [In] uint reserved, [MarshalAs(UnmanagedType.Interface)] out IStorage ppstg);

	void RemoteCopyTo([In] uint ciidExclude, [In] ref Guid rgiidExclude, [In][ComAliasName("VSINTERFACESLib.wireSNB")] ref tagRemSNB snbExclude, [In][MarshalAs(UnmanagedType.Interface)] IStorage pstgDest);

	void MoveElementTo([In][MarshalAs(UnmanagedType.LPWStr)] string pwcsName, [In][MarshalAs(UnmanagedType.Interface)] IStorage pstgDest, [In][MarshalAs(UnmanagedType.LPWStr)] string pwcsNewName, [In] uint grfFlags);

	void Commit([In] uint grfCommitFlags);

	void Revert();

	void RemoteEnumElements([In] uint reserved1, [In] uint cbReserved2, [In] ref byte reserved2, [In] uint reserved3, [MarshalAs(UnmanagedType.Interface)] out IEnumSTATSTG ppenum);

	void DestroyElement([In][MarshalAs(UnmanagedType.LPWStr)] string pwcsName);

	void RenameElement([In][MarshalAs(UnmanagedType.LPWStr)] string pwcsOldName, [In][MarshalAs(UnmanagedType.LPWStr)] string pwcsNewName);

	void SetElementTimes([In][MarshalAs(UnmanagedType.LPWStr)] string pwcsName, [In] ref _FILETIME pctime, [In] ref _FILETIME patime, [In] ref _FILETIME pmtime);

	void SetClass([In] ref Guid clsid);

	void SetStateBits([In] uint grfStateBits, [In] uint grfMask);

	void Stat(out tagSTATSTG pstatstg, [In] uint grfStatFlag);
}
