#nullable disable
using Quire.Mirror;

namespace Quire.PersistentDataObjects;

public partial class presentationTextRendering
{
    [MirrorVerified("PtcPersistentDataObjectspresentationTextRenderingMechanicalDifferentialTests")]
    private string proofingLanguageField;

    [MirrorVerified("PtcPersistentDataObjectspresentationTextRenderingMechanicalDifferentialTests")]
    public presentationTextRendering()
    {
        proofingLanguageField = "en-US";
    }

    [MirrorVerified("PtcPersistentDataObjectspresentationTextRenderingMechanicalDifferentialTests")]
    private presentationTextRenderingTextStyle[] textStylesField;

    [MirrorVerified("PtcPersistentDataObjectspresentationTextRenderingMechanicalDifferentialTests")]
    public presentationTextRenderingTextStyle[] textStyles { get => textStylesField; set => textStylesField = value; }

    [MirrorVerified("PtcPersistentDataObjectspresentationTextRenderingMechanicalDifferentialTests")]
    public string ProofingLanguage { get => proofingLanguageField; set => proofingLanguageField = value; }
}
