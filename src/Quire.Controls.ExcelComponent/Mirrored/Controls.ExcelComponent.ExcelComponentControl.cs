#nullable disable
#pragma warning disable

using System;
using System.Collections.Generic;
using System.Windows.Input;
using Quire;
using Quire.Calculation.Core;
using Quire.Controls.Core;
using Quire.Controls.Core.Serialization;
using Quire.Controls.ExcelComponent.Model;
using Quire.Controls.ExcelComponent.View;
using Quire.Controls.ExcelComponent.ViewModel;

namespace Quire.Controls.ExcelComponent;

public partial class ExcelComponentControl
{
    private readonly ExcelComponentModel _exModel;
    private readonly ExcelComponentViewModel _exViewModel;
    internal const string ExcelIdentifier = "excel";

    protected override ISerializationStrategy SerializationStrategy { get; set; }
    protected override IDeserializationStrategy DeserializationStrategy { get; set; }

    internal ExcelContainer ExcelContainer => (ExcelContainer)OleContainerItem;

    private void ExcelInitialize()
    {
        BindCommands();
    }

    protected override IOleContainerItem CreateOleContainerItem()
    {
        return new ExcelContainer();
    }

    private string GetExcelSelectionOrDefault(bool isInput)
    {
        return ExcelContainer.ViewModel.GetExcelSelectionOrDefault(isInput);
    }

    private void BindCommands()
    {
        CommandBindings.Add(new CommandBinding(ExcelComponentCommands.InsertExcelInputExpression, OnInsertInputExpression, CanInsertInputExpression));
        CommandBindings.Add(new CommandBinding(ExcelComponentCommands.InsertExcelOutputExpression, OnInsertOutputExpression, CanInsertOutputExpression));
    }

    protected override string GetInternalHelpContext()
    {
        return "ExcelComponentControl";
    }

    public override string GetHelpContext()
    {
        if (ActiveItem == null)
        {
            return GetInternalHelpContext();
        }
        string result;
        string text = null;
        IEquationControl equationControl = ActiveItem as IEquationControl;
        if (equationControl != null)
        {
            text = equationControl.GetCurrentNodeIdentifier();
        }
        ExcelComponentEquationType equationType = GetEquationType(equationControl);
        if (text != null && text.Equals("excel") && equationType.IsValidType())
        {
            result = string.Format("{0}.{1}", GetInternalHelpContext(), equationType);
        }
        else
        {
            result = ActiveItem.GetHelpContext();
        }
        return result;
    }

    private void CanInsertOutputExpression(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = _exViewModel.CanInsertOutputExpression;
    }

    private void OnInsertOutputExpression(object sender, ExecutedRoutedEventArgs e)
    {
        string expression = string.Format("(:= @PLACEHOLDER (@INDEX {0} \"{1}\"))", "excel", GetExcelSelectionOrDefault(isInput: false));
        InsertNewOleComponentExpression(expression, navigateToEnd: false);
    }

    private void CanInsertInputExpression(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = _exViewModel.CanInsertInputExpression;
    }

    private void OnInsertInputExpression(object sender, ExecutedRoutedEventArgs e)
    {
        string expression = string.Format("(:= (@INDEX (@LABEL VARIABLE {0}) \"{1}\") @PLACEHOLDER)", "excel", GetExcelSelectionOrDefault(isInput: true));
        InsertNewOleComponentExpression(expression, navigateToEnd: true);
    }

    public void CollapseToIconicIfUnsetState(bool forceIfSet)
    {
        if (forceIfSet || !_exViewModel.IsAsIcon.HasValue)
        {
            _exViewModel.IsAsIcon = true;
        }
    }

    public EngineError GetError(EquationControl item)
    {
        return _exModel.GetError(item);
    }

    public ExcelComponentEquationType GetEquationType(IEquationControl equation)
    {
        return _exModel.GetEquationType(equation);
    }

    public bool SetValuesIntoExcelTable(IEnumerable<Pair<string, object>> valuesToSet, bool forceRun)
    {
        return ExcelContainer.SetValues(valuesToSet, forceRun);
    }

    public object[] GetValuesFromExcelTable(string[] cellIdxs, bool forceRun)
    {
        return ExcelContainer.GetValues(cellIdxs, forceRun);
    }

    public IEnumerable<ICalculatableResultOwner> ResultOwners => _exModel.CalculatableWrappers;

    object ICloneable.Clone()
    {
        return Clone();
    }

    public ExcelComponentControl Clone()
    {
        return new ExcelComponentControl(this);
    }
}
