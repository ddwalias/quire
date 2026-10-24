#nullable disable
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Quire.VSINTERFACESLib;

[global::Quire.Mirror.MirrorOf("VSINTERFACESLib.IVSDataType, interop.vsinterfaceslib")]
[ComImport]
[Guid("7BF01C94-0A1A-11E0-B2FC-11F8E2A8C36C")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[TypeLibType(TypeLibTypeFlags.FOleAutomation)]
public interface IVSDataType
{
	[DispId(1610678272)]
	bool IsComplex
	{
		get;
		[param: In]
		set;
	}

	[DispId(1610678274)]
	VSDATATYPE DataType
	{
		get;
	}

	[DispId(1610678275)]
	uint Dimensions
	{
		get;
	}

	[DispId(1610678276)]
	VSDATABOUNDS Bounds
	{
		get;
		[param: In]
		set;
	}

	void InitNew([In] uint Rows, [In] uint Cols, [In] bool vbImag);

	[return: MarshalAs(UnmanagedType.Interface)]
	IVSDataType Clone();

	int Lock([In] bool bWriteLock = true);

	int LockI([In] bool bWriteLock = true);

	void Unlock();

	void UnlockI();

	bool IsString([In] uint row, [In] uint col);

	bool IsMatrix([In] uint row, [In] uint col);

	bool IsObject([In] uint row, [In] uint col);

	[return: MarshalAs(UnmanagedType.BStr)]
	string GetStringValue([In] uint row, [In] uint col);

	void SetStringValue([In] uint row, [In] uint col, [In][MarshalAs(UnmanagedType.BStr)] string str);

	[return: MarshalAs(UnmanagedType.Interface)]
	IVSDataType GetElement([In] uint row, [In] uint col);

	void SetElement([In] uint row, [In] uint col, [In][MarshalAs(UnmanagedType.Interface)] IVSDataType pRetVal);

	[return: MarshalAs(UnmanagedType.Interface)]
	IVSDataType GetSubmatrix([In] uint srow, [In] uint scol, [In] VSDATABOUNDS Bounds);

	void SetSubmatrix([In] uint drow, [In] uint dcol, [In][MarshalAs(UnmanagedType.Interface)] IVSDataType pMatrix);
}
