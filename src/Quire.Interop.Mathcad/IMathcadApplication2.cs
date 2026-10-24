#nullable disable
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Quire;

[global::Quire.Mirror.MirrorOf("Mathcad.IMathcadApplication2, interop.mathcad")]
[ComImport]
[Guid("7BDE6954-0A1A-11E0-A7F6-11F8E2A8C36C")]
[TypeLibType(TypeLibTypeFlags.FDual | TypeLibTypeFlags.FDispatchable)]
[DefaultMember("Name")]
public interface IMathcadApplication2 : IMathcadApplication
{
	[DispId(0)]
	new string Name
	{
		[DispId(0)]
		[return: MarshalAs(UnmanagedType.BStr)]
		get;
	}

	[DispId(1)]
	new Application Application
	{
		[DispId(1)]
		[return: MarshalAs(UnmanagedType.Interface)]
		get;
	}

	[DispId(2)]
	new Application Parent
	{
		[DispId(2)]
		[return: MarshalAs(UnmanagedType.Interface)]
		get;
	}

	[DispId(3)]
	new Worksheet ActiveWorksheet
	{
		[DispId(3)]
		[return: MarshalAs(UnmanagedType.Interface)]
		get;
	}

	[DispId(4)]
	new Window ActiveWindow
	{
		[DispId(4)]
		[return: MarshalAs(UnmanagedType.Interface)]
		get;
	}

	[DispId(5)]
	new Worksheets Worksheets
	{
		[DispId(5)]
		[return: MarshalAs(UnmanagedType.Interface)]
		get;
	}

	[DispId(6)]
	new string FullName
	{
		[DispId(6)]
		[return: MarshalAs(UnmanagedType.BStr)]
		get;
	}

	[DispId(7)]
	new string Path
	{
		[DispId(7)]
		[return: MarshalAs(UnmanagedType.BStr)]
		get;
	}

	[DispId(8)]
	new string DefaultFilePath
	{
		[DispId(8)]
		[return: MarshalAs(UnmanagedType.BStr)]
		get;
		[DispId(8)]
		[param: In]
		[param: MarshalAs(UnmanagedType.BStr)]
		set;
	}

	[DispId(9)]
	new int Height
	{
		[DispId(9)]
		get;
		[DispId(9)]
		[param: In]
		set;
	}

	[DispId(10)]
	new int Width
	{
		[DispId(10)]
		get;
		[DispId(10)]
		[param: In]
		set;
	}

	[DispId(11)]
	new int Left
	{
		[DispId(11)]
		get;
		[DispId(11)]
		[param: In]
		set;
	}

	[DispId(12)]
	new int Top
	{
		[DispId(12)]
		get;
		[DispId(12)]
		[param: In]
		set;
	}

	[DispId(13)]
	new string Version
	{
		[DispId(13)]
		[return: MarshalAs(UnmanagedType.BStr)]
		get;
	}

	[DispId(14)]
	new bool Visible
	{
		[DispId(14)]
		get;
		[DispId(14)]
		[param: In]
		set;
	}

	[DispId(15)]
	new Windows Windows
	{
		[DispId(15)]
		[return: MarshalAs(UnmanagedType.Interface)]
		get;
	}

	[DispId(16)]
	new void Quit([In][ComAliasName("Mathcad.MCSaveOption")] MCSaveOption SaveOption = MCSaveOption.mcPromptToSaveChanges);

	[DispId(17)]
	new void CloseAll([In][ComAliasName("Mathcad.MCSaveOption")] MCSaveOption SaveOption = MCSaveOption.mcPromptToSaveChanges);

	[DispId(18)]
	void SetOption([In][ComAliasName("Mathcad.MCAppOption")] MCAppOption option, [In][MarshalAs(UnmanagedType.Struct)] object val);

	[DispId(19)]
	[return: MarshalAs(UnmanagedType.Struct)]
	object GetOption([In][ComAliasName("Mathcad.MCAppOption")] MCAppOption option);

	[DispId(20)]
	bool Active
	{
		[DispId(20)]
		get;
		[DispId(20)]
		[param: In]
		set;
	}

	[DispId(21)]
	int HWND
	{
		[DispId(21)]
		get;
	}
}
