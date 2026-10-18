#nullable disable
using Quire.Mirror;

namespace Quire.PersistentDataObjects;

public partial class resultFormatTypeTable
{
    [MirrorVerified("PtcPersistentDataObjectsresultFormatTypeTableMechanicalDifferentialTests")]
    public resultFormatTypeTable() { }

    [MirrorVerified("PtcPersistentDataObjectsresultFormatTypeTableMechanicalDifferentialTests")]
    private string itemidrefField;

    [MirrorVerified("PtcPersistentDataObjectsresultFormatTypeTableMechanicalDifferentialTests")]
    public string itemidref { get => itemidrefField; set => itemidrefField = value; }
}
