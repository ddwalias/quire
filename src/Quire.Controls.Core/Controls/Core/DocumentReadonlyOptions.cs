#nullable disable
using Quire.Mirror;

namespace Quire.Controls.Core;

public partial class DocumentReadonlyOptions
{
    [MirrorVerified("PtcControlsCoreDocumentReadonlyOptionsMechanicalDifferentialTests")]
    public bool RequestToUpdateInputsEnabled { get; private set; }

    [MirrorVerified("PtcControlsCoreDocumentReadonlyOptionsMechanicalDifferentialTests")]
    public bool FileLocationHistoryEnabled { get; private set; }

    [MirrorVerified("PtcControlsCoreDocumentReadonlyOptionsMechanicalDifferentialTests")]
    public bool GenerateInputsOutputsStatesOnSave { get; private set; }

    [MirrorVerified("PtcControlsCoreDocumentReadonlyOptionsMechanicalDifferentialTests")]
    public bool CaseInsensitiveAliasComparisonEnabled { get; private set; }
}
