#nullable disable
using Quire.Mirror;
using System.Runtime.InteropServices;

namespace Quire.PersistentData;

public static partial class OleUtils
{
    [MirrorVerified("Release IL")]
    [DllImport("ole32.dll", CharSet = CharSet.Unicode, PreserveSig = false)]
    private static extern string ProgIDFromCLSID([In] ref Guid clsid);

    [MirrorVerified("Release IL")]
    public static bool CanCreateExcelContainer() => IsSupportedExcelGuid(GetExcelSheetGuid("Excel.Sheet"));

    [MirrorVerified("Release IL")]
    public static bool IsMicrosoftEquationGuid(Guid guid) => new System.Collections.Generic.List<Guid> { _equation1Guid, _equation2Guid, _equation3Guid }.Contains(guid);

    [MirrorVerified("Release IL")]
    public static bool IsSupportedExcelGuid(Guid guid) => new System.Collections.Generic.List<Guid> { _excel8Guid, _excel12Guid, _excelMacroEnabled12Guid, _excelBinaryMacroEnabled12Guid }.Contains(guid);

    [MirrorVerified("Release IL")]
    public static Guid GetExcelSheetGuid(string excelSheetProgId)
    {
        Type type = Type.GetTypeFromProgID(excelSheetProgId);
        if (null != type)
        {
            goto HasProgramId;
        }
        return Guid.Empty;
    HasProgramId:
        return type.GUID;
    }

    [MirrorVerified("Release IL")]
    public static bool IsOleAppSupportZoom(string progId)
    {
        if (progId != null && "OrgPlusWOPX,Mathcad,AcroExch.Document,PDFPlus.Document,MSGraph.Chart,PowerPoint.,Word.Document,Excel.Sheet".Contains(progId.Split('.', StringSplitOptions.None)[0]))
        {
            return true;
        }
        return false;
    }

    [MirrorVerified("Release IL")]
    public static bool? IsVersionInstalled(object comObject)
    {
        bool? result;
        IPersist persist = comObject as IPersist;
        if (persist != null)
        {
            Guid classId = persist.GetClassID();
            try
            {
                ProgIDFromCLSID(ref classId);
                result = new bool?(true);
            }
            catch (COMException)
            {
                result = new bool?(!(classId == _excel8Guid) ? false : CanCreateExcelContainer());
            }
        }
        else
        {
            result = null;
        }
        return result;
    }

    [MirrorVerified("Release IL")]
    private static readonly Guid _excel8Guid;

    [MirrorVerified("Release IL")]
    public static bool IsExcel8Installed => GetExcelSheetGuid("Excel.Sheet") == _excel8Guid;
}
