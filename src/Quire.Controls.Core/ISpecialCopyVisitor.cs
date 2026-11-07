#nullable disable
using Quire.Controls.Core;

namespace Quire;

public partial interface ISpecialCopyVisitor
{
    void Visit(IEquationControl equationControl);
    void Visit(IRichTextWithEmbeddedItem textBox);
    void Visit(ICollapsibleAreaControl collapsibleArea);
}
