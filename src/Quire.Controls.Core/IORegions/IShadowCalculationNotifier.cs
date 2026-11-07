#nullable disable
namespace Quire.Controls.Core.IORegions;

public partial interface IShadowCalculationNotifier
{
    void Add(Quire.Pair<IIORegion, Quire.Calculation.Core.ICalculatable> item);
    void Remove(Quire.Pair<IIORegion, Quire.Calculation.Core.ICalculatable> item);
}
