#nullable disable
using System.Runtime.InteropServices;

namespace Quire.AutomationPrivate;

[global::Quire.Mirror.MirrorOf("MathcadAutomationPrivate.IWorksheetPrivate, interop.mathcadautomationprivate")]
[ComImport]
[Guid("7BEEE418-0A1A-11E0-83E4-11F8E2A8C36C")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[TypeLibType(TypeLibTypeFlags.FOleAutomation)]
public interface IWorksheetPrivate
{
    void SetDocument(int pDoc);

    void Fire_Changed();

    void SetNodeComponentValue([In] int NodeHandle, [In, MarshalAs(UnmanagedType.Struct)] object vValue);

    void SetHTMLExportPreferences([In, MarshalAs(UnmanagedType.BStr)] string bstrPrefsXML);

    void SetImageCache([In, MarshalAs(UnmanagedType.Interface)] IMASImageCache pCache);

    [return: MarshalAs(UnmanagedType.BStr)]
    string GetHTMLStyles();

    void SetWebScrollPos([In] int x, [In] int y);

    void SetSynchronous([In] sbyte sync, out sbyte oldSync);

    [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_BSTR)]
    string[] GetInputs();

    [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_BSTR)]
    string[] GetOutputs();

    [return: MarshalAs(UnmanagedType.IUnknown)]
    object GetInputRegion([In, MarshalAs(UnmanagedType.BStr)] string varName);

    [return: MarshalAs(UnmanagedType.IUnknown)]
    object GetOutputRegion([In, MarshalAs(UnmanagedType.BStr)] string varName);

    [DispId(0x6001000c)]
    sbyte IsModified { get; [param: In] set; }
}
