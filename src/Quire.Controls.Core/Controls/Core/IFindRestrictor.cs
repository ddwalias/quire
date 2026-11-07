#nullable disable
namespace Quire.Controls.Core;

public partial interface IFindRestrictor
{
    bool CanSearch { get; }
    ISearchableRegion SearchableRegion { get; }
}
