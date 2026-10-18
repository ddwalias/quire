#nullable disable
using System.Xml;
using Quire.Mirror;

namespace Quire.PersistentDataObjects;

public partial class valueType
{
    [MirrorVerified("PtcPersistentDataObjectsvalueTypeMechanicalDifferentialTests")]
    public valueType() { }

    [MirrorVerified("PtcPersistentDataObjectsvalueTypeMechanicalDifferentialTests")]
    private int resultRefField;

    [MirrorVerified("PtcPersistentDataObjectsvalueTypeMechanicalDifferentialTests")]
    public int resultRef { get => resultRefField; set => resultRefField = value; }

    [MirrorVerified("PtcPersistentDataObjectsvalueTypeMechanicalDifferentialTests")]
    private bool resultRefFieldSpecified;

    [MirrorVerified("PtcPersistentDataObjectsvalueTypeMechanicalDifferentialTests")]
    public bool resultRefSpecified { get => resultRefFieldSpecified; set => resultRefFieldSpecified = value; }

    [MirrorVerified("PtcPersistentDataObjectsvalueTypeMechanicalDifferentialTests")]
    public XmlNode[] InnerXml { get; set; }

    [MirrorVerified("PtcPersistentDataObjectsvalueTypeMechanicalDifferentialTests")]
    public string[] EquationSExpression { get; set; }
}
