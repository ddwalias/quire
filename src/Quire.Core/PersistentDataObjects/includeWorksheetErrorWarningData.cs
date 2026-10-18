#nullable disable
using Quire.Mirror;

namespace Quire.PersistentDataObjects;

public partial class includeWorksheetErrorWarningData
{
    [MirrorVerified("PtcPersistentDataObjectsincludeWorksheetErrorWarningDataMechanicalDifferentialTests")]
    public includeWorksheetErrorWarningData() { }

    [MirrorVerified("PtcPersistentDataObjectsincludeWorksheetErrorWarningDataMechanicalDifferentialTests")]
    private string codeField;

    [MirrorVerified("PtcPersistentDataObjectsincludeWorksheetErrorWarningDataMechanicalDifferentialTests")]
    public string code { get => codeField; set => codeField = value; }

    [MirrorVerified("PtcPersistentDataObjectsincludeWorksheetErrorWarningDataMechanicalDifferentialTests")]
    private string fileNameField;

    [MirrorVerified("PtcPersistentDataObjectsincludeWorksheetErrorWarningDataMechanicalDifferentialTests")]
    public string fileName { get => fileNameField; set => fileNameField = value; }

    [MirrorVerified("PtcPersistentDataObjectsincludeWorksheetErrorWarningDataMechanicalDifferentialTests")]
    private string indexesField;

    [MirrorVerified("PtcPersistentDataObjectsincludeWorksheetErrorWarningDataMechanicalDifferentialTests")]
    public string indexes { get => indexesField; set => indexesField = value; }
}
