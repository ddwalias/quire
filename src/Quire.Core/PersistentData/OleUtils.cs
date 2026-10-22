#nullable disable
using Quire.Mirror;

namespace Quire.PersistentData;

public static partial class OleUtils
{
    [MirrorVerified("Release IL")]
    public static bool CanCreateExcelContainer() => IsSupportedExcelGuid(GetExcelSheetGuid("Excel.Sheet"));

    [MirrorVerified("Release IL")]
    public static bool IsMicrosoftEquationGuid(Guid guid) => new System.Collections.Generic.List<Guid> { _equation1Guid, _equation2Guid, _equation3Guid }.Contains(guid);

    [MirrorVerified("Release IL")]
    public static bool IsSupportedExcelGuid(Guid guid) => new System.Collections.Generic.List<Guid> { _excel8Guid, _excel12Guid, _excelMacroEnabled12Guid, _excelBinaryMacroEnabled12Guid }.Contains(guid);

    [MirrorVerified("Release IL")]
    private static readonly Guid _excel8Guid;

    [MirrorVerified("Release IL")]
    public static bool IsExcel8Installed => GetExcelSheetGuid("Excel.Sheet") == _excel8Guid;
}
