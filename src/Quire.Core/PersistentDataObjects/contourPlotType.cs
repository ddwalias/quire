#nullable disable
using Quire.Mirror;

namespace Quire.PersistentDataObjects;

public partial class contourPlotType
{
    [MirrorVerified("PtcPersistentDataObjectscontourPlotTypeMechanicalDifferentialTests")]
    private bool equalscaleField;

    [MirrorVerified("PtcPersistentDataObjectscontourPlotTypeMechanicalDifferentialTests")]
    public contourPlotType()
    {
        equalscaleField = false;
    }

    [MirrorVerified("PtcPersistentDataObjectscontourPlotTypeMechanicalDifferentialTests")]
    private axes3Type axesField;

    [MirrorVerified("PtcPersistentDataObjectscontourPlotTypeMechanicalDifferentialTests")]
    public axes3Type axes { get => axesField; set => axesField = value; }

    [MirrorVerified("PtcPersistentDataObjectscontourPlotTypeMechanicalDifferentialTests")]
    private contourPlotTypeContour contourField;

    [MirrorVerified("PtcPersistentDataObjectscontourPlotTypeMechanicalDifferentialTests")]
    public contourPlotTypeContour contour { get => contourField; set => contourField = value; }

    [MirrorVerified("PtcPersistentDataObjectscontourPlotTypeMechanicalDifferentialTests")]
    private SizeType graphsizeField;

    [MirrorVerified("PtcPersistentDataObjectscontourPlotTypeMechanicalDifferentialTests")]
    public SizeType graphsize { get => graphsizeField; set => graphsizeField = value; }

    [MirrorVerified("PtcPersistentDataObjectscontourPlotTypeMechanicalDifferentialTests")]
    private plotEquation plotEquationField;

    [MirrorVerified("PtcPersistentDataObjectscontourPlotTypeMechanicalDifferentialTests")]
    public plotEquation plotEquation { get => plotEquationField; set => plotEquationField = value; }

    [MirrorVerified("PtcPersistentDataObjectscontourPlotTypeMechanicalDifferentialTests")]
    public bool equalscale { get => equalscaleField; set => equalscaleField = value; }
}
