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

    protected override ISerializationStrategy SerializationStrategy { get; set; }
    protected override IDeserializationStrategy DeserializationStrategy { get; set; }

    static ExcelComponentControl()
    {
        EngineErrorMessages.MergeDictionary(ResourceProvider<ExcelComponentErrorsDictionary>.Instance);
    }

    public ExcelComponentControl()
        : this(createExcelContainer: false)
    {
    }

    public ExcelComponentControl(Size? gridCellSize = null)
        : this(createExcelContainer: true, gridCellSize)
    {
    }

    private ExcelComponentControl(bool createExcelContainer, Size? gridCellSize = null)
        : base(new ExcelComponentViewModel(new ExcelComponentModel(), gridCellSize), new ExcelComponentDictionary())
    {
        _exModel = (ExcelComponentModel)_model;
        _exModel.QueryHiddenFromCalculationContext += () => HideFromCalculationContext;
        _exViewModel = (ExcelComponentViewModel)ViewModel;
        SerializationStrategy = new ExcelComponentSerializationStrategy(this, () => new excelComponentRegionType());
        DeserializationStrategy = new ExcelComponentDeserializationStrategy(this);
        ExcelInitialize();
        if (createExcelContainer)
        {
            CreateAndInitOleContainerItem();
        }
    }

    private ExcelComponentControl(ExcelComponentControl original)
        : base(original._exViewModel.CloneExcelComponentViewModel(), null)
    {
        _exModel = _model as ExcelComponentModel;
        _exViewModel = (ExcelComponentViewModel)ViewModel;
        SerializationStrategy = new ExcelComponentSerializationStrategy(this, () => new excelComponentRegionType());
        DeserializationStrategy = new ExcelComponentDeserializationStrategy(this);
        ExcelInitialize();
        if (Resources.MergedDictionaries.Count != original.Resources.MergedDictionaries.Count)
        {
            foreach (ResourceDictionary mergedDictionary in original.Resources.MergedDictionaries)
            {
                if (!Resources.MergedDictionaries.Contains(mergedDictionary))
                {
                    Resources.MergedDictionaries.Add(mergedDictionary);
                }
            }
        }
        if (_exModel != null)
        {
            _exModel.Resources = Resources.MergedDictionaries[0];
        }
    }

    private void ExcelInitialize()
    {
        BindCommands();
    }

    internal ExcelContainer ExcelContainer => (ExcelContainer)OleContainerItem;

    internal const string ExcelIdentifier = "excel";

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
        excelComponentType componentData = data as excelComponentType;
        if (componentData != null)
        {
            double height = _exViewModel.Grid.GridCellSize.Height;
            double num = height * 3.0;
            if (componentData.resultRefSpecified)
            {
                ResultManager.Instance.SetResultByRefId(componentData.resultRef, this);
            }
            IList<IRegionPersistentData> result = DeserializationStrategy.SetRegions(componentData.regions, deS11NProvider);
            OleBaseComponentControl.SetItemWidth(ExcelContainer, _exViewModel.ContentWidth);
            OleBaseComponentControl.SetItemHeight(ExcelContainer, Math.Round(OleBaseComponentControl.GetItemHeight(ExcelContainer) / height) * height);
            if (componentData.InputSection == null)
            {
                componentData.InputSection = new oleComponentSectionType();
                componentData.InputSection.Visibility = OleComponentSectionVisibility.Visible;
            }
            if (!componentData.InputSection.HeightSpecified)
            {
                Point itemLocation = GetItemLocation(ExcelContainer);
                if (itemLocation.Y < num)
                {
                    double heightDelta = num - itemLocation.Y;
                    itemLocation.Y = num;
                    _exViewModel.ContentHeight += heightDelta;
                }
                _exViewModel.OleContainerItemTop = itemLocation.Y;
                componentData.InputSection.Height = _exViewModel.OleContainerItemTop;
            }
            _exViewModel.InputSectionActualHeight = componentData.InputSection.Height;
            if (componentData.InputSection.HeightSpecified && componentData.InputSection.Visibility != OleComponentSectionVisibility.Visible)
            {
                _exViewModel.ContentHeight += _exViewModel.InputSectionActualHeight;
            }
            if (componentData.OutputSection == null)
            {
                componentData.OutputSection = new oleComponentSectionType();
                componentData.OutputSection.Visibility = OleComponentSectionVisibility.Visible;
            }
            if (!componentData.OutputSection.HeightSpecified)
            {
                double remainingContentHeight = _exViewModel.ContentHeight - _exViewModel.InputSectionActualHeight;
                double outputSectionHeight = remainingContentHeight - _exViewModel.OleContainerItemSize.Height;
                if (outputSectionHeight < num)
                {
                    double extraHeight = num - outputSectionHeight;
                    outputSectionHeight = num;
                    _exViewModel.ContentHeight += extraHeight;
                }
                componentData.OutputSection.Height = outputSectionHeight;
            }
            _exViewModel.OutputSectionActualHeight = componentData.OutputSection.Height;
            if (componentData.OutputSection.HeightSpecified && componentData.OutputSection.Visibility != OleComponentSectionVisibility.Visible)
            {
                _exViewModel.ContentHeight += _exViewModel.OutputSectionActualHeight;
            }
            IsInputSectionVisible = componentData.InputSection.Visibility == OleComponentSectionVisibility.Visible;
            IsOutputSectionVisible = componentData.OutputSection.Visibility == OleComponentSectionVisibility.Visible;
            _exViewModel.ContentHeight = Math.Round(_exViewModel.ContentHeight / height) * height;
            DeserializeRegionsEpilog(deS11NProvider, componentData);
            return result;
        }
        throw new ArgumentException("should be of " + typeof(excelComponentType).Name);
    }

    public object GetRegionPersistentData(IS11NProvider s11NProvider)
    {
        excelComponentType componentData = new excelComponentType
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
            componentData.resultRefSpecified = true;
            componentData.resultRef = num;
        }
        SerializeRegionsEpilog(s11NProvider, componentData);
        return componentData;
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
