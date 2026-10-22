#nullable disable
#pragma warning disable

namespace Quire.PersistentDataObjects;

partial interface ICalculationHideable
{
    bool IsCalculationDisabled { get; }
}
