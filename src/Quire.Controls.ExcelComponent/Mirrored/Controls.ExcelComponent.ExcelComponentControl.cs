#nullable disable
#pragma warning disable

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using Quire;
using Quire.Calculation.Core;
using Quire.Controls.Core;
using Quire.Controls.Core.Serialization;
using Quire.Controls.ExcelComponent.Model;
using Quire.Controls.ExcelComponent.View;
using Quire.Controls.ExcelComponent.ViewModel;
using Quire.PersistentData;
using Quire.Serialization;
using Quire.Wpf;

namespace Quire.Controls.ExcelComponent;

public partial class ExcelComponentControl
{
    private readonly ExcelComponentModel _exModel;
    private readonly ExcelComponentViewModel _exViewModel;
    internal const string ExcelIdentifier = "excel";

    protected override ISerializationStrategy SerializationStrategy { get; set; }
    protected override IDeserializationStrategy DeserializationStrategy { get; set; }

    internal ExcelContainer ExcelContainer => (ExcelContainer)OleContainerItem;

    static ExcelComponentControl()
    {
        EngineErrorMessages.MergeDictionary(ResourceProvider<ExcelComponentErrorsDictionary>.Instance);
    }

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

    public IList<IRegionPersistentData> SetRegionPersistentData(IDeS11NProvider deS11NProvider, object data)
    {
        excelComponentType excelComponentType2 = data as excelComponentType;
        if (excelComponentType2 != null)
        {
            double height = _exViewModel.Grid.GridCellSize.Height;
            double num = height * 3.0;
            if (excelComponentType2.resultRefSpecified)
            {
                ResultManager.Instance.SetResultByRefId(excelComponentType2.resultRef, this);
            }
            IList<IRegionPersistentData> result = DeserializationStrategy.SetRegions(excelComponentType2.regions, deS11NProvider);
            OleBaseComponentControl.SetItemWidth(ExcelContainer, _exViewModel.ContentWidth);
            OleBaseComponentControl.SetItemHeight(ExcelContainer, Math.Round(OleBaseComponentControl.GetItemHeight(ExcelContainer) / height) * height);
            if (excelComponentType2.InputSection == null)
            {
                excelComponentType2.InputSection = new oleComponentSectionType();
                excelComponentType2.InputSection.Visibility = OleComponentSectionVisibility.Visible;
            }
            if (!excelComponentType2.InputSection.HeightSpecified)
            {
                Point itemLocation = GetItemLocation(ExcelContainer);
                if (itemLocation.Y < num)
                {
                    double num2 = num - itemLocation.Y;
                    itemLocation.Y = num;
                    _exViewModel.ContentHeight += num2;
                }
                _exViewModel.OleContainerItemTop = itemLocation.Y;
                excelComponentType2.InputSection.Height = _exViewModel.OleContainerItemTop;
            }
            _exViewModel.InputSectionActualHeight = excelComponentType2.InputSection.Height;
            if (excelComponentType2.InputSection.HeightSpecified && excelComponentType2.InputSection.Visibility != OleComponentSectionVisibility.Visible)
            {
                _exViewModel.ContentHeight += _exViewModel.InputSectionActualHeight;
            }
            if (excelComponentType2.OutputSection == null)
            {
                excelComponentType2.OutputSection = new oleComponentSectionType();
                excelComponentType2.OutputSection.Visibility = OleComponentSectionVisibility.Visible;
            }
            if (!excelComponentType2.OutputSection.HeightSpecified)
            {
                double num3 = _exViewModel.ContentHeight - _exViewModel.InputSectionActualHeight;
                double num4 = num3 - _exViewModel.OleContainerItemSize.Height;
                if (num4 < num)
                {
                    double num5 = num - num4;
                    num4 = num;
                    _exViewModel.ContentHeight += num5;
                }
                excelComponentType2.OutputSection.Height = num4;
            }
            _exViewModel.OutputSectionActualHeight = excelComponentType2.OutputSection.Height;
            if (excelComponentType2.OutputSection.HeightSpecified && excelComponentType2.OutputSection.Visibility != OleComponentSectionVisibility.Visible)
            {
                _exViewModel.ContentHeight += _exViewModel.OutputSectionActualHeight;
            }
            IsInputSectionVisible = excelComponentType2.InputSection.Visibility == OleComponentSectionVisibility.Visible;
            IsOutputSectionVisible = excelComponentType2.OutputSection.Visibility == OleComponentSectionVisibility.Visible;
            _exViewModel.ContentHeight = Math.Round(_exViewModel.ContentHeight / height) * height;
            DeserializeRegionsEpilog(deS11NProvider, excelComponentType2);
            return result;
        }
        throw new ArgumentException("should be of " + typeof(excelComponentType).Name);
    }

    public object GetRegionPersistentData(IS11NProvider s11NProvider)
    {
        excelComponentType excelComponentType2 = new excelComponentType
        {
            Regions = SerializationStrategy.GetRegions(s11NProvider),
            InputSection = new oleComponentSectionType
            {
                Height = InputSectionIndicator.Height,
                HeightSpecified = true,
                Visibility = (IsInputSectionVisible ? OleComponentSectionVisibility.Visible : OleComponentSectionVisibility.Collapsed)
            },
            OutputSection = new oleComponentSectionType
            {
                Height = OutputSectionIndicator.Height,
                HeightSpecified = true,
                Visibility = (IsOutputSectionVisible ? OleComponentSectionVisibility.Visible : OleComponentSectionVisibility.Collapsed)
            }
        };
        int num = ResultManager.Instance.CreateResultRefId(this);
        if (IResultManagerHelper.IsResultRefIdSpecified(num))
        {
            excelComponentType2.resultRefSpecified = true;
            excelComponentType2.resultRef = num;
        }
        SerializeRegionsEpilog(s11NProvider, excelComponentType2);
        return excelComponentType2;
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
