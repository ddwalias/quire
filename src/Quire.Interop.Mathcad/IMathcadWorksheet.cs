#nullable disable
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Quire;

[global::Quire.Mirror.MirrorOf("Mathcad.IMathcadWorksheet, interop.mathcad")]
[ComImport]
[Guid("7BDDF425-0A1A-11E0-AD8E-11F8E2A8C36C")]
[TypeLibType(TypeLibTypeFlags.FDual | TypeLibTypeFlags.FDispatchable)]
public interface IMathcadWorksheet
{
	[DispId(1)]
	Application Application
	{
		[DispId(1)]
		[return: MarshalAs(UnmanagedType.Interface)]
		get;
	}

	[DispId(2)]
	string FullName
	{
		[DispId(2)]
		[return: MarshalAs(UnmanagedType.BStr)]
		get;
	}

	[DispId(3)]
	string Name
	{
		[DispId(3)]
		[return: MarshalAs(UnmanagedType.BStr)]
		get;
	}

	[DispId(4)]
	bool NeedsSave
	{
		[DispId(4)]
		get;
	}

	[DispId(5)]
	Application Parent
	{
		[DispId(5)]
		[return: MarshalAs(UnmanagedType.Interface)]
		get;
	}

	[DispId(6)]
	string Path
	{
		[DispId(6)]
		[return: MarshalAs(UnmanagedType.BStr)]
		get;
	}

	[DispId(7)]
	Regions Regions
	{
		[DispId(7)]
		[return: MarshalAs(UnmanagedType.Interface)]
		get;
	}

	[DispId(8)]
	Windows Windows
	{
		[DispId(8)]
		[return: MarshalAs(UnmanagedType.Interface)]
		get;
	}

	[DispId(9)]
	void Close([In][ComAliasName("Mathcad.MCSaveOption")] MCSaveOption SaveOption = MCSaveOption.mcPromptToSaveChanges);

	[DispId(10)]
	[return: MarshalAs(UnmanagedType.Struct)]
	object GetOption([In][ComAliasName("Mathcad.MCWorksheetOption")] MCWorksheetOption option);

	[DispId(11)]
	[return: MarshalAs(UnmanagedType.IDispatch)]
	object GetValue([In][MarshalAs(UnmanagedType.BStr)] string bstrName);

	[DispId(12)]
	void PrintAll();

	[DispId(13)]
	void Recalculate();

	[DispId(14)]
	void Save();

	[DispId(15)]
	void SaveAs([In][MarshalAs(UnmanagedType.BStr)] string filespec, [In][ComAliasName("Mathcad.MCFileFormat")] MCFileFormat Format = MCFileFormat.mcMcadCurrentVersion);

	[DispId(16)]
	void SetOption([In][ComAliasName("Mathcad.MCWorksheetOption")] MCWorksheetOption option, [In][MarshalAs(UnmanagedType.Struct)] object vValue);

	[DispId(17)]
	void SetValue([In][MarshalAs(UnmanagedType.BStr)] string bstrName, [In][MarshalAs(UnmanagedType.Struct)] object vValue);
}
