#nullable disable
namespace Quire.Controls.Core.AttachedBehavior;

public partial interface IAttachedBehavior<T>
{
    T OriginalValue { get; set; }
}
