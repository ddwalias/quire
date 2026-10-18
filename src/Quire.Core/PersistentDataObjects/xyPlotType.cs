#nullable disable
using System.Xml;
using Quire.Mirror;

namespace Quire.PersistentDataObjects;

public partial class xyPlotType
{
    [MirrorVerified("PtcPersistentDataObjectsxyPlotTypeMechanicalDifferentialTests")]
    private bool equalscaleField;

    [MirrorVerified("PtcPersistentDataObjectsxyPlotTypeMechanicalDifferentialTests")]
    public xyPlotType()
    {
        equalscaleField = false;
    }

    [MirrorVerified("PtcPersistentDataObjectsxyPlotTypeMechanicalDifferentialTests")]
    private plotTitleType plotTitleField;

    [MirrorVerified("PtcPersistentDataObjectsxyPlotTypeMechanicalDifferentialTests")]
    public plotTitleType plotTitle { get => plotTitleField; set => plotTitleField = value; }

    [MirrorVerified("PtcPersistentDataObjectsxyPlotTypeMechanicalDifferentialTests")]
    private xyPlotTypeAxes axesField;

    [MirrorVerified("PtcPersistentDataObjectsxyPlotTypeMechanicalDifferentialTests")]
    public xyPlotTypeAxes axes { get => axesField; set => axesField = value; }

    [MirrorVerified("PtcPersistentDataObjectsxyPlotTypeMechanicalDifferentialTests")]
    public bool equalscale { get => equalscaleField; set => equalscaleField = value; }

    [MirrorVerified("PtcPersistentDataObjectsxyPlotTypeMechanicalDifferentialTests")]
    public XmlNode[] InnerXml { get; set; }
}
