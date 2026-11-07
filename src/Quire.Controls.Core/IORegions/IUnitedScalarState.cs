#nullable disable
namespace Quire.Controls.Core.IORegions;

public partial interface IUnitedScalarState
{
    double Value { get; }
    string Units { get; }
}
