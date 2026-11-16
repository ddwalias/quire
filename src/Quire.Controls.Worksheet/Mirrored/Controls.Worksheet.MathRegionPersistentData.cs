namespace Quire.Controls.Worksheet;

public partial class MathRegionPersistentData : RegionPersistentData
{
    private readonly EquationControl _equationControl;

    public MathRegionPersistentData(EquationControl equationControl)
        : base(equationControl)
    {
        _equationControl = equationControl;
    }

    public override void DeserializeEpilog()
    {
        _equationControl.Commit();
    }

    protected override void Deserialize()
    {
        base.Deserialize();
        if (!RegionData.ConvertedFromMC14)
        {
            DeserializeWidth();
        }
    }
}
