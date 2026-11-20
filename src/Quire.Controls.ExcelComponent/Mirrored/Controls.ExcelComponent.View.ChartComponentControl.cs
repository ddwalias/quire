#nullable disable
#pragma warning disable

using System;
using System.Windows.Input;
using System.Windows.Threading;
using Quire.Controls.Core;
using Quire.Controls.Core.Serialization;
using Quire.Controls.ExcelComponent.Model;
using Quire.Controls.ExcelComponent.ViewModel;

namespace Quire.Controls.ExcelComponent.View;

public partial class ChartComponentControl
{
    private readonly ChartComponentModel _chartModel;
    private readonly ChartComponentViewModel _chartViewModel;

    protected override ISerializationStrategy SerializationStrategy { get; set; }

    protected override IDeserializationStrategy DeserializationStrategy { get; set; }

    private DispatcherOperation UpdateIdentifierLabelsOperation { get; set; }

    private ChartContainer ChartContainer => OleContainerItem as ChartContainer;

    IChartContainer IChartComponentControl.ChartContainer => ChartContainer;

    protected override IOleContainerItem CreateOleContainerItem()
    {
        return new ChartContainer();
    }

    private void BindCommands()
    {
        CommandBindings.Add(new CommandBinding(ChartComponentCommands.InsertAxisExpression, OnInsertAxisExpression, CanInsertAxisExpression));
    }

    protected override string GetInternalHelpContext()
    {
        return "ChartComponentControl";
    }

    public override string GetHelpContext()
    {
        if (ActiveItem == null)
        {
            return "ChartComponentControl";
        }
        string result;
        string text = null;
        if (ActiveItem is IEquationControl equationControl)
        {
            text = equationControl.GetCurrentNodeIdentifier();
        }
        if (ChartComponentCalculatableWrapper.IdentifierToChartType(text, null).IsValidType())
        {
            result = string.Format("{0}.{1}", GetInternalHelpContext(), text);
        }
        else
        {
            result = ActiveItem.GetHelpContext();
        }
        return result;
    }

    private void CanInsertAxisExpression(object sender, CanExecuteRoutedEventArgs args)
    {
        args.CanExecute = _chartViewModel.CanInsertInputExpression;
    }

    object ICloneable.Clone()
    {
        return Clone();
    }

    public ChartComponentControl Clone()
    {
        return new ChartComponentControl(this);
    }
}
