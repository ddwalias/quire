using System.Windows;
using System.Windows.Input;
using Quire;
using Quire.Controls.Core;

namespace Quire.Controls;

public partial class VisualFunctionDescriptor
{
    public string Params { get; set; }

    public override string Header => SymbolicRepresentation + "(" + Params + ")";

    public override RoutedCommand Command
    {
        get => EquationCommands.InsertFunction;
        set { }
    }

    public override object CommandParameter
    {
        get
        {
            int startIndex = 0;
            int argumentCount = 1;
            while ((startIndex = Params.IndexOf(',', startIndex)) >= 0)
            {
                startIndex++;
                argumentCount++;
            }

            return new InsertFunctionCommandParameter(SymbolicRepresentation, argumentCount, Label);
        }
    }

    public override string ScreenRepresentation
    {
        get => base.ScreenRepresentation.IsNullOrEmpty() ? SymbolicRepresentation : base.ScreenRepresentation;
        set => base.ScreenRepresentation = value;
    }

    public VisualFunctionDescriptor()
    {
        _label = IdLabels.Function;
    }
}
