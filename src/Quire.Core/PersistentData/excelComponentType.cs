#nullable disable
using Quire.Mirror;
using Quire.Mirror;
using Quire.PersistentDataObjects;

namespace Quire.PersistentData;

public partial class excelComponentType
{
    [MirrorVerified("PtcPersistentDataexcelComponentTypeMechanicalDifferentialTests")]
    public excelComponentType() { }

    [MirrorVerified("PtcPersistentDataexcelComponentTypeMechanicalDifferentialTests")]
    private excelComponentRegionType[] regionsField;

    [MirrorVerified("PtcPersistentDataexcelComponentTypeMechanicalDifferentialTests")]
    public excelComponentRegionType[] regions { get => regionsField; set => regionsField = value; }

    [MirrorVerified("PtcPersistentDataexcelComponentTypeMechanicalDifferentialTests")]
    private oleComponentSectionType inputSectionField;

    [MirrorVerified("PtcPersistentDataexcelComponentTypeMechanicalDifferentialTests")]
    public oleComponentSectionType InputSection { get => inputSectionField; set => inputSectionField = value; }

    [MirrorVerified("PtcPersistentDataexcelComponentTypeMechanicalDifferentialTests")]
    private oleComponentSectionType outputSectionField;

    [MirrorVerified("PtcPersistentDataexcelComponentTypeMechanicalDifferentialTests")]
    public oleComponentSectionType OutputSection { get => outputSectionField; set => outputSectionField = value; }

    [MirrorVerified("PtcPersistentDataexcelComponentTypeMechanicalDifferentialTests")]
    private int resultRefField;

    [MirrorVerified("PtcPersistentDataexcelComponentTypeMechanicalDifferentialTests")]
    public int resultRef { get => resultRefField; set => resultRefField = value; }

    [MirrorVerified("PtcPersistentDataexcelComponentTypeMechanicalDifferentialTests")]
    private bool resultRefFieldSpecified;

    [MirrorVerified("PtcPersistentDataexcelComponentTypeMechanicalDifferentialTests")]
    public bool resultRefSpecified { get => resultRefFieldSpecified; set => resultRefFieldSpecified = value; }

    [MirrorVerified("PtcPersistentDataexcelComponentTypeMechanicalDifferentialTests")]
    public double HeightAfterConversion { get; set; }

    [MirrorVerified("PtcPersistentDataexcelComponentTypeMechanicalDifferentialTests")]
    public IEnumerable<string> MC14ConversionLossAnnotations => MC14ConversionLossAnnotationsLocal;

    [MirrorVerified("PtcPersistentDataexcelComponentTypeMechanicalDifferentialTests")]
    public List<string> MC14ConversionLossAnnotationsLocal { get; set; }

    [MirrorVerified("Release IL")]
    public override IEnumerable<IRegionType> Regions { get => regions; set => regions = Enumerable.ToArray<excelComponentRegionType>(Enumerable.Cast<excelComponentRegionType>(value)); }
}
