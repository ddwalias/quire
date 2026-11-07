#nullable disable
namespace Quire.Controls.Core.IORegions;

public partial interface IIOCalculatableContainerRegion
{
    Quire.Calculation.Core.ICalculatable GetCalculatableBySExpression(string sExpr);
}
