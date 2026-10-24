#nullable disable
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Quire;

[global::Quire.Mirror.MirrorOf("Mathcad.IMathcadApplication, interop.mathcad")]
[ComImport]
[TypeLibType(TypeLibTypeFlags.FDual | TypeLibTypeFlags.FDispatchable)]
[DefaultMember("Name")]
[Guid("7BDE4247-0A1A-11E0-A89F-11F8E2A8C36C")]
public interface IMathcadApplication
{
	[DispId(0)]
	string Name
	{
		[DispId(0)]
		[return: MarshalAs(UnmanagedType.BStr)]
		get;
	}

	[DispId(1)]
	Application Application
	{
		[DispId(1)]
		[return: MarshalAs(UnmanagedType.Interface)]
		get;
	}

	[DispId(2)]
	Application Parent
	{
		[DispId(2)]
		[return: MarshalAs(UnmanagedType.Interface)]
		get;
	}

	[DispId(3)]
	Worksheet ActiveWorksheet
	{
		[DispId(3)]
		[return: MarshalAs(UnmanagedType.Interface)]
		get;
	}

	[DispId(4)]
	Window ActiveWindow
	{
		[DispId(4)]
		[return: MarshalAs(UnmanagedType.Interface)]
		get;
	}

	[DispId(5)]
	Worksheets Worksheets
	{
		[DispId(5)]
		[return: MarshalAs(UnmanagedType.Interface)]
		get;
	}

	[DispId(6)]
	string FullName
	{
		[DispId(6)]
		[return: MarshalAs(UnmanagedType.BStr)]
		get;
	}

	[DispId(7)]
	string Path
	{
		[DispId(7)]
		[return: MarshalAs(UnmanagedType.BStr)]
		get;
	}

	[DispId(8)]
	string DefaultFilePath
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
	int Height
	{
		[DispId(9)]
		get;
		[DispId(9)]
		[param: In]
		set;
	}

	[DispId(10)]
	int Width
	{
		[DispId(10)]
		get;
		[DispId(10)]
		[param: In]
		set;
	}

	[DispId(11)]
	int Left
	{
		[DispId(11)]
		get;
		[DispId(11)]
		[param: In]
		set;
	}

	[DispId(12)]
	int Top
	{
		[DispId(12)]
		get;
		[DispId(12)]
		[param: In]
		set;
	}

	[DispId(13)]
	string Version
	{
		[DispId(13)]
		[return: MarshalAs(UnmanagedType.BStr)]
		get;
	}

	[DispId(14)]
	bool Visible
	{
		[DispId(14)]
		get;
		[DispId(14)]
		[param: In]
		set;
	}

	[DispId(15)]
	Windows Windows
	{
		[DispId(15)]
		[return: MarshalAs(UnmanagedType.Interface)]
		get;
	}

	[DispId(16)]
	void Quit([In][ComAliasName("Mathcad.MCSaveOption")] MCSaveOption SaveOption = MCSaveOption.mcPromptToSaveChanges);

	[DispId(17)]
	void CloseAll([In][ComAliasName("Mathcad.MCSaveOption")] MCSaveOption SaveOption = MCSaveOption.mcPromptToSaveChanges);
}
