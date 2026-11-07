#nullable disable
namespace Quire.Controls.Core.IORegions;

public partial interface IScalarItemWithOriginalState
{
    IUnitedScalarState OriginalScalarState { get; }
}
