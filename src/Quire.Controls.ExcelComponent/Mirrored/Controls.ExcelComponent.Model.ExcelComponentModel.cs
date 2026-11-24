using System;
using System.Collections.Generic;
using System.Linq;
using Quire.Calculation.Core;
using Quire.Controls.Core;
using Quire.Controls.Mathboard;

namespace Quire.Controls.ExcelComponent.Model;

public partial class ExcelComponentModel
{
    public EngineError GetError(ICalculatable calculatable)
    {
        return GetWrapper(calculatable).Error;
    }

    public ExcelComponentEquationType GetEquationType(ICalculatable calculatable)
    {
        return GetExcelComponentCalculatableWrapper(calculatable).ExcelComponentEquationType;
    }

    private IExcelComponentCalculatableWrapper GetExcelComponentCalculatableWrapper(ICalculatable calc)
    {
        return (IExcelComponentCalculatableWrapper)GetWrapper(calc);
    }

    private NodeHandleMapper _nodeHandleMapper = new NodeHandleMapper();

    private readonly HashSet<IOleBaseComponentCalculatableWrapper> _wasOutput = new HashSet<IOleBaseComponentCalculatableWrapper>();

    public event Func<bool> QueryHiddenFromCalculationContext;

    private IEnumerable<IWhiteboardItem> OutputsConnectedToTheEngine => CalculatablesConnectedToTheEngine.OfType<IWhiteboardItem>().Where(IsOutputItem);

    private IEnumerable<IWhiteboardItem> InputsConnectedToTheEngine => CalculatablesConnectedToTheEngine.OfType<IWhiteboardItem>().Where(IsInputItem);

    protected override void RemoveCalculatableWrapper(ICalculatable calc)
    {
        IExcelComponentCalculatableWrapper excelComponentCalculatableWrapper = GetExcelComponentCalculatableWrapper(calc);
        excelComponentCalculatableWrapper.ExcelComponentEquationTypeChanged -= HandleChildWrapperExcelComponentEquationTypeChanged;
        excelComponentCalculatableWrapper.ErrorChanged -= HandleChildWrapperErrorChanged;
        excelComponentCalculatableWrapper.Dispose();
        CalculatableWrapperMap.Remove(calc);
        UpdateErrorAccordingToChildren();
    }

    protected override void SetErrorByNodeHandle(EngineError error)
    {
        ICalculatable mappedEquation = _nodeHandleMapper.GetMappedEquation(error.NodeHandle);
        if (CalculatableChildren.Contains(mappedEquation) && error.Argument == 0)
        {
            error.NodeHandle = _nodeHandleMapper.GetMappedNodeNandle(error.NodeHandle);
            error.ErrorSource = mappedEquation;
            SetChildError(error);
            SetComponentError(null);
        }
        else
        {
            DebugTools.FailAssertion(error.NodeHandle.ToString());
            SetErrorFallback(error);
        }
    }

    protected override ICalculatable CreateCalculatableWrapper(ICalculatable calc, bool arg2)
    {
        if (CalculatableWrapperMap.ContainsKey(calc))
        {
            throw new ArgumentException("Logic Error: Attempting to add a calculatable wrapper which already exists.", "calc");
        }
        ExcelComponentCalculatableWrapper excelComponentCalculatableWrapper = new ExcelComponentCalculatableWrapper(calc);
        CalculatableWrapperMap[calc] = excelComponentCalculatableWrapper;
        excelComponentCalculatableWrapper.ExcelComponentEquationTypeChanged += HandleChildWrapperExcelComponentEquationTypeChanged;
        excelComponentCalculatableWrapper.ErrorChanged += HandleChildWrapperErrorChanged;
        return excelComponentCalculatableWrapper;
    }

    protected override void UpdateCustomErrorBasedOnPlacement(IWhiteboardItem item, bool placedAfterOleContainer)
    {
        ICalculatable calculatable = item as ICalculatable;
        if (DebugTools.AssertNotNull(calculatable, null).Failed)
        {
            return;
        }
        IOleBaseComponentCalculatableWrapper wrapper = GetWrapper(calculatable);
        if (wrapper.IsInputEquation || wrapper.IsOutputEquation)
        {
            if (wrapper.IsInputEquation & placedAfterOleContainer)
            {
                wrapper.SetCustomError("WronglyPlacedInput.Error.Key");
            }
            else if (wrapper.IsOutputEquation && !placedAfterOleContainer)
            {
                wrapper.SetCustomError("WronglyPlacedOutput.Error.Key");
            }
            else if (wrapper.CustomErrorType != OleWrapperCustomErrorType.Calculation)
            {
                wrapper.SetCustomError(null);
            }
            else
            {
                DebugTools.FailAssertion(wrapper.Error.ErrorCode);
            }
        }
        else if (wrapper.IsInvalidEquation)
        {
            wrapper.SetCustomError(placedAfterOleContainer ? "WronglyFormattedOutput.Error.Key" : "WronglyFormattedInput.Error.Key");
        }
        else
        {
            DebugTools.DevAssert(wrapper.IsUnknownEquation && wrapper.CustomErrorType != OleWrapperCustomErrorType.Calculation, null);
            wrapper.SetCustomError(null);
        }
        AddOrRemoveCalculatable(calculatable);
    }

    protected void AddOrRemoveCalculatable(ICalculatable calc)
    {
        IExcelComponentCalculatableWrapper excelComponentCalculatableWrapper = GetExcelComponentCalculatableWrapper(calc);
        if (DebugTools.AssertNotNull(excelComponentCalculatableWrapper, null).Failed)
        {
            return;
        }
        if (excelComponentCalculatableWrapper.ExcelComponentEquationType == ExcelComponentEquationType.Unknown || excelComponentCalculatableWrapper.HideFromCalculationContext)
        {
            return;
        }
        if (IsValidItem(excelComponentCalculatableWrapper))
        {
            if (!excelComponentCalculatableWrapper.IsConnectedToTheEngine)
            {
                FireCalculatableAdded(excelComponentCalculatableWrapper);
            }
        }
        else if (excelComponentCalculatableWrapper.IsConnectedToTheEngine)
        {
            FireCalculatableRemoved(excelComponentCalculatableWrapper);
        }
    }

    private void HandleChildWrapperExcelComponentEquationTypeChanged(ICalculatable equation, ExcelComponentEquationType oldType, ExcelComponentEquationType newType)
    {
        if (equation == null)
        {
            return;
        }
        IWhiteboardItem whiteboardItem = equation as IWhiteboardItem;
        if (DebugTools.AssertNotNull(whiteboardItem, null).Passed)
        {
            UpdateCustomErrorBasedOnPlacement(whiteboardItem);
        }
        IOleBaseComponentCalculatableWrapper wrapper = GetWrapper(equation);
        if (newType == ExcelComponentEquationType.Unknown)
        {
            if (oldType == ExcelComponentEquationType.Output)
            {
                _wasOutput.Add(wrapper);
            }
        }
        else
        {
            if (_wasOutput.Contains(wrapper) || oldType == ExcelComponentEquationType.Output || newType == ExcelComponentEquationType.Output)
            {
                FireRequestEvaluation();
            }
            _wasOutput.Remove(wrapper);
        }
    }

    protected override void FireCalculatableAdded(ICalculatable calculatable)
    {
        IExcelComponentCalculatableWrapper excelComponentCalculatableWrapper = (IExcelComponentCalculatableWrapper)calculatable;
        if (DebugTools.AssertNotNull(excelComponentCalculatableWrapper, null).Failed)
        {
            return;
        }
        if (DebugTools.DevAssert(!excelComponentCalculatableWrapper.IsConnectedToTheEngine, null).Failed)
        {
            return;
        }
        if (!IsValidItem(excelComponentCalculatableWrapper))
        {
            return;
        }
        base.FireCalculatableAdded(calculatable);
        if (excelComponentCalculatableWrapper.IsOutputEquation)
        {
            FireRequestEvaluation();
        }
    }

    protected override void FireCalculatableRemoved(ICalculatable calculatable)
    {
        IExcelComponentCalculatableWrapper excelComponentCalculatableWrapper = (IExcelComponentCalculatableWrapper)calculatable;
        if (DebugTools.AssertNotNull(excelComponentCalculatableWrapper, null).Failed)
        {
            return;
        }
        if (!excelComponentCalculatableWrapper.IsConnectedToTheEngine)
        {
            return;
        }
        base.FireCalculatableRemoved(calculatable);
        if (excelComponentCalculatableWrapper.IsOutputEquation || _wasOutput.Contains(excelComponentCalculatableWrapper))
        {
            FireRequestEvaluation();
            _wasOutput.Remove(excelComponentCalculatableWrapper);
        }
    }

    private string GetOutputList()
    {
        return string.Join(" ", OutputsConnectedToTheEngine.OfType<ICalculatable>().Select(GetOutputDefineLeftHandSide));
    }

    private string GetOutputDefineLeftHandSide(ICalculatable eq)
    {
        return _nodeHandleMapper.GetReplacedRl(GetWrapper(eq), GetWrapper(eq).LhsExpression);
    }

    public override object Clone()
    {
        ExcelComponentModel excelComponentModel = new ExcelComponentModel();
        CopyStateTo(excelComponentModel);
        return excelComponentModel;
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

    private bool CheckComponentEnabledCalcItems(bool checkOutputOnly)
    {
        // Release IL tests the null-guarded delegate result as one value, so the guard stays a ternary.
        if (!(QueryHiddenFromCalculationContext == null ? false : QueryHiddenFromCalculationContext()))
        {
            if (OutputsConnectedToTheEngine.Count() <= 0)
            {
                if (!checkOutputOnly)
                {
                    return InputsConnectedToTheEngine.Count() > 0;
                }
                return false;
            }
            return true;
        }
        return false;
    }
}
