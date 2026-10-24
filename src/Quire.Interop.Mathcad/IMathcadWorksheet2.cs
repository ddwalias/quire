#nullable disable
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Quire;

[global::Quire.Mirror.MirrorOf("Mathcad.IMathcadWorksheet2, interop.mathcad")]
[ComImport]
[Guid("7BDDF426-0A1A-11E0-A466-11F8E2A8C36C")]
[TypeLibType(TypeLibTypeFlags.FDual | TypeLibTypeFlags.FDispatchable)]
public interface IMathcadWorksheet2 : IMathcadWorksheet
{
	[DispId(1)]
	new Application Application
	{
		[DispId(1)]
		[return: MarshalAs(UnmanagedType.Interface)]
		get;
	}

	[DispId(2)]
	new string FullName
	{
		[DispId(2)]
		[return: MarshalAs(UnmanagedType.BStr)]
		get;
	}

	[DispId(3)]
	new string Name
	{
		[DispId(3)]
		[return: MarshalAs(UnmanagedType.BStr)]
		get;
	}

	[DispId(4)]
	new bool NeedsSave
	{
		[DispId(4)]
		get;
	}

	[DispId(5)]
	new Application Parent
	{
		[DispId(5)]
		[return: MarshalAs(UnmanagedType.Interface)]
		get;
	}

	[DispId(6)]
	new string Path
	{
		[DispId(6)]
		[return: MarshalAs(UnmanagedType.BStr)]
		get;
	}

	[DispId(7)]
	new Regions Regions
	{
		[DispId(7)]
		[return: MarshalAs(UnmanagedType.Interface)]
		get;
	}

	[DispId(8)]
	new Windows Windows
	{
		[DispId(8)]
		[return: MarshalAs(UnmanagedType.Interface)]
		get;
	}

	[DispId(9)]
	new void Close([In][ComAliasName("Mathcad.MCSaveOption")] MCSaveOption SaveOption = MCSaveOption.mcPromptToSaveChanges);

	[DispId(10)]
	[return: MarshalAs(UnmanagedType.Struct)]
	new object GetOption([In][ComAliasName("Mathcad.MCWorksheetOption")] MCWorksheetOption option);

	[DispId(11)]
	[return: MarshalAs(UnmanagedType.IDispatch)]
	new object GetValue([In][MarshalAs(UnmanagedType.BStr)] string bstrName);

	[DispId(12)]
	new void PrintAll();

	[DispId(13)]
	new void Recalculate();

	[DispId(14)]
	new void Save();

	[DispId(15)]
	new void SaveAs([In][MarshalAs(UnmanagedType.BStr)] string filespec, [In][ComAliasName("Mathcad.MCFileFormat")] MCFileFormat Format = MCFileFormat.mcMcadCurrentVersion);

	[DispId(16)]
	new void SetOption([In][ComAliasName("Mathcad.MCWorksheetOption")] MCWorksheetOption option, [In][MarshalAs(UnmanagedType.Struct)] object vValue);

	[DispId(17)]
	new void SetValue([In][MarshalAs(UnmanagedType.BStr)] string bstrName, [In][MarshalAs(UnmanagedType.Struct)] object vValue);

	[DispId(18)]
	WorksheetMetadata Metadata
	{
		[DispId(18)]
		[return: MarshalAs(UnmanagedType.Interface)]
		get;
	}

	[DispId(19)]
	bool IsOpen
	{
		[DispId(19)]
		get;
	}

	[DispId(20)]
	IMathcadRegion2 SelectedRegion
	{
		[DispId(20)]
		[return: MarshalAs(UnmanagedType.Interface)]
		get;
	}
}
