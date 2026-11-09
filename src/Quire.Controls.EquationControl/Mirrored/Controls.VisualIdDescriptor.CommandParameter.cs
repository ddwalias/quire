namespace Quire.Controls;

public partial class VisualIdDescriptor
{
    public override object CommandParameter
    {
        get
        {
            if (IdName != null)
            {
                return new InsertIdCommandParameter(IdName, Label);
            }

            return new InsertIdCommandParameter(SymbolicRepresentation, Label);
        }
    }
}
