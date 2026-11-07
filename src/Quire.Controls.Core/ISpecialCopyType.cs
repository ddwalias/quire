#nullable disable
namespace Quire;

public partial interface ISpecialCopyType
{
    void Accept(ISpecialCopyVisitor visitor);
}
