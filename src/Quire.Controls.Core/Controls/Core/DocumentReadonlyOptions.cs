#nullable disable
namespace Quire.Controls.Core;

public partial class DocumentReadonlyOptions
{
    private static DocumentReadonlyOptions _default;

    public bool RequestToUpdateInputsEnabled { get; private set; }

    public bool FileLocationHistoryEnabled { get; private set; }

    public bool GenerateInputsOutputsStatesOnSave { get; private set; }

    public bool CaseInsensitiveAliasComparisonEnabled { get; private set; }

    public static DocumentReadonlyOptions Default => _default ?? (_default = new DocumentReadonlyOptions(generateInputsOutputsStatesOnSave: false, fileLocationHistoryEnabled: true, requestToUpdateInputsEnabled: false, caseInsensitiveAliasComparisonEnabled: false));

    public DocumentReadonlyOptions(bool generateInputsOutputsStatesOnSave, bool fileLocationHistoryEnabled, bool requestToUpdateInputsEnabled, bool caseInsensitiveAliasComparisonEnabled)
    {
        GenerateInputsOutputsStatesOnSave = generateInputsOutputsStatesOnSave;
        FileLocationHistoryEnabled = fileLocationHistoryEnabled;
        RequestToUpdateInputsEnabled = requestToUpdateInputsEnabled;
        CaseInsensitiveAliasComparisonEnabled = caseInsensitiveAliasComparisonEnabled;
    }
}
