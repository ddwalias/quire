#nullable disable
using Quire.Mirror;
using Quire.Mirror;

namespace Quire.PersistentDataObjects;

public partial class Plot3DType
{
    [MirrorVerified("PtcPersistentDataObjectsPlot3DTypeMechanicalDifferentialTests")]
    private trace3DType[] tracesField;

    [MirrorVerified("PtcPersistentDataObjectsPlot3DTypeMechanicalDifferentialTests")]
    public trace3DType[] traces { get => tracesField; set => tracesField = value; }

    [MirrorVerified("PtcPersistentDataObjectsPlot3DTypeMechanicalDifferentialTests")]
    private SizeType graphsizeField;

    [MirrorVerified("PtcPersistentDataObjectsPlot3DTypeMechanicalDifferentialTests")]
    public SizeType graphsize { get => graphsizeField; set => graphsizeField = value; }

    [MirrorVerified("PtcPersistentDataObjectsPlot3DTypeMechanicalDifferentialTests")]
    private axes3Type axesField;

    [MirrorVerified("PtcPersistentDataObjectsPlot3DTypeMechanicalDifferentialTests")]
    public axes3Type axes { get => axesField; set => axesField = value; }

    [MirrorVerified("PtcPersistentDataObjectsPlot3DTypeMechanicalDifferentialTests")]
    private plotEquation[] plotEquationsField;

    [MirrorVerified("PtcPersistentDataObjectsPlot3DTypeMechanicalDifferentialTests")]
    public plotEquation[] plotEquations { get => plotEquationsField; set => plotEquationsField = value; }

    [MirrorVerified("PtcPersistentDataObjectsPlot3DTypeMechanicalDifferentialTests")]
    private MatrixOfDoubles cameraTransformMatrixField;

    [MirrorVerified("PtcPersistentDataObjectsPlot3DTypeMechanicalDifferentialTests")]
    public MatrixOfDoubles CameraTransformMatrix { get => cameraTransformMatrixField; set => cameraTransformMatrixField = value; }

    [MirrorVerified("PtcPersistentDataObjectsPlot3DTypeMechanicalDifferentialTests")]
    private Plot3DTypeCameraType cameraTypeField;

    [MirrorVerified("PtcPersistentDataObjectsPlot3DTypeMechanicalDifferentialTests")]
    public Plot3DTypeCameraType CameraType { get => cameraTypeField; set => cameraTypeField = value; }

    [MirrorVerified("PtcPersistentDataObjectsPlot3DTypeMechanicalDifferentialTests")]
    private bool equalscaleField;

    [MirrorVerified("PtcPersistentDataObjectsPlot3DTypeMechanicalDifferentialTests")]
    public bool equalscale { get => equalscaleField; set => equalscaleField = value; }

    [MirrorVerified("Release IL")]
    public Plot3DType()
    {
        cameraTypeField = Plot3DTypeCameraType.Perspective;
        equalscaleField = false;
    }
}
