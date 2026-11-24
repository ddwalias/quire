using System;
using System.Collections.Generic;
using System.Linq;
using Quire.Calculation.Core;
using Quire.Controls.Core;
using Quire.Controls.Mathboard;

namespace Quire.Controls.ExcelComponent.Model;

public partial class ExcelComponentModel
{
    private NodeHandleMapper _nodeHandleMapper = new NodeHandleMapper();

    private readonly HashSet<IOleBaseComponentCalculatableWrapper> _wasOutput = new HashSet<IOleBaseComponentCalculatableWrapper>();

    public EngineError GetError(ICalculatable calculatable)
    {
        return GetWrapper(calculatable).Error;
    }

    protected override IEnumerable<string> ComponentErrors
    {
        get
        {
            yield return "com_exception";
            yield return "excel_version_not_installed";
        }
    }

    public override string Rl { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }

    public override object Clone()
    {
        ExcelComponentModel excelComponentModel = new ExcelComponentModel();
        CopyStateTo(excelComponentModel);
        return excelComponentModel;
    }

    public ExcelComponentEquationType GetEquationType(ICalculatable calculatable)
    {
        return GetExcelComponentCalculatableWrapper(calculatable).ExcelComponentEquationType;
    }

    private static bool IsValidItem(IExcelComponentCalculatableWrapper wrapper)
    {
        OleWrapperCustomErrorType customErrorType = wrapper.CustomErrorType;
        bool flag = wrapper.IsInputEquation || wrapper.IsOutputEquation;
        DebugTools.DevAssert(wrapper.IsUnknownEquation || flag == (customErrorType != OleWrapperCustomErrorType.Formatting), null);
        if (flag && customErrorType != OleWrapperCustomErrorType.Placement)
        {
            return customErrorType != OleWrapperCustomErrorType.Formatting;
        }
        return false;
    }

    public override void SetIdentifierLabels(IDictionary<ResolvedId, string[]> idLabelMap)
    {
        Dictionary<ICalculatable, IDictionary<ResolvedId, string[]>> dictionary = new Dictionary<ICalculatable, IDictionary<ResolvedId, string[]>>();
        foreach (KeyValuePair<ResolvedId, string[]> item in idLabelMap)
        {
            ResolvedId key = item.Key;
            string[] value = item.Value;
            if (DebugTools.DevAssert(key.Calculatable.Equals(this), null).Failed)
            {
                continue;
            }
            IExcelComponentCalculatableWrapper excelComponentCalculatableWrapper = _nodeHandleMapper.GetMappedEquation((uint)key.Tag) as IExcelComponentCalculatableWrapper;
            if (DebugTools.AssertNotNull(excelComponentCalculatableWrapper, null).Passed)
            {
                IDictionary<ResolvedId, string[]> dictionary2;
                if (dictionary.ContainsKey(excelComponentCalculatableWrapper))
                {
                    dictionary2 = dictionary[excelComponentCalculatableWrapper];
                }
                else
                {
                    // The chained order (indexer target first, then the local) is what the Release IL requires.
                    dictionary[excelComponentCalculatableWrapper] = dictionary2 = new Dictionary<ResolvedId, string[]>();
                }
                ResolvedId key2 = new ResolvedId
                {
                    Calculatable = excelComponentCalculatableWrapper,
                    Name = key.Name,
                    Tag = (int)_nodeHandleMapper.GetMappedNodeNandle((uint)key.Tag)
                };
                if (dictionary2.ContainsKey(key2))
                {
                    DebugTools.DevAssert(dictionary2[key2].SequenceEqual(value), null);
                }
                else
                {
                    dictionary2.Add(key2, value);
                }
            }
        }
        dictionary.Each((KeyValuePair<ICalculatable, IDictionary<ResolvedId, string[]>> calcToLabelsPair) =>
        {
            calcToLabelsPair.Key.SetIdentifierLabels(calcToLabelsPair.Value);
        });
    }

    protected override void AddItems(IEnumerable<IWhiteboardItem> items)

    {

        ExcelContainer excelContainer = (ExcelContainer)items.FirstOrDefault((IWhiteboardItem item) => item is ExcelContainer);

        if (excelContainer != null)

        {

            excelContainer.PublicModel.RequestEvaluation += FireRequestEvaluation;

            IPublicExcelContainerModel publicModel = excelContainer.PublicModel;

            publicModel.QueryAttachedInputsOrOutputs = (QueryAttachedInputsOrOutputsHandler)Delegate.Combine(publicModel.QueryAttachedInputsOrOutputs, new QueryAttachedInputsOrOutputsHandler(CheckComponentEnabledCalcItems));

        }

        base.AddItems(items);

    }



    protected override void RemoveItems(IEnumerable<IWhiteboardItem> items)

    {

        ExcelContainer excelContainer = (ExcelContainer)items.FirstOrDefault((IWhiteboardItem item) => item is ExcelContainer);

        if (excelContainer != null)

        {

            IPublicExcelContainerModel publicModel = excelContainer.PublicModel;

            publicModel.QueryAttachedInputsOrOutputs = (QueryAttachedInputsOrOutputsHandler)Delegate.Remove(publicModel.QueryAttachedInputsOrOutputs, new QueryAttachedInputsOrOutputsHandler(CheckComponentEnabledCalcItems));

            excelContainer.PublicModel.RequestEvaluation -= FireRequestEvaluation;

        }

        base.RemoveItems(items);

    }

}
