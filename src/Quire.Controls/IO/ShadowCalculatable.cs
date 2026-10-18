#nullable disable
using System.Windows.Threading;
using Quire.EquationEditor.Tree.Transform;
using Quire.Mirror;

namespace Quire.Controls.IO;

internal partial class ShadowCalculatable
{
    [MirrorVerified("PtcControlsIOShadowCalculatableMechanicalDifferentialTests")]
    protected internal static string ConvertSExpressionToRl(string sExpr) => SExpressionToRlTransform.Default.Transform(sExpr);

    [MirrorVerified("PtcControlsIOShadowCalculatableMechanicalDifferentialTests")]
    protected ShadowCalculatable() { }

    [MirrorVerified("PtcControlsIOShadowCalculatableMechanicalDifferentialTests")]
    public static readonly DispatcherPriority ShadowResultsProcessingPriority_Send;

    [MirrorVerified("PtcControlsIOShadowCalculatableMechanicalDifferentialTests")]
    public Nullable<DispatcherPriority> ResultsProcessingPriority => new DispatcherPriority?(ShadowResultsProcessingPriority_Send);
}
