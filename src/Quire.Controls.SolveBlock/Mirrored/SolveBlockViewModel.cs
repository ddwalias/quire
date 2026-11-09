using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Quire.Controls;

internal partial class SolveBlockViewModel
{
    private global::Quire.Controls.ISolveBlockModel _sbModel;

    [EditorBrowsable(EditorBrowsableState.Never)]
    internal SolveBlockViewModel(
        global::Quire.Controls.ISolveBlockModel mockModel,
        global::Quire.Controls.Core.IAlignmentGrid stubGrid)
        : base(mockModel, stubGrid)
    {
    }

    public SolveBlockViewModel(
        global::Quire.Controls.ISolveBlockModel model,
        Size? gridCellSize = null)
        : base(model, gridCellSize)
    {
        _sbModel = model;
        MinimumSize = new Size(5.2 * Grid.GridCellSize.Width, 5.2 * Grid.GridCellSize.Height);
        ContentSize = new Size(15.625 * Grid.GridCellSize.Width, 15.625 * Grid.GridCellSize.Height);
    }

    private SolveBlockViewModel(
        global::Quire.Controls.Mathboard.AggregateMathboardViewModel original)
        : base(original)
    {
        _sbModel = (global::Quire.Controls.ISolveBlockModel)_mathboardModel;
    }

    protected override void SetupCommandBindings()
    {
        base.SetupCommandBindings();
        var commandBinding = new CommandBinding(global::Quire.Controls.EquationCommands.InsertOperator);
        commandBinding.PreviewCanExecute += PreviewCanInsertOperator;
        CommandBindings.Add(commandBinding);
    }

    private void PreviewCanInsertOperator(object sender, CanExecuteRoutedEventArgs eArgs)
    {
        string operatorName = eArgs.Parameter as string;
        // Release IL branches on null to the method exit, so keep the work in the non-null branch.
        if (operatorName != null)
        {
            if (!global::Quire.EquationEditor.Tree.DefaultDescriptorTable.Default.ContainsKey(operatorName))
            {
                KeyValuePair<string, global::Quire.EquationEditor.Tree.MathNodeDescriptor> match =
                    global::Quire.EquationEditor.Tree.DefaultDescriptorTable.Default.FirstOrDefault(
                        item => item.Value.OperatorSExpName == operatorName);
                if (match.Key != null)
                    operatorName = match.Key;
            }

            if (operatorName == "GLOBAL_DEFINITION")
            {
                eArgs.ContinueRouting = false;
                eArgs.Handled = true;
                eArgs.CanExecute = false;
            }
        }
    }

    public override object Clone()
    {
        return CloneSolveBlockViewModel();
    }

    public SolveBlockViewModel CloneSolveBlockViewModel()
    {
        var clone = new SolveBlockViewModel(this);
        clone.WhiteboardItems.Each(clone.HandleItemAdded);
        return clone;
    }
}
