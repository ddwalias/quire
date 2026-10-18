#nullable disable
using Quire.Mirror;

namespace Quire.PersistentDataObjects;

public partial class presentation
{
    [MirrorVerified("PtcPersistentDataObjectspresentationMechanicalDifferentialTests")]
    public presentation() { }

    [MirrorVerified("PtcPersistentDataObjectspresentationMechanicalDifferentialTests")]
    private presentationPageModel pageModelField;

    [MirrorVerified("PtcPersistentDataObjectspresentationMechanicalDifferentialTests")]
    public presentationPageModel pageModel { get => pageModelField; set => pageModelField = value; }

    [MirrorVerified("PtcPersistentDataObjectspresentationMechanicalDifferentialTests")]
    private presentationMathRendering mathRenderingField;

    [MirrorVerified("PtcPersistentDataObjectspresentationMechanicalDifferentialTests")]
    public presentationMathRendering mathRendering { get => mathRenderingField; set => mathRenderingField = value; }

    [MirrorVerified("PtcPersistentDataObjectspresentationMechanicalDifferentialTests")]
    private presentationTextRendering textRenderingField;

    [MirrorVerified("PtcPersistentDataObjectspresentationMechanicalDifferentialTests")]
    public presentationTextRendering textRendering { get => textRenderingField; set => textRenderingField = value; }

    [MirrorVerified("PtcPersistentDataObjectspresentationMechanicalDifferentialTests")]
    private presentationLanguage languageField;

    [MirrorVerified("PtcPersistentDataObjectspresentationMechanicalDifferentialTests")]
    public presentationLanguage language { get => languageField; set => languageField = value; }
}
