#nullable disable
using Quire.Mirror;
using Quire.Mirror;

namespace Quire.PersistentDataObjects;

public partial class axisBaseType
{
    [MirrorVerified("PtcPersistentDataObjectsaxisBaseTypeMechanicalDifferentialTests")]
    private bool enableField;

    [MirrorVerified("PtcPersistentDataObjectsaxisBaseTypeMechanicalDifferentialTests")]
    public axisBaseType()
    {
        enableField = true;
    }

    [MirrorVerified("PtcPersistentDataObjectsaxisBaseTypeMechanicalDifferentialTests")]
    private axisBaseTypeAxisLine axisLineField;

    [MirrorVerified("PtcPersistentDataObjectsaxisBaseTypeMechanicalDifferentialTests")]
    public axisBaseTypeAxisLine axisLine { get => axisLineField; set => axisLineField = value; }

    [MirrorVerified("PtcPersistentDataObjectsaxisBaseTypeMechanicalDifferentialTests")]
    private axisGridType axisGridField;

    [MirrorVerified("PtcPersistentDataObjectsaxisBaseTypeMechanicalDifferentialTests")]
    public axisGridType axisGrid { get => axisGridField; set => axisGridField = value; }

    [MirrorVerified("PtcPersistentDataObjectsaxisBaseTypeMechanicalDifferentialTests")]
    private plotTitleType axisLabelField;

    [MirrorVerified("PtcPersistentDataObjectsaxisBaseTypeMechanicalDifferentialTests")]
    public plotTitleType axisLabel { get => axisLabelField; set => axisLabelField = value; }

    [MirrorVerified("PtcPersistentDataObjectsaxisBaseTypeMechanicalDifferentialTests")]
    private axisBaseTypeMarker[] markersField;

    [MirrorVerified("PtcPersistentDataObjectsaxisBaseTypeMechanicalDifferentialTests")]
    public axisBaseTypeMarker[] markers { get => markersField; set => markersField = value; }

    [MirrorVerified("PtcPersistentDataObjectsaxisBaseTypeMechanicalDifferentialTests")]
    private resultFormatType numberFormatField;

    [MirrorVerified("PtcPersistentDataObjectsaxisBaseTypeMechanicalDifferentialTests")]
    public resultFormatType numberFormat { get => numberFormatField; set => numberFormatField = value; }

    [MirrorVerified("PtcPersistentDataObjectsaxisBaseTypeMechanicalDifferentialTests")]
    private axisBaseTypePlotEquations plotEquationsField;

    [MirrorVerified("PtcPersistentDataObjectsaxisBaseTypeMechanicalDifferentialTests")]
    public axisBaseTypePlotEquations plotEquations { get => plotEquationsField; set => plotEquationsField = value; }

    [MirrorVerified("PtcPersistentDataObjectsaxisBaseTypeMechanicalDifferentialTests")]
    private double startField;

    [MirrorVerified("PtcPersistentDataObjectsaxisBaseTypeMechanicalDifferentialTests")]
    public double start { get => startField; set => startField = value; }

    [MirrorVerified("PtcPersistentDataObjectsaxisBaseTypeMechanicalDifferentialTests")]
    private bool startFieldSpecified;

    [MirrorVerified("PtcPersistentDataObjectsaxisBaseTypeMechanicalDifferentialTests")]
    public bool startSpecified { get => startFieldSpecified; set => startFieldSpecified = value; }

    [MirrorVerified("PtcPersistentDataObjectsaxisBaseTypeMechanicalDifferentialTests")]
    private double endField;

    [MirrorVerified("PtcPersistentDataObjectsaxisBaseTypeMechanicalDifferentialTests")]
    public double end { get => endField; set => endField = value; }

    [MirrorVerified("PtcPersistentDataObjectsaxisBaseTypeMechanicalDifferentialTests")]
    private bool endFieldSpecified;

    [MirrorVerified("PtcPersistentDataObjectsaxisBaseTypeMechanicalDifferentialTests")]
    public bool endSpecified { get => endFieldSpecified; set => endFieldSpecified = value; }

    [MirrorVerified("PtcPersistentDataObjectsaxisBaseTypeMechanicalDifferentialTests")]
    private string rankField;

    [MirrorVerified("PtcPersistentDataObjectsaxisBaseTypeMechanicalDifferentialTests")]
    public string rank { get => rankField; set => rankField = value; }

    [MirrorVerified("PtcPersistentDataObjectsaxisBaseTypeMechanicalDifferentialTests")]
    public bool enable { get => enableField; set => enableField = value; }

    [MirrorVerified("Release IL")]
    public bool ShouldSerializemarkers() => markers != null;
}
