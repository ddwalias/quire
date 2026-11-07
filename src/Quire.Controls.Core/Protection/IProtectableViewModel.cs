#nullable disable
namespace Quire.Controls.Core.Protection;

public partial interface IProtectableViewModel
{
    Quire.ProtectionData ProtectionData { get; }
    bool ProtectionOperationsEnabled { get; }
}
