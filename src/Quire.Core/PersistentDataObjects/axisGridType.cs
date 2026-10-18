#nullable disable
using Quire.Mirror;

namespace Quire.PersistentDataObjects;

public partial class axisGridType
{
    [MirrorVerified("PtcPersistentDataObjectsaxisGridTypeMechanicalDifferentialTests")]
    public axisGridType() { }

    [MirrorVerified("PtcPersistentDataObjectsaxisGridTypeMechanicalDifferentialTests")]
    private axisGridTypeGridFrequency gridFrequencyField;

    [MirrorVerified("PtcPersistentDataObjectsaxisGridTypeMechanicalDifferentialTests")]
    public axisGridTypeGridFrequency gridFrequency { get => gridFrequencyField; set => gridFrequencyField = value; }

    [MirrorVerified("PtcPersistentDataObjectsaxisGridTypeMechanicalDifferentialTests")]
    private axisGridTypeGridLabels gridLabelsField;

    [MirrorVerified("PtcPersistentDataObjectsaxisGridTypeMechanicalDifferentialTests")]
    public axisGridTypeGridLabels gridLabels { get => gridLabelsField; set => gridLabelsField = value; }

    [MirrorVerified("PtcPersistentDataObjectsaxisGridTypeMechanicalDifferentialTests")]
    private axisGridTypeGridLines gridLinesField;

    [MirrorVerified("PtcPersistentDataObjectsaxisGridTypeMechanicalDifferentialTests")]
    public axisGridTypeGridLines gridLines { get => gridLinesField; set => gridLinesField = value; }

    [MirrorVerified("PtcPersistentDataObjectsaxisGridTypeMechanicalDifferentialTests")]
    private axisGridTypeTickMarks tickMarksField;

    [MirrorVerified("PtcPersistentDataObjectsaxisGridTypeMechanicalDifferentialTests")]
    public axisGridTypeTickMarks tickMarks { get => tickMarksField; set => tickMarksField = value; }
}
