namespace Quire.Controls.Include;

public partial class IncludeWorksheetErrorWarningData
{
    public static string ResetCode = "Reset";

    public const string CalculationSuccededErrorCode = "CalculationSucceededErrorCode";

    public const string CalculationFailedErrorCode = "IncludeControlCalculationErrors";

    public const string CalculationFreemiumErrorCode = "IncludeControlCalculationFreemiumErrors";

    public const string FileNotExist = "FileNotExist";

    public const string FileDoesNotExistErrorCode = "IncludeControlErrorsFileDoesNotExist";

    public const string RecursiveInclusionErrorCode = "IncludeControlErrorsRecursive";

    public const string IncorrectFileFormatErrorCode = "IncludeControlErrorsIncorrectFileFormat";

    public const string FileNotCompatibleErrorCode = "IncludeControlErrorsFileNotCompatible";

    public const string UnaccesibleFileErrorCode = "IncludeControlErrorsFileCannotBeAccessed";

    public const string CacheUsedWarningCode = "IncludeControlWarningsCache";

    public const string CorruptedCacheWarningCode = "IncludeControlWarningsCorruptedCachedPart";

    public const string ConflictWarningCode = "IncludeControlWarningsConflict";

    public const string ConflictSettingsWarningCode = "IncludeControlWarningsConflictSettings";

    public const string ConflictSettingWarningCode = "IncludeControlWarningsConflictSetting";

    public const string ORIGINConflictWarningCode = "IncludeControlWarningsConflictPropertyNameORIGIN";

    public const string TOLConflictWarningCode = "IncludeControlWarningsConflictPropertyNameTOL";

    public const string CTOLConflictWarningCode = "IncludeControlWarningsConflictPropertyNameCTOL";

    public const string CWDConflictWarningCode = "IncludeControlWarningsConflictPropertyNameCWD";

    public const string ApproximateEqualityConflictWarningCode = "IncludeControlWarningsConflictPropertyNameApproximateEquality";

    public const string OriginInStringsConflictWarningCode = "IncludeControlWarningsConflictPropertyNameOriginInStrings";

    public const string StrictSingularityCheckConflictWarningCode = "IncludeControlWarningsConflictPropertyNameStrictSingularityCheck";

    public const string UnitsAndConstantsInSymbolicsConflictWarningCode = "IncludeControlWarningsConflictPropertyNameUnitsAndConstantsInSymbolics";

    private static string[] conflictwarnings = new string[8]
    {
        "IncludeControlWarningsConflictPropertyNameORIGIN",
        "IncludeControlWarningsConflictPropertyNameTOL",
        "IncludeControlWarningsConflictPropertyNameCTOL",
        "IncludeControlWarningsConflictPropertyNameCWD",
        "IncludeControlWarningsConflictPropertyNameApproximateEquality",
        "IncludeControlWarningsConflictPropertyNameOriginInStrings",
        "IncludeControlWarningsConflictPropertyNameStrictSingularityCheck",
        "IncludeControlWarningsConflictPropertyNameUnitsAndConstantsInSymbolics"
    };

    public string PathIndexes { get; set; }

    public string FileName { get; set; }

    public string Code { get; set; }

    public string Text { get; set; }

    public bool IsPathRooted { get; set; }

    internal static bool IsConflictWarningCode(string code)
    {
        return ((System.ReadOnlySpan<string>)conflictwarnings).Contains(code);
    }

    internal static bool IsCacheWarningCode(string code)
    {
        if (!code.Equals("IncludeControlWarningsCache"))
        {
            return code.Equals("IncludeControlWarningsCorruptedCachedPart");
        }
        return true;
    }

    public IncludeWorksheetErrorWarningData()
    {
    }
}
