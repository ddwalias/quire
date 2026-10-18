#nullable disable
using Quire.Mirror;

namespace Quire.PersistentDataObjects;

public partial class presentationLanguage
{
    [MirrorVerified("PtcPersistentDataObjectspresentationLanguageMechanicalDifferentialTests")]
    private string mathField;

    [MirrorVerified("PtcPersistentDataObjectspresentationLanguageMechanicalDifferentialTests")]
    public string math { get => mathField; set => mathField = value; }

    [MirrorVerified("PtcPersistentDataObjectspresentationLanguageMechanicalDifferentialTests")]
    private string uiField;

    [MirrorVerified("PtcPersistentDataObjectspresentationLanguageMechanicalDifferentialTests")]
    public string UI { get => uiField; set => uiField = value; }
}
