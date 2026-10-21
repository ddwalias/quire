#nullable disable
using System.Xml.Serialization;
using Quire.Mirror;
using Quire.Xml;

namespace Quire.PersistentDataObjects;

public partial class calculation
{
    [MirrorVerified("Release IL")]
    public static XmlSerializer XmlSerializer
    {
        get
        {
            if (_xmlSerializer == null)
            {
                using (new XmlSerializerCreationWrapper())
                {
                    _xmlSerializer = new XmlSerializer(_typeOfThis, new XmlRootAttribute("calculation")
                    {
                        Namespace = "http://schemas.ptc.com/mathcad/settings/calculation10"
                    });
                }
            }
            return _xmlSerializer;
        }
    }

    [MirrorVerified("PtcPersistentDataObjectscalculationMechanicalDifferentialTests")]
    private bool cacheworksheetField;

    [MirrorVerified("PtcPersistentDataObjectscalculationMechanicalDifferentialTests")]
    public calculation()
    {
        cacheworksheetField = false;
    }

    [MirrorVerified("PtcPersistentDataObjectscalculationMechanicalDifferentialTests")]
    private calculationBuiltInVariables builtInVariablesField;

    [MirrorVerified("PtcPersistentDataObjectscalculationMechanicalDifferentialTests")]
    public calculationBuiltInVariables builtInVariables { get => builtInVariablesField; set => builtInVariablesField = value; }

    [MirrorVerified("PtcPersistentDataObjectscalculationMechanicalDifferentialTests")]
    private calculationCalculationBehavior calculationBehaviorField;

    [MirrorVerified("PtcPersistentDataObjectscalculationMechanicalDifferentialTests")]
    public calculationCalculationBehavior calculationBehavior { get => calculationBehaviorField; set => calculationBehaviorField = value; }

    [MirrorVerified("PtcPersistentDataObjectscalculationMechanicalDifferentialTests")]
    private calculationUnits unitsField;

    [MirrorVerified("PtcPersistentDataObjectscalculationMechanicalDifferentialTests")]
    public calculationUnits units { get => unitsField; set => unitsField = value; }

    [MirrorVerified("PtcPersistentDataObjectscalculationMechanicalDifferentialTests")]
    public bool cacheworksheet { get => cacheworksheetField; set => cacheworksheetField = value; }
}
