#nullable disable
#pragma warning disable

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Quire.Calculation.Core;
using Quire.Controls.Core;
using Quire.Controls.Core.Serialization;
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

    static ChartComponentControl()
    {
        EngineErrorMessages.MergeDictionary(ResourceProvider<ChartComponentErrorsDictionary>.Instance);
    }

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
