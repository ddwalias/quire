#nullable disable
using Quire.Mirror;

namespace Quire.PersistentDataObjects;

public partial class presentationMathRenderingSymbolics
{
    [MirrorVerified("PtcPersistentDataObjectspresentationMathRenderingSymbolicsMechanicalDifferentialTests")]
    private presentationMathRenderingSymbolicsDerivationstepsstyle derivationstepsstyleField;

    [MirrorVerified("PtcPersistentDataObjectspresentationMathRenderingSymbolicsMechanicalDifferentialTests")]
    public presentationMathRenderingSymbolicsDerivationstepsstyle derivationstepsstyle { get => derivationstepsstyleField; set => derivationstepsstyleField = value; }

    [MirrorVerified("PtcPersistentDataObjectspresentationMathRenderingSymbolicsMechanicalDifferentialTests")]
    private bool showcommentsField;

    [MirrorVerified("PtcPersistentDataObjectspresentationMathRenderingSymbolicsMechanicalDifferentialTests")]
    public bool showcomments { get => showcommentsField; set => showcommentsField = value; }

    [MirrorVerified("PtcPersistentDataObjectspresentationMathRenderingSymbolicsMechanicalDifferentialTests")]
    private bool evaluateinplaceField;

    [MirrorVerified("PtcPersistentDataObjectspresentationMathRenderingSymbolicsMechanicalDifferentialTests")]
    public bool evaluateinplace { get => evaluateinplaceField; set => evaluateinplaceField = value; }
}
