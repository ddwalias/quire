using Quire;

namespace Quire.Controls;

public partial class VisualKeywordDescriptor
{
    public override object CommandParameter
    {
        get
        {
            if (!SExpression.IsNullOrEmpty())
            {
                return SExpression;
            }

            return SymbolicRepresentation;
        }
    }
}
