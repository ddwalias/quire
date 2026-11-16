using Quire.Controls.Mathboard;

namespace Quire.Controls.Worksheet;

public partial class ContainerRegionPersistentData : RegionPersistentData
{
    public ContainerRegionPersistentData(MathboardControl mathBoardControl)
        : base(mathBoardControl)
    {
    }

    protected override void Deserialize()
    {
        DeserializeDimensions();
        base.Deserialize();
    }
}
