#nullable disable
using System.Xml;
using Quire.Mirror;

namespace Quire.PersistentDataObjects;

public partial class polarPlotType
{
    [MirrorVerified("PtcPersistentDataObjectspolarPlotTypeMechanicalDifferentialTests")]
    public polarPlotType() { }

    [MirrorVerified("PtcPersistentDataObjectspolarPlotTypeMechanicalDifferentialTests")]
    private plotTitleType plotTitleField;

    [MirrorVerified("PtcPersistentDataObjectspolarPlotTypeMechanicalDifferentialTests")]
    public plotTitleType plotTitle { get => plotTitleField; set => plotTitleField = value; }

    [MirrorVerified("PtcPersistentDataObjectspolarPlotTypeMechanicalDifferentialTests")]
    private polarPlotTypeAxes axesField;

    [MirrorVerified("PtcPersistentDataObjectspolarPlotTypeMechanicalDifferentialTests")]
    public polarPlotTypeAxes axes { get => axesField; set => axesField = value; }

    [MirrorVerified("PtcPersistentDataObjectspolarPlotTypeMechanicalDifferentialTests")]
    public XmlNode[] InnerXml { get; set; }
}
