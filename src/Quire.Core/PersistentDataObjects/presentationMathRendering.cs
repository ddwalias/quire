#nullable disable
using Quire.Mirror;

namespace Quire.PersistentDataObjects;

public partial class presentationMathRendering
{
    [MirrorVerified("PtcPersistentDataObjectspresentationMathRenderingMechanicalDifferentialTests")]
    public presentationMathRendering() { }

    [MirrorVerified("PtcPersistentDataObjectspresentationMathRenderingMechanicalDifferentialTests")]
    private presentationMathRenderingOperators operatorsField;

    [MirrorVerified("PtcPersistentDataObjectspresentationMathRenderingMechanicalDifferentialTests")]
    public presentationMathRenderingOperators operators { get => operatorsField; set => operatorsField = value; }

    [MirrorVerified("PtcPersistentDataObjectspresentationMathRenderingMechanicalDifferentialTests")]
    private presentationMathRenderingMathStyle[] mathStylesField;

    [MirrorVerified("PtcPersistentDataObjectspresentationMathRenderingMechanicalDifferentialTests")]
    public presentationMathRenderingMathStyle[] mathStyles { get => mathStylesField; set => mathStylesField = value; }

    [MirrorVerified("PtcPersistentDataObjectspresentationMathRenderingMechanicalDifferentialTests")]
    private presentationMathRenderingSymbolics symbolicsField;

    [MirrorVerified("PtcPersistentDataObjectspresentationMathRenderingMechanicalDifferentialTests")]
    public presentationMathRenderingSymbolics symbolics { get => symbolicsField; set => symbolicsField = value; }

    [MirrorVerified("PtcPersistentDataObjectspresentationMathRenderingMechanicalDifferentialTests")]
    private mathFormattingType formattingField;

    [MirrorVerified("PtcPersistentDataObjectspresentationMathRenderingMechanicalDifferentialTests")]
    public mathFormattingType formatting { get => formattingField; set => formattingField = value; }

    [MirrorVerified("PtcPersistentDataObjectspresentationMathRenderingMechanicalDifferentialTests")]
    private resultFormatType resultsField;

    [MirrorVerified("PtcPersistentDataObjectspresentationMathRenderingMechanicalDifferentialTests")]
    public resultFormatType results { get => resultsField; set => resultsField = value; }
}
