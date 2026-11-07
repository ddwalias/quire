#nullable disable
namespace Quire.Controls.Core.Serialization;

public partial interface IFreezable
{
    bool IsFrozen { get; }
    void Freeze();
}
