#nullable disable
using Quire.Mirror;
using System.Runtime.InteropServices;

namespace Quire.PersistentData;

public static partial class OleUtils
{
    [MirrorVerified("Release IL")]
    static OleUtils()
    {
        _excel8Guid = new Guid("{00020820-0000-0000-C000-000000000046}");
        _excel12Guid = new Guid("{00020830-0000-0000-C000-000000000046}");
        _excelMacroEnabled12Guid = new Guid("{00020832-0000-0000-C000-000000000046}");
        _excelBinaryMacroEnabled12Guid = new Guid("{00020833-0000-0000-C000-000000000046}");
        _chartAppOlePrime5Guid = new Guid("{7A9F0834-D6C9-4E7F-BD68-11C159C68C18}");
        _chartAppOlePrime6Guid = new Guid("{63B430F1-C980-40A0-AB26-6D8C52D906A9}");
        _chartAppOlePrime7Guid = new Guid("{D126FF25-0B04-4A76-BA13-DCAA94939DC1}");
        _chartAppOlePrime8Guid = new Guid("{A9D25763-E04C-4F3A-B447-2AD07A1DECA3}");
        _chartAppOlePrime9Guid = new Guid("{7EAB3A35-D5FF-4DDC-B5D5-E1D4366CF566}");
        _chartAppOlePrime10Guid = new Guid("{5FE0BD66-5E1C-4212-AAFC-C7CAE2097DE8}");
        _chartAppOlePrime11Guid = new Guid("{C8A78E8A-3EF8-42D6-B6F8-B0968379A841}");
        ChartAppOleGuidsConversionDict = new Dictionary<Guid, Guid>
        {
            { _chartAppOlePrime5Guid, _chartAppOlePrime11Guid },
            { _chartAppOlePrime6Guid, _chartAppOlePrime11Guid },
            { _chartAppOlePrime7Guid, _chartAppOlePrime11Guid },
            { _chartAppOlePrime8Guid, _chartAppOlePrime11Guid },
            { _chartAppOlePrime9Guid, _chartAppOlePrime11Guid },
            { _chartAppOlePrime10Guid, _chartAppOlePrime11Guid }
        };
        _equation1Guid = new Guid("{0003000B-0000-0000-C000-000000000046}");
        _equation2Guid = new Guid("{00021700-0000-0000-C000-000000000046}");
        _equation3Guid = new Guid("{0002ce02-0000-0000-c000-000000000046}");
    }

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
