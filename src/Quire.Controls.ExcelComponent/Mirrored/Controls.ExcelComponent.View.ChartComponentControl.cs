#nullable disable
#pragma warning disable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Quire;
using Quire.Calculation.Core;
using Quire.Controls.Core;
using Quire.Controls.Core.Serialization;
using Quire.Controls.Core.Whiteboard;
using Quire.Controls.ExcelComponent.Model;
using Quire.Controls.ExcelComponent.ViewModel;
using Quire.PersistentData;
using Quire.Serialization;
using Quire.Wpf;

namespace Quire.Controls.ExcelComponent.View;

public partial class ChartComponentControl
{
    private readonly ChartComponentModel _chartModel;
    private readonly ChartComponentViewModel _chartViewModel;

    protected override ISerializationStrategy SerializationStrategy { get; set; }

    protected override IDeserializationStrategy DeserializationStrategy { get; set; }

    static ChartComponentControl()
    {
        EngineErrorMessages.MergeDictionary(ResourceProvider<ChartComponentErrorsDictionary>.Instance);
    }

    public ChartComponentControl()
        : this(createOleContainer: false)
    {
    }

    public ChartComponentControl(Size? gridCellSize)
        : this(createOleContainer: true, gridCellSize)
    {
    }

    private ChartComponentControl(bool createOleContainer, Size? gridCellSize = null)
        : base(new ChartComponentViewModel(new ChartComponentModel(), gridCellSize), new ChartComponentDictionary())
    {
        _chartModel = (ChartComponentModel)_model;
        _chartModel.RequestUpdateIdentifierLabels += HandleRequestUpdateIdentifierLabels;
        _chartViewModel = (ChartComponentViewModel)ViewModel;
        Init();
        if (createOleContainer)
        {
            CreateAndInitOleContainerItem();
            _chartViewModel.EndInit();
        }
    }

    private ChartComponentControl(ChartComponentControl original)
        : base(original._chartViewModel.CloneChartComponentViewModel(), null)
    {
        _chartModel = (ChartComponentModel)_model;
        _chartViewModel = (ChartComponentViewModel)ViewModel;
        Init();
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
        if (_chartModel != null)
        {
            _chartModel.Resources = Resources.MergedDictionaries[0];
        }
        _chartViewModel.EndInit();
    }

    private void Init()
    {
        SerializationStrategy = new ChartComponentSerializationStrategy(this, () => new chartComponentRegionType());
        DeserializationStrategy = new ChartComponentDeserializationStrategy(this);
        BindCommands();
    }

    private DispatcherOperation UpdateIdentifierLabelsOperation { get; set; }

    private void HandleRequestUpdateIdentifierLabels()
    {
        if (UpdateIdentifierLabelsOperation == null)
        {
            UpdateIdentifierLabelsOperation = Dispatcher.BeginInvoke((Action)(() =>
            {
                foreach (IChartComponentCalculatableWrapper item in _chartModel.CalculatableWrappers.Cast<IChartComponentCalculatableWrapper>())
                {
                    item.UpdateIdentifierLabelsIfNeeded();
                }
            }), (DispatcherPriority)9, Array.Empty<object>());
            UpdateIdentifierLabelsOperation.Aborted += delegate
            {
                UpdateIdentifierLabelsOperation = null;
            };
            UpdateIdentifierLabelsOperation.Completed += delegate
            {
                UpdateIdentifierLabelsOperation = null;
            };
        }
        else
        {
            DebugTools.DevAssert((int)UpdateIdentifierLabelsOperation.Status == 0, null);
        }
    }

    protected override IOleContainerItem CreateOleContainerItem()
    {
        return new ChartContainer();
    }

    public override void SetStatusSynchronized()
    {
        CalculatableChildren.Where((ICalculatable calc) => !(calc is IOleBaseComponentCalculatableWrapper)).Each((ICalculatable calc) =>
        {
            calc.SetStatusSynchronized();
        });
        base.SetStatusSynchronized();
    }

    private ChartContainer ChartContainer => OleContainerItem as ChartContainer;

    IChartContainer IChartComponentControl.ChartContainer => ChartContainer;

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

    private void OnInsertAxisExpression(object sender, ExecutedRoutedEventArgs args)
    {
        if (DebugTools.DevAssert(args.Parameter is ChartWrapperType, null).Failed)
        {
            return;
        }
        ChartWrapperType chartType = (ChartWrapperType)args.Parameter;
        uint? index;
        IEnumerable<IChartComponentCalculatableWrapper> enumerable = _chartModel.CalculatableWrappers.Cast<IChartComponentCalculatableWrapper>().Where((IChartComponentCalculatableWrapper wrapper) => wrapper.ChartWrapperType == chartType);
        if (chartType.IsIndexedTraceType())
        {
            if (enumerable.IsNullOrEmpty())
            {
                index = 1u;
            }
            else
            {
                uint num = 1u;
                foreach (IChartComponentCalculatableWrapper item in enumerable)
                {
                    if (!DebugTools.AssertNotNull(item.TraceId, null).Failed)
                    {
                        num = Math.Max(num, item.TraceId.Value);
                    }
                }
                index = num + 1;
            }
        }
        else
        {
            index = null;
        }
        string expression = ChartComponentCalculatableWrapper.ConstructAxisExpression(chartType, index);
        InsertNewOleComponentExpression(expression, navigateToEnd: true);
    }

    public IList<IRegionPersistentData> SetRegionPersistentData(IDeS11NProvider d11NProvider, object data)
    {
        chartComponentType chartComponentType2 = data as chartComponentType;
        if (chartComponentType2 == null)
        {
            throw new ArgumentException("should be of chartComponentType");
        }
        double height = _chartViewModel.Grid.GridCellSize.Height;
        double num = height * 3.0;
        if (chartComponentType2.resultRefSpecified)
        {
            ResultManager.Instance.SetResultByRefId(chartComponentType2.resultRef, this);
        }
        IList<IRegionPersistentData> result = DeserializationStrategy.SetRegions(chartComponentType2.regions, d11NProvider);
        OleBaseComponentControl.SetItemWidth(ChartContainer, _chartViewModel.ContentWidth);
        double num2 = OleBaseComponentControl.GetItemHeight(ChartContainer);
        if (_chartViewModel.ShouldAlignOleContainerItemWithGridLines)
        {
            num2 = Math.Round(num2 / height) * height;
        }
        OleBaseComponentControl.SetItemHeight(ChartContainer, num2);
        if (chartComponentType2.InputSection == null)
        {
            chartComponentType2.InputSection = new oleComponentSectionType();
            chartComponentType2.InputSection.Visibility = OleComponentSectionVisibility.Visible;
        }
        if (!chartComponentType2.InputSection.HeightSpecified)
        {
            Point itemLocation = GetItemLocation(ChartContainer);
            if (itemLocation.Y < num)
            {
                double num3 = num - itemLocation.Y;
                itemLocation.Y = num;
                _chartViewModel.ContentHeight += num3;
            }
            _chartViewModel.OleContainerItemTop = itemLocation.Y;
            chartComponentType2.InputSection.Height = _chartViewModel.OleContainerItemTop;
        }
        _chartViewModel.InputSectionActualHeight = chartComponentType2.InputSection.Height;
        if (chartComponentType2.InputSection.HeightSpecified && chartComponentType2.InputSection.Visibility != OleComponentSectionVisibility.Visible)
        {
            _chartViewModel.ContentHeight += _chartViewModel.InputSectionActualHeight;
        }
        IsInputSectionVisible = chartComponentType2.InputSection.Visibility == OleComponentSectionVisibility.Visible;
        _chartViewModel.OutputSectionActualHeight = _chartViewModel.SectionsMinimumHeight;
        DeserializeRegionsEpilog(d11NProvider, chartComponentType2);
        return result;
    }

    public object GetRegionPersistentData(IS11NProvider s11NProvider)
    {
        chartComponentType chartComponentType2 = new chartComponentType
        {
            Regions = SerializationStrategy.GetRegions(s11NProvider),
            InputSection = new oleComponentSectionType
            {
                Height = InputSectionIndicator.Height,
                HeightSpecified = true,
                Visibility = (IsInputSectionVisible ? OleComponentSectionVisibility.Visible : OleComponentSectionVisibility.Collapsed)
            }
        };
        int num = ResultManager.Instance.CreateResultRefId(this);
        if (IResultManagerHelper.IsResultRefIdSpecified(num))
        {
            chartComponentType2.resultRefSpecified = true;
            chartComponentType2.resultRef = num;
        }
        SerializeRegionsEpilog(s11NProvider, chartComponentType2);
        return chartComponentType2;
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
