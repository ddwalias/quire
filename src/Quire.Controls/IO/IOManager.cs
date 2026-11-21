#nullable disable
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using Quire.Calculation.Core;
using Quire.Calculation.Core.Support;
using Quire.Controls.Core;
using Quire.Controls.Core.IORegions;
using Quire.Controls.Core.Whiteboard;
using Quire.Core;
using Quire.EquationEditor.Tree;
using Quire.EquationEditor.Tree.Transform;
using Quire.Mirror;
using Quire.Wpf;

namespace Quire.Controls.IO;

public partial class IOManager
{
    private int _defaultCalculationTimeout;

    public int DefaultCalculationTimeout
    {
        get => _defaultCalculationTimeout;
        set
        {
            if (value < -1)
            {
                return;
            }
            _defaultCalculationTimeout = value;
        }
    }

    public bool IsValidAlias(string alias, bool isIn)
    {
        if (string.IsNullOrEmpty(alias))
        {
            return false;
        }
        if (isIn)
        {
            return Inputs.GetItem(alias, false) == null;
        }
        return Outputs.GetItem(alias, false) == null;
    }

    private void FireIODataChanged()
    {
        if (IODataChanged != null)
        {
            IODataChanged.Invoke();
        }
    }

    public IOManager(IEngineeringDocument doc)
    {
        _inputs = new IOItemCollection(true);
        _outputs = new IOItemCollection(false);
        _inputs.RangeChanged += HandleInputItemsCollectionRangeChanged;
        _inputs.AliasChanged += FireIODataChanged;
        _outputs.RangeChanged += HandleOutputItemsCollectionRangeChanged;
        _outputs.AliasChanged += FireIODataChanged;
        _shadowCalculatableNotifier = new ShadowCalculationNotifier();
        _document = doc;
        DefaultCalculationTimeout = 60;
    }

    [MirrorVerified("PtcControlsIOIOManagerMechanicalDifferentialTests")]
    private readonly IEngineeringDocument _document;

    [MirrorVerified("PtcControlsIOIOManagerMechanicalDifferentialTests")]
    private void FireWaitForCalculation(DependencyObject region, int seconds) => _document.FireWaitForCalculationIdle(seconds, ShadowCalculatable.ShadowResultsProcessingPriority_Send);

    public void HandleElementAddedToParentContainer(IIORegion ioRegion)
    {
        DependencyObject val = ioRegion as DependencyObject;
        if (IOTag.HasInputTag(val))
        {
            if (val is IInputRegion region && region.IsSuitableForInputTagging())
            {
                _inputs.HandleTaggedRegionAdded(val);
            }
            else
            {
                RemoveIOTagFromRegion(ioRegion, IOTagType.Input);
            }
        }
        if (IOTag.HasOutputTag(val))
        {
            if (val is IOutputRegion region2 && region2.IsSuitableForOutputTagging())
            {
                _outputs.HandleTaggedRegionAdded(val);
            }
            else
            {
                RemoveIOTagFromRegion(ioRegion, IOTagType.Output);
            }
        }
    }

    public void HandleElementRemovedFromParentContainer(IIORegion ioRegion)
    {
        DependencyObject region = ioRegion as DependencyObject;
        if (IOTag.HasInputTag(region))
        {
            _inputs.HandleTaggedRegionRemoved(region);
        }
        if (IOTag.HasOutputTag(region))
        {
            _outputs.HandleTaggedRegionRemoved(region);
        }
    }

    public void HandleElementsListSelectedInParentContainer(IEnumerable<IIORegionBase> ioItemsList)
    {
        ioItemsList.OfType<IIORegion>().Each(HandleElementSelectedInParentContainer);
    }

    private void HandleElementSelectedInParentContainer(IIORegion region)
    {
        if (IOTag.IsIOTagged(region as DependencyObject) && IORegionsSelected != null)
        {
            List<string> list = new List<string>();
            List<string> list2 = new List<string>();
            if (IOTag.HasInputTag(region as DependencyObject))
            {
                list.AddRange(IOItemsAliasManager.GetAliases(region as DependencyObject, true));
            }
            if (IOTag.HasOutputTag(region as DependencyObject))
            {
                list2.AddRange(IOItemsAliasManager.GetAliases(region as DependencyObject, false));
            }
            IORegionsSelected(_document, list, list2);
        }
    }

    public IList<ICalculatable> GetCalculationOverrides(IIORegion region)
    {
        DependencyObject val = region as DependencyObject;
        if (DebugTools.AssertNotNull(val, null).Failed)
        {
            return null;
        }
        ICalculationOverrideContainer calculationOverrides = CalculationOverridesHandler.GetCalculationOverrides(val);
        if (calculationOverrides != null)
        {
            return ((IEnumerable<ICalculatable>)calculationOverrides.CalculatableChildren).ToList();
        }
        return null;
    }

    public void ToggleInputRegions(IEnumerable<IIORegion> inputRegions)
    {
        if (inputRegions.IsNullOrEmpty())
        {
            return;
        }
        List<IIORegion> list = inputRegions.ToList().FindAll(CanToggleRegionAsInput);
        if (list.IsNullOrEmpty())
        {
            return;
        }
        ToggleRegionsDesignationCommand command = new ToggleRegionsDesignationCommand(CreateToggleTagCommandsCollection(list, true), "Toggle Inputs");
        command.Execute();
    }

    public void ToggleOutputRegions(IEnumerable<IIORegion> outputRegions)
    {
        if (outputRegions.IsNullOrEmpty())
        {
            return;
        }
        List<IIORegion> list = outputRegions.ToList().FindAll(CanToggleRegionAsOutput);
        if (list.IsNullOrEmpty())
        {
            return;
        }
        ToggleRegionsDesignationCommand command = new ToggleRegionsDesignationCommand(CreateToggleTagCommandsCollection(list, false), "Toggle Outputs");
        command.Execute();
    }

    public bool CanToggleRegionsAsInput(IEnumerable<IIORegion> regions) => regions.Any(CanToggleRegionAsInput);

    public bool CanToggleRegionsAsOutput(IEnumerable<IIORegion> regions) => regions.Any(CanToggleRegionAsOutput);

    [MirrorVerified("PtcControlsIOIOManagerMechanicalDifferentialTests")]
    public IScalarItem InputOutputGetRealValue(string alias, bool isInp) => InputOutputGetRealValueAs(alias, isInp, null);

    [MirrorVerified("PtcControlsIOIOManagerMechanicalDifferentialTests")]
    public IMatrixItem InputOutputGetMatrixValue(string alias, bool isInp) => InputOutputGetMatrixValueAs(alias, isInp, null);

    public static bool SetIOAlias(IOItem item, string alias)
    {
        IIORegionManager inputOutputRegionManager = DocumentAttachedProperties.GetInputOutputRegionManager(item.Region);
        bool flag = inputOutputRegionManager.IsValidAlias(alias, item.IsInput);
        if (flag)
        {
            Dictionary<string, string> iOAliasMap = IOItemsAliasManager.GetIOAliasMap(item.Region, item.IsInput);
            Dictionary<string, string> dictionary = new Dictionary<string, string>(iOAliasMap);
            string oldAlias = iOAliasMap[item.IdSexp];
            dictionary[item.IdSexp] = alias;
            if (item.IsInput)
            {
                inputOutputRegionManager.Inputs.ResetAlias(item, oldAlias, alias);
            }
            else
            {
                inputOutputRegionManager.Outputs.ResetAlias(item, oldAlias, alias);
            }
            IOItemsAliasManager.SetIOAliasMap(item.Region, dictionary, item.IsInput);
        }
        return flag;
    }

    [MirrorOf("Ptc.Controls.IO.IOManager+EvaluationHelper, Ptc.Controls")]
    private static partial class EvaluationHelper
    {
        public static IOManagerErrorCode AnalyzeEvaluationError(ISimpleCalculatable calculatable)
        {
            IOManagerErrorCode result;
            if (calculatable.Error == null)
            {
                result = IOManagerErrorCode.NoError;
            }
            else
            {
                result = calculatable.IsErrorOnScalingUnits() ? IOManagerErrorCode.UnitsIncompatible : IOManagerErrorCode.CalculationError;
            }
            return result;
        }
    }

    private static IOManagerErrorCode ScaledEvaluationResultToIOManagerErrorCode(ScaledEvaluation.ScaledEvaluationResult scaledEvaluationResult)
    {
        IOManagerErrorCode result;
        switch (scaledEvaluationResult)
        {
        case ScaledEvaluation.ScaledEvaluationResult.NoError:
            result = IOManagerErrorCode.NoError;
            break;
        case ScaledEvaluation.ScaledEvaluationResult.ResultTypeIncompatible:
            result = IOManagerErrorCode.ResultTypeIncompatible;
            break;
        case ScaledEvaluation.ScaledEvaluationResult.MatrixResultInvalid:
            result = IOManagerErrorCode.MatrixResultInvalid;
            break;
        default:
            result = IOManagerErrorCode.UnknownError;
            DebugTools.DevAssert(false, "ScaledEvaluationResultToIOManagerOperationResult {0} unsupported value", scaledEvaluationResult);
            break;
        }
        return result;
    }

    public static string GetIOAlias(IOItem item)
    {
        Dictionary<string, string> iOAliasMap = IOItemsAliasManager.GetIOAliasMap(item.Region, item.IsInput);
        if (iOAliasMap != null)
        {
            return iOAliasMap[item.IdSexp];
        }
        return string.Empty;
    }

    public static void ClearIOAlias(IOItem item)
    {
        Dictionary<string, string> iOAliasMap = IOItemsAliasManager.GetIOAliasMap(item.Region, item.IsInput);
        if (iOAliasMap != null && iOAliasMap[item.IdSexp] != null)
        {
            iOAliasMap.Remove(item.IdSexp);
        }
    }

    public IOManagerErrorCode SetValue(string alias, double value, string units)
    {
        UnitedScalarSetter unitedScalarSetter = new UnitedScalarSetter(alias, value, units);
        SetValue(unitedScalarSetter);
        return unitedScalarSetter.OperationResult;
    }

    public IOManagerErrorCode SetValue(string alias, double[][] value, string units)
    {
        UnitedMatrixSetter unitedMatrixSetter = new UnitedMatrixSetter(alias, value, units);
        SetValue(unitedMatrixSetter);
        return unitedMatrixSetter.OperationResult;
    }

    private void SetValue(ISingleValueSetter unitedSingleValueSetter)
    {
        SetValues(new ISingleValueSetter[1] { unitedSingleValueSetter }, DefaultCalculationTimeout);
    }

    public IOManagerErrorCode SetStringValue(string alias, string stringValue)
    {
        StringValueSetter stringValueSetter = new StringValueSetter(alias, stringValue);
        SetValue(stringValueSetter);
        return stringValueSetter.OperationResult;
    }

    public IOManagerErrorCode SetSExprValue(string alias, string sExpression)
    {
        SExpressionSetter sExpressionSetter = new SExpressionSetter(alias, sExpression);
        SetValue(sExpressionSetter);
        return sExpressionSetter.OperationResult;
    }

    public IScalarItem InputOutputGetRealValueAs(string alias, bool isInp, string units)
    {
        IScalarItem result;
        IUnitedValueGetter getter;
        if (isInp)
        {
            ScalarInputGetter inputGetter = new ScalarInputGetter(alias, GetInputValueTypes.OverriddenValues, units);
            result = inputGetter;
            getter = inputGetter;
        }
        else
        {
            ScalarOutputGetter outputGetter = new ScalarOutputGetter(alias, GetOutputValueTypes.OverriddenValues, units);
            result = outputGetter;
            getter = outputGetter;
        }
        GetValue(isInp, getter);
        return result;
    }

    public IMatrixItem InputOutputGetMatrixValueAs(string alias, bool isInp, string units)
    {
        UnitedMatrixGetter unitedMatrixGetter = new UnitedMatrixGetter(alias, isInp, units);
        GetValue(isInp, unitedMatrixGetter);
        return unitedMatrixGetter;
    }

    public IStringItem InputOutputGetStringValue(string alias, bool isInp)
    {
        StringGetter stringGetter = new StringGetter(alias, isInp);
        GetValue(isInp, stringGetter);
        return stringGetter;
    }

    public ICommonValueItem InputOutputGetCalculatedValue(string alias, bool isInp)
    {
        CommonValueGetter commonValueGetter = new CommonValueGetter(alias, isInp);
        GetValue(isInp, commonValueGetter);
        return commonValueGetter;
    }

    public string InputGetSExprValue(string alias)
    {
        IIOItem item = Inputs.GetItem(alias, GetAndSetOperationsCompareAliasesCaseInsensitively);
        if (item != null)
        {
            return ((IInputRegion)item.Region).GetDefinitionSExpression(item.IdSexp);
        }
        return null;
    }

    private static bool CanToggleRegionAsInput(IIORegion region)
    {
        if (region is IInputRegion region2)
        {
            return region2.IsSuitableForInputTagging();
        }
        return false;
    }

    private static bool CanToggleRegionAsOutput(IIORegion region)
    {
        if (region is IOutputRegion region2)
        {
            return region2.IsSuitableForOutputTagging();
        }
        return false;
    }

    private void InputRemoveAllCalculationOverrides(DependencyObject region)
    {
        foreach (ICalculatable item2 in CalculationOverridesHandler.RemoveCalculatableOverrides(region, CalculationOverridesTypes.InputOverriddenValueSetter))
        {
            Pair<IIORegion, ICalculatable> item = new Pair<IIORegion, ICalculatable>
            {
                First = region as IIORegion,
                Second = item2
            };
            _shadowCalculatableNotifier.Remove(item);
        }
        DebugTools.DevAssert(CalculationOverridesHandler.GetInputCalculationOverrides(region).IsNullOrEmpty(), null);
    }

    private void InputRemoveCalculationOverridesOnRemoveIOItems(IEnumerable<IIOItem> ioItem)
    {
        List<DependencyObject> list = new List<DependencyObject>();
        foreach (IOItem item2 in ioItem)
        {
            DependencyObject region = item2.Region;
            string idSexp = item2.IdSexp;
            ICalculatable calculatable = CalculationOverridesHandler.RemoveCalculationOverrideForSExpr(region, idSexp, CalculationOverridesTypes.InputOverriddenValueSetter);
            if (calculatable != null)
            {
                Pair<IIORegion, ICalculatable> item = new Pair<IIORegion, ICalculatable>
                {
                    First = region as IIORegion,
                    Second = calculatable
                };
                _shadowCalculatableNotifier.Remove(item);
            }
            if (!list.Contains(region))
            {
                list.Add(region);
            }
        }
        foreach (DependencyObject item3 in list)
        {
            if (_inputs.GetRegionIOItems(item3).IsNullOrEmpty())
            {
                DebugTools.DevAssert(CalculationOverridesHandler.GetInputCalculationOverrides(item3).IsNullOrEmpty(), null);
            }
        }
    }

    private void OutputAssertNoCalculationOverridesOnRemoveIOItems(IEnumerable<IIOItem> ioItem)
    {
        List<DependencyObject> list = new List<DependencyObject>();
        foreach (IOItem item in ioItem)
        {
            DependencyObject region = item.Region;
            if (!list.Contains(region))
            {
                list.Add(region);
            }
        }
        foreach (DependencyObject item2 in list)
        {
            if (DebugTools.DevAssert(_outputs.GetRegionIOItems(item2).IsNullOrEmpty(), null).Passed)
            {
                OutputAssertNoCalculationOverrides(item2);
            }
        }
    }

    private static void OutputAssertNoCalculationOverrides(DependencyObject region)
    {
        DebugTools.DevAssert(CalculationOverridesHandler.GetOutputCalculationOverrides(region).IsNullOrEmpty(), null);
    }

    private void HandleIORegionChanged(DependencyObject region, IORegionChangeType ioRegionChangeType)
    {
        if (region is IIORegion)
        {
            IIORegion iIORegion = region as IIORegion;
            if (IOTag.HasInputTag(region))
            {
                switch (ioRegionChangeType)
                {
                case IORegionChangeType.Content:
                    if (!CanToggleRegionAsInput(iIORegion))
                    {
                        DebugTools.FailAssertion(null, Array.Empty<object>());
                        _inputs.HandleTaggedRegionRemoved(region);
                        RemoveInputTagFromRegion(iIORegion);
                    }
                    else
                    {
                        _inputs.HandleTaggedRegionChanged(region);
                    }
                    break;
                case IORegionChangeType.CalcIsDisabledStatus:
                    if (CalculatableChildHider.IsHiddenFromCalculation(region))
                    {
                        InputRemoveAllCalculationOverrides(region);
                    }
                    break;
                default:
                    DebugTools.FailAssertion("unexpected IORegionChangeType {0}", ioRegionChangeType);
                    break;
                }
            }
            if (IOTag.HasOutputTag(region))
            {
                switch (ioRegionChangeType)
                {
                case IORegionChangeType.Content:
                    if (!CanToggleRegionAsOutput(iIORegion))
                    {
                        _outputs.HandleTaggedRegionRemoved(region);
                        RemoveOutputTagFromRegion(iIORegion);
                    }
                    else
                    {
                        _outputs.HandleTaggedRegionChanged(region);
                    }
                    break;
                case IORegionChangeType.CalcIsDisabledStatus:
                    if (CalculatableChildHider.IsHiddenFromCalculation(region))
                    {
                        OutputAssertNoCalculationOverrides(region);
                    }
                    break;
                default:
                    DebugTools.FailAssertion("unexpected IORegionChangeType {0}", ioRegionChangeType);
                    break;
                }
            }
        }
    }

    private void HandleInputItemsCollectionRangeChanged(object sender, NotifyCollectionChangedEventArgs args)
    {
        switch (args.Action)
        {
        case NotifyCollectionChangedAction.Remove:
            InputRemoveCalculationOverridesOnRemoveIOItems(args.OldItems.OfType<IOItem>());
            break;
        default:
            DebugTools.FailAssertion("Unsupported action: {0}", args.Action);
            break;
        case NotifyCollectionChangedAction.Add:
            break;
        }
        FireIODataChanged();
    }

    private void HandleOutputItemsCollectionRangeChanged(object sender, NotifyCollectionChangedEventArgs args)
    {
        switch (args.Action)
        {
        case NotifyCollectionChangedAction.Remove:
            OutputAssertNoCalculationOverridesOnRemoveIOItems(args.OldItems.OfType<IOItem>());
            break;
        default:
            DebugTools.FailAssertion("Unsupported action: {0}", args.Action);
            break;
        case NotifyCollectionChangedAction.Add:
            break;
        }
        FireIODataChanged();
    }

    private void SetIOTagOnRegion(IIORegion element, IOTagType type)
    {
        IOTagType tagType = IOTag.GetIOTag(element as DependencyObject) | type;
        IOTag.SetIOTag(element as DependencyObject, tagType);
        element.IORegionChanged += HandleIORegionChanged;
    }

    private void SetInputTagOnRegion(IIORegion element)
    {
        if (DebugTools.DevAssert(CanToggleRegionAsInput(element), null).Failed)
        {
            return;
        }
        SetIOTagOnRegion(element, IOTagType.Input);
    }

    private void SetOutputTagOnRegion(IIORegion element)
    {
        if (DebugTools.DevAssert(CanToggleRegionAsOutput(element), null).Failed)
        {
            return;
        }
        SetIOTagOnRegion(element, IOTagType.Output);
    }

    private void RemoveIOTagFromRegion(IIORegion element, IOTagType type)
    {
        IOTagType iOTagType = IOTag.GetIOTag(element as DependencyObject) & ~type;
        IOTag.SetIOTag(element as DependencyObject, iOTagType);
        if (iOTagType == IOTagType.None)
        {
            element.IORegionChanged -= HandleIORegionChanged;
        }
    }

    private void RemoveInputTagFromRegion(IIORegion element)
    {
        if (!IOTag.HasInputTag(element as DependencyObject))
        {
            return;
        }
        RemoveIOTagFromRegion(element, IOTagType.Input);
    }

    private void RemoveOutputTagFromRegion(IIORegion element)
    {
        if (!IOTag.HasOutputTag(element as DependencyObject))
        {
            return;
        }
        RemoveIOTagFromRegion(element, IOTagType.Output);
    }

    private void ToggleIOTagOnRegion(IIORegion ioRegion, bool asInp, bool set, bool toggleRegionBorder)
    {
        DependencyObject region = ioRegion as DependencyObject;
        if (set)
        {
            if (asInp)
            {
                SetInputTagOnRegion(ioRegion);
                _inputs.HandleTaggedRegionAdded(region);
            }
            else
            {
                SetOutputTagOnRegion(ioRegion);
                _outputs.HandleTaggedRegionAdded(region);
            }
        }
        else if (asInp)
        {
            _inputs.HandleTaggedRegionRemoved(region);
            RemoveInputTagFromRegion(ioRegion);
        }
        else
        {
            _outputs.HandleTaggedRegionRemoved(region);
            RemoveOutputTagFromRegion(ioRegion);
        }
        if (toggleRegionBorder)
        {
            WhiteboardDependencyProperties.ToggleRegionBorderVisibility(region);
        }
    }

    public void Dispose()
    {
        _inputs.RangeChanged -= HandleInputItemsCollectionRangeChanged;
        _inputs.AliasChanged -= FireIODataChanged;
        _outputs.RangeChanged -= HandleOutputItemsCollectionRangeChanged;
        _outputs.AliasChanged -= FireIODataChanged;
    }

    [MirrorVerified("PtcControlsIOIOManagerMechanicalDifferentialTests")]
    public IValuesSetter CreateScaledValueSettersCollection() => new ValuesSetter(this);

    [MirrorVerified("PtcControlsIOIOManagerMechanicalDifferentialTests")]
    private void GetValue(bool asInp, IUnitedValueGetter unitedValueGetter) => GetComputedValueScaledBy(asInp, unitedValueGetter);

    [MirrorVerified("PtcControlsIOIOManagerMechanicalDifferentialTests")]
    public bool GetAndSetOperationsCompareAliasesCaseInsensitively { private get; set; }

    [MirrorVerified("PtcControlsIOIOManagerMechanicalDifferentialTests")]
    private readonly IOItemCollection _inputs;

    [MirrorVerified("PtcControlsIOIOManagerMechanicalDifferentialTests")]
    public IIOItemCollection Inputs => _inputs;

    [MirrorVerified("PtcControlsIOIOManagerMechanicalDifferentialTests")]
    private readonly IOItemCollection _outputs;

    [MirrorVerified("PtcControlsIOIOManagerMechanicalDifferentialTests")]
    public IIOItemCollection Outputs => _outputs;

    [MirrorVerified("PtcControlsIOIOManagerMechanicalDifferentialTests")]
    private readonly ShadowCalculationNotifier _shadowCalculatableNotifier;

    [MirrorVerified("PtcControlsIOIOManagerMechanicalDifferentialTests")]
    public IShadowCalculationNotifier ShadowCalculationsChangedNotifier => _shadowCalculatableNotifier;

    [MirrorVerified("PtcControlsIOIOManagerMechanicalDifferentialTests")]
    public event Action<IEngineeringDocument, List<string>, List<string>> IORegionsSelected;

    [MirrorVerified("PtcControlsIOIOManagerMechanicalDifferentialTests")]
    public event Action IODataChanged;

    [MirrorOf("Ptc.Controls.IO.IOManager+InputOutputStateGetterNotifier, Ptc.Controls")]
    private class InputOutputStateGetterNotifier : INotifyProgress, IDisposable
    {
        private const int ProgressZero = 0;
        private const int ProgressNotStarted = -1;
        private const int ProgressAborted = -2;

        private int _progress = -1;
        private readonly int _originalCount;
        private DispatcherTimer _timer;
        private readonly List<ICalcWithSyncStatus> _calculatables;

        public event ProgressChangedEventHandler ProgressChanged;
        public event Action<bool> ProgressStopped;

        public InputOutputStateGetterNotifier(IEnumerable<ICalcWithSyncStatus> shadows)
        {
            _calculatables = shadows.ToList();
            _originalCount = _calculatables.Count;
        }

        public void SetProgressAbortTimeout(int msThreshold)
        {
            if (_timer != null)
            {
                _timer.Stop();
            }
            if (msThreshold < 0)
            {
                return;
            }
            if (_timer == null)
            {
                _timer = new DispatcherTimer(DispatcherPriority.Background);
                _timer.Tick += delegate
                {
                    AbortProgressTracking();
                };
            }
            _timer.Interval = new TimeSpan(0, 0, 0, 0, msThreshold);
            if (_progress >= 0 && _progress != _originalCount)
            {
                _timer.Start();
            }
        }

        public void StartProgressTracking()
        {
            if (DebugTools.DevAssert(_progress == -1, null).Failed)
            {
                return;
            }
            _progress = 0;
            if (_originalCount != 0)
            {
                foreach (ICalcWithSyncStatus item in _calculatables.ToList())
                {
                    // Mathcad SynchronizationStatus.Synchronized is 3; Quire.Calculation.Core has not assigned = 3 yet
                    if (item.SynchronizationStatus == (SynchronizationStatus)3)
                    {
                        _progress++;
                        _calculatables.Remove(item);
                    }
                    else
                    {
                        Hookup(item);
                    }
                }
            }
            if (!ReportProgressOrComplete() && _timer != null)
            {
                _timer.Start();
            }
        }

        // Mathcad emits a shared ret at IL_0017 with bne.un.s to body, differing from Roslyn 10 branch layout
        public void AbortProgressTracking() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

        public bool IsProgressTrackingAborted()
        {
            return _progress == -2;
        }

        public bool IsTrackingInProgress
        {
            get
            {
                if (_progress >= 0)
                {
                    return _progress < _originalCount;
                }
                return false;
            }
        }

        public void Dispose()
        {
            AbortProgressTracking();
        }

        private void Hookup(ICalcWithSyncStatus calc)
        {
            calc.Synchronized += FireProgressChanged;
        }

        private void Unhookup(ICalcWithSyncStatus calc)
        {
            calc.Synchronized -= FireProgressChanged;
        }

        private void FireProgressChanged(ICalcWithSyncStatus simpleCalculatable)
        {
            _progress++;
            Unhookup(simpleCalculatable);
            _calculatables.Remove(simpleCalculatable);
            ReportProgressOrComplete();
        }

        private bool ReportProgressOrComplete()
        {
            if (_progress == _originalCount)
            {
                CompleteProgressTracking();
                return true;
            }
            if (ProgressChanged != null)
            {
                ProgressChanged(this, new ProgressChangedEventArgs(100 * _progress / _originalCount, null));
            }
            return false;
        }

        private void CompleteProgressTracking()
        {
            if (ProgressChanged != null)
            {
                ProgressChanged(this, new ProgressChangedEventArgs(100, null));
            }
            StopProgressTracking(true);
        }

        private void StopProgressTracking(bool finished)
        {
            if (_timer != null)
            {
                _timer.Stop();
            }
            if (ProgressStopped != null)
            {
                ProgressStopped(finished);
            }
            _calculatables.Each(Unhookup);
            _calculatables.Clear();
        }
    }

    [MirrorOf("Ptc.Controls.IO.IOManager+IUnitedValueGetter, Ptc.Controls")]
    private interface IUnitedValueGetter
    {
        string Alias { get; }
        IEnumerable<CalculationOverridesTypes> OverrideTypes { get; }
        string ScalingSExpression { get; }
        bool IsEmptyState(CalculationOverridesTypes type);
        void GetComputedValue(IScaledEvaluation shadowEval, CalculationOverridesTypes type);
        void SetOperationResult(IOManagerErrorCode code, CalculationOverridesTypes type);
        IOManagerErrorCode GetOperationResult(CalculationOverridesTypes type);
        bool IsScalingValid();
    }

    [MirrorOf("Ptc.Controls.IO.IOManager+ISingleValueSetter, Ptc.Controls")]
    private interface ISingleValueSetter
    {
        string Alias { get; }
        IOManagerErrorCode OperationResult { get; set; }
        bool IsValid();
        IShadowIdDefinition CreateShadowCalculatable(string sExprKey);
    }

    [MirrorOf("Ptc.Controls.IO.IOManager+UnitedScalarState, Ptc.Controls")]
    private partial class UnitedScalarState : IUnitedScalarState, IErrorState
    {
        internal UnitedScalarState(double value, string units, IOManagerErrorCode errorCode)
        {
            Value = value;
            Units = units;
            ErrorCode = errorCode;
        }

        [MirrorVerified("PtcControlsIOIOManagerMechanicalDifferentialTests")]
        public double Value { get; private set; }

        [MirrorVerified("PtcControlsIOIOManagerMechanicalDifferentialTests")]
        public string Units { get; private set; }

        [MirrorVerified("PtcControlsIOIOManagerMechanicalDifferentialTests")]
        public IOManagerErrorCode ErrorCode { get; private set; }
    }

    [MirrorOf("Ptc.Controls.IO.IOManager+UnitedMatrixState, Ptc.Controls")]
    private partial class UnitedMatrixState : IUnitedMatrixState, IErrorState
    {
        internal UnitedMatrixState(double[][] value, string units, IOManagerErrorCode errorCode)
        {
            Value = value;
            Units = units;
            ErrorCode = errorCode;
        }

        [MirrorVerified("PtcControlsIOIOManagerMechanicalDifferentialTests")]
        public double[][] Value { get; private set; }

        [MirrorVerified("PtcControlsIOIOManagerMechanicalDifferentialTests")]
        public string Units { get; private set; }

        [MirrorVerified("PtcControlsIOIOManagerMechanicalDifferentialTests")]
        public IOManagerErrorCode ErrorCode { get; private set; }
    }

    [MirrorOf("Ptc.Controls.IO.IOManager+StringState, Ptc.Controls")]
    private partial class StringState : IStringState, IErrorState
    {
        private string _value;

        internal StringState(string value, IOManagerErrorCode errorCode)
        {
            Value = value;
            ErrorCode = errorCode;
        }

        public string Value
        {
            get
            {
                DebugTools.DevAssert(_value == null || ErrorCode == IOManagerErrorCode.NoError, null);
                return _value;
            }
            private set => _value = value;
        }

        [MirrorVerified("PtcControlsIOIOManagerMechanicalDifferentialTests")]
        public IOManagerErrorCode ErrorCode { get; private set; }
    }

    [MirrorOf("Ptc.Controls.IO.IOManager+ErrorState, Ptc.Controls")]
    private partial class ErrorState : IErrorState
    {
        public ErrorState(IOManagerErrorCode errorCode)
        {
            ErrorCode = errorCode;
        }

        [MirrorVerified("PtcControlsIOIOManagerMechanicalDifferentialTests")]
        public IOManagerErrorCode ErrorCode { get; private set; }
    }

    [MirrorOf("Ptc.Controls.IO.IOManager+UnitedValueGetter, Ptc.Controls")]
    private abstract partial class UnitedValueGetter : IUnitedValueGetter
    {
        public abstract IEnumerable<CalculationOverridesTypes> OverrideTypes { get; }

        public string ScaledByUnits { get; private set; }

        public string Alias { get; private set; }

        public string ScalingSExpression { get; private set; }

        protected UnitedValueGetter(string alias, string scaledByUnits)
        {
            Alias = alias;
            ScaledByUnits = scaledByUnits;
            ScalingSExpression = EnumerableExtensions.IsNotEmpty<char>(scaledByUnits) ? InfixMonomialToSExpressionTransform.GetInstance(true).Transform(ScaledByUnits) : string.Empty;
        }

        public abstract bool IsEmptyState(CalculationOverridesTypes type);

        public abstract void GetComputedValue(IScaledEvaluation shadowEval, CalculationOverridesTypes type);

        public abstract void SetOperationResult(IOManagerErrorCode code, CalculationOverridesTypes type);

        public abstract IOManagerErrorCode GetOperationResult(CalculationOverridesTypes type);

        protected IOManagerErrorCode GetComputedScalar(IScaledEvaluation shadowEval, out double value, out string units)
        {
            string units2 = null;
            double value2 = double.NaN;
            IOManagerErrorCode iOManagerErrorCode = EvaluationHelper.AnalyzeEvaluationError(shadowEval);
            if (iOManagerErrorCode == IOManagerErrorCode.NoError)
            {
                iOManagerErrorCode = ScaledEvaluationResultToIOManagerErrorCode(shadowEval.GetComputedValue(out value2, out units2));
            }
            if (iOManagerErrorCode == IOManagerErrorCode.NoError && !AreUnitsCompatible(units2))
            {
                iOManagerErrorCode = IOManagerErrorCode.UnitsIncompatible;
            }
            value = ((iOManagerErrorCode == IOManagerErrorCode.NoError) ? value2 : double.NaN);
            units = ((iOManagerErrorCode == IOManagerErrorCode.NoError) ? units2 : null);
            return iOManagerErrorCode;
        }

        protected IOManagerErrorCode GetComputedMatrix(IScaledEvaluation shadowEval, out double[][] value, out string units)
        {
            string units2 = null;
            double[][] value2 = null;
            IOManagerErrorCode iOManagerErrorCode = EvaluationHelper.AnalyzeEvaluationError(shadowEval);
            if (iOManagerErrorCode == IOManagerErrorCode.NoError)
            {
                iOManagerErrorCode = ScaledEvaluationResultToIOManagerErrorCode(shadowEval.GetComputedValue(out value2, out units2));
            }
            if (iOManagerErrorCode == IOManagerErrorCode.NoError && !AreUnitsCompatible(units2))
            {
                iOManagerErrorCode = IOManagerErrorCode.UnitsIncompatible;
            }
            value = ((iOManagerErrorCode == IOManagerErrorCode.NoError) ? value2 : null);
            units = ((iOManagerErrorCode == IOManagerErrorCode.NoError) ? units2 : null);
            return iOManagerErrorCode;
        }

        protected static IOManagerErrorCode GetComputedString(IScaledEvaluation shadowEval, out string value)
        {
            IOManagerErrorCode iOManagerErrorCode = EvaluationHelper.AnalyzeEvaluationError(shadowEval);
            if (iOManagerErrorCode == IOManagerErrorCode.NoError)
            {
                iOManagerErrorCode = ScaledEvaluationResultToIOManagerErrorCode(shadowEval.GetComputedValue(out value));
            }
            else
            {
                value = null;
            }
            return iOManagerErrorCode;
        }

        private bool AreUnitsCompatible(string resultUnits)
        {
            if (!ScaledByUnits.IsNullOrEmpty())
            {
                return resultUnits.IsNullOrEmpty();
            }
            return true;
        }

        public bool IsScalingValid()
        {
            return ScaledByUnits.IsNullOrEmpty() || ScalingSExpression.IsNotEmpty();
        }
    }

    [MirrorOf("Ptc.Controls.IO.IOManager+ScalarOutputGetter, Ptc.Controls")]
    private class ScalarOutputGetter : UnitedValueGetter, IScalarItem, IItemAlias
    {
        private readonly GetOutputValueTypes _valueTypes;

        string IItemAlias.Alias => Alias;

        public ScalarOutputGetter(string alias, GetOutputValueTypes valueTypes, string scaledByUnits = default)
            : base(alias, scaledByUnits)
        {
            _valueTypes = valueTypes;
            DebugTools.DevAssert((uint)valueTypes > 0, null);
        }

        public override bool IsEmptyState(CalculationOverridesTypes type)
        {
            bool result;
            if (type == CalculationOverridesTypes.OutputOverriddenValueGetter)
            {
                result = ScalarState == null;
            }
            else
            {
                result = true;
                DebugTools.FailAssertion("Unsupported CalculationOverridesType");
            }
            return result;
        }

        public override void GetComputedValue(IScaledEvaluation shadowEval, CalculationOverridesTypes type)
        {
            double value;
            string units;
            IOManagerErrorCode computedScalar = GetComputedScalar(shadowEval, out value, out units);
            if (type == CalculationOverridesTypes.OutputOverriddenValueGetter)
            {
                ScalarState = new UnitedScalarState(value, units, computedScalar);
            }
            else
            {
                DebugTools.FailAssertion("Unsupported CalculationOverridesType");
            }
        }

        public override void SetOperationResult(IOManagerErrorCode code, CalculationOverridesTypes type)
        {
            if (DebugTools.DevAssert((uint)code > 0, null).Failed)
            {
                return;
            }
            if (type == CalculationOverridesTypes.OutputOverriddenValueGetter)
            {
                ScalarState = new UnitedScalarState(double.NaN, null, code);
            }
            else
            {
                DebugTools.FailAssertion("Unsupported CalculationOverridesType");
            }
        }

        public override IOManagerErrorCode GetOperationResult(CalculationOverridesTypes type)
        {
            IOManagerErrorCode result;
            if (type == CalculationOverridesTypes.OutputOverriddenValueGetter)
            {
                result = (DebugTools.AssertNotNull(ScalarState, null).Passed ? ScalarState.ErrorCode : IOManagerErrorCode.UnknownError);
            }
            else
            {
                result = IOManagerErrorCode.UnknownError;
                DebugTools.FailAssertion("Unsupported CalculationOverridesType");
            }
            return result;
        }

        public override IEnumerable<CalculationOverridesTypes> OverrideTypes
        {
            get
            {
                if (_valueTypes.HasFlag(GetOutputValueTypes.OverriddenValues))
                {
                    yield return CalculationOverridesTypes.OutputOverriddenValueGetter;
                }
            }
        }

        public IUnitedScalarState ScalarState { get; private set; }
    }

    [MirrorOf("Ptc.Controls.IO.IOManager+ItemAliasComparer, Ptc.Controls")]
    public partial class ItemAliasComparer : EqualityComparer<IItemAlias>
    {
        public override bool Equals(IItemAlias x, IItemAlias y)
        {
            return x.Alias.Equals(y.Alias, _caseInsensitiveAliasComparisonEnabled ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
        }

        public override int GetHashCode(IItemAlias obj)
        {
            return obj.Alias.ToUpperInvariant().GetHashCode();
        }

        [MirrorVerified("PtcControlsIOIOManagerMechanicalDifferentialTests")]
        private readonly bool _caseInsensitiveAliasComparisonEnabled;

        [MirrorVerified("PtcControlsIOIOManagerMechanicalDifferentialTests")]
        public ItemAliasComparer(bool caseInsensitiveAliasComparisonEnabled)
        {
            _caseInsensitiveAliasComparisonEnabled = caseInsensitiveAliasComparisonEnabled;
        }
    }

    [MirrorOf("Ptc.Controls.IO.IOManager+ScalarInputGetter, Ptc.Controls")]
    private class ScalarInputGetter : UnitedValueGetter, IScalarItemWithOriginalState, IScalarItem, IItemAlias
    {
        private readonly GetInputValueTypes _valueTypes;

        string IItemAlias.Alias => Alias;

        public ScalarInputGetter(string alias, GetInputValueTypes valueTypes, string scaledByUnits = default)
            : base(alias, scaledByUnits)
        {
            _valueTypes = valueTypes;
            DebugTools.DevAssert((uint)valueTypes > 0, null);
        }

        public override bool IsEmptyState(CalculationOverridesTypes type)
        {
            bool result;
            switch (type)
            {
            case CalculationOverridesTypes.InputOverriddenValueGetter:
                result = ScalarState == null;
                break;
            case CalculationOverridesTypes.InputOriginalValueGetter:
                result = OriginalScalarState == null;
                break;
            default:
                result = true;
                DebugTools.FailAssertion("Unsupported CalculationOverridesType");
                break;
            }
            return result;
        }

        public override void GetComputedValue(IScaledEvaluation shadowEval, CalculationOverridesTypes type)
        {
            double value;
            string units;
            IOManagerErrorCode computedScalar = GetComputedScalar(shadowEval, out value, out units);
            switch (type)
            {
            case CalculationOverridesTypes.InputOverriddenValueGetter:
                ScalarState = new UnitedScalarState(value, units, computedScalar);
                break;
            case CalculationOverridesTypes.InputOriginalValueGetter:
                OriginalScalarState = new UnitedScalarState(value, units, computedScalar);
                break;
            default:
                DebugTools.FailAssertion("Unsupported CalculationOverridesType");
                break;
            }
        }

        public override void SetOperationResult(IOManagerErrorCode code, CalculationOverridesTypes type)
        {
            if (DebugTools.DevAssert((uint)code > 0, null).Failed)
            {
                return;
            }
            switch (type)
            {
            case CalculationOverridesTypes.InputOverriddenValueGetter:
                ScalarState = new UnitedScalarState(double.NaN, null, code);
                break;
            case CalculationOverridesTypes.InputOriginalValueGetter:
                OriginalScalarState = new UnitedScalarState(double.NaN, null, code);
                break;
            default:
                DebugTools.FailAssertion("Unsupported CalculationOverridesType");
                break;
            }
        }

        public override IOManagerErrorCode GetOperationResult(CalculationOverridesTypes type)
        {
            IOManagerErrorCode result;
            switch (type)
            {
            case CalculationOverridesTypes.InputOverriddenValueGetter:
                result = (DebugTools.AssertNotNull(ScalarState, null).Passed ? ScalarState.ErrorCode : IOManagerErrorCode.UnknownError);
                break;
            case CalculationOverridesTypes.InputOriginalValueGetter:
                result = (DebugTools.AssertNotNull(OriginalScalarState, null).Passed ? OriginalScalarState.ErrorCode : IOManagerErrorCode.UnknownError);
                break;
            default:
                result = IOManagerErrorCode.UnknownError;
                DebugTools.FailAssertion("Unsupported CalculationOverridesType");
                break;
            }
            return result;
        }

        public override IEnumerable<CalculationOverridesTypes> OverrideTypes
        {
            get
            {
                if (_valueTypes.HasFlag(GetInputValueTypes.OriginalValues))
                {
                    yield return CalculationOverridesTypes.InputOriginalValueGetter;
                }
                if (_valueTypes.HasFlag(GetInputValueTypes.OverriddenValues))
                {
                    yield return CalculationOverridesTypes.InputOverriddenValueGetter;
                }
            }
        }

        public IUnitedScalarState OriginalScalarState { get; private set; }

        public IUnitedScalarState ScalarState { get; private set; }
    }

    [MirrorOf("Ptc.Controls.IO.IOManager+UnitedMatrixGetter, Ptc.Controls")]
    private class UnitedMatrixGetter : UnitedValueGetter, IMatrixItem, IItemAlias
    {
        private readonly CalculationOverridesTypes _getterType;

        string IItemAlias.Alias => Alias;

        public UnitedMatrixGetter(string alias, bool isInput, string scaledByUnits = default)
            : base(alias, scaledByUnits)
        {
            _getterType = isInput ? CalculationOverridesTypes.InputOverriddenValueGetter : CalculationOverridesTypes.OutputOverriddenValueGetter;
        }

        public override bool IsEmptyState(CalculationOverridesTypes type)
        {
            if (_getterType == type)
            {
                return MatrixState == null;
            }
            DebugTools.FailAssertion("Unsupported CalculationOverridesType");
            return true;
        }

        public override void GetComputedValue(IScaledEvaluation shadowEval, CalculationOverridesTypes type)
        {
            double[][] value;
            string units;
            IOManagerErrorCode computedMatrix = GetComputedMatrix(shadowEval, out value, out units);
            if (_getterType == type)
            {
                MatrixState = new UnitedMatrixState(value, units, computedMatrix);
            }
            else
            {
                DebugTools.FailAssertion("Unsupported CalculationOverridesType");
            }
        }

        public override void SetOperationResult(IOManagerErrorCode code, CalculationOverridesTypes type)
        {
            if (DebugTools.DevAssert((uint)code > 0, null).Failed)
            {
                return;
            }
            if (_getterType == type)
            {
                MatrixState = new UnitedMatrixState((double[][])null, null, code);
            }
            else
            {
                DebugTools.FailAssertion("Unsupported CalculationOverridesType");
            }
        }

        public override IOManagerErrorCode GetOperationResult(CalculationOverridesTypes type)
        {
            if (_getterType == type)
            {
                if (!DebugTools.AssertNotNull(MatrixState, null).Passed)
                {
                    return IOManagerErrorCode.UnknownError;
                }
                return MatrixState.ErrorCode;
            }
            DebugTools.FailAssertion("Unsupported CalculationOverridesType");
            return IOManagerErrorCode.UnknownError;
        }

        public override IEnumerable<CalculationOverridesTypes> OverrideTypes
        {
            get
            {
                yield return _getterType;
            }
        }

        public IUnitedMatrixState MatrixState { get; private set; }
    }

    [MirrorOf("Ptc.Controls.IO.IOManager+StringGetter, Ptc.Controls")]
    private class StringGetter : UnitedValueGetter, IStringItem, IItemAlias
    {
        private readonly CalculationOverridesTypes _getterType;

        public StringGetter(string alias, bool isInput)
            : base(alias, null)
        {
            _getterType = isInput ? CalculationOverridesTypes.InputOverriddenValueGetter : CalculationOverridesTypes.OutputOverriddenValueGetter;
        }

        public override bool IsEmptyState(CalculationOverridesTypes type)
        {
            if (DebugTools.DevAssert(_getterType == type, "Unsupported CalculationOverridesType").Failed)
            {
                return true;
            }
            return StringState == null;
        }

        public override void GetComputedValue(IScaledEvaluation shadowEval, CalculationOverridesTypes type)
        {
            string value;
            IOManagerErrorCode computedString = GetComputedString(shadowEval, out value);
            if (_getterType == type)
            {
                StringState = new StringState(value, computedString);
            }
            else
            {
                DebugTools.FailAssertion("Unsupported CalculationOverridesType");
            }
        }

        public override void SetOperationResult(IOManagerErrorCode code, CalculationOverridesTypes type)
        {
            if (DebugTools.DevAssert((uint)code > 0, null).Failed)
            {
                return;
            }
            if (_getterType == type)
            {
                StringState = new StringState(null, code);
            }
            else
            {
                DebugTools.FailAssertion("Unsupported CalculationOverridesType");
            }
        }

        public override IOManagerErrorCode GetOperationResult(CalculationOverridesTypes type)
        {
            if (_getterType == type)
            {
                if (!DebugTools.AssertNotNull(StringState, null).Passed)
                {
                    return IOManagerErrorCode.UnknownError;
                }
                return StringState.ErrorCode;
            }
            DebugTools.FailAssertion("Unsupported CalculationOverridesType");
            return IOManagerErrorCode.UnknownError;
        }

        public override IEnumerable<CalculationOverridesTypes> OverrideTypes
        {
            get
            {
                yield return _getterType;
            }
        }

        public IStringState StringState { get; private set; }
    }

    [MirrorOf("Ptc.Controls.IO.IOManager+CommonValueGetter, Ptc.Controls")]
    private class CommonValueGetter : UnitedValueGetter, ICommonValueItem, IItemAlias
    {
        private readonly CalculationOverridesTypes _getterType;

        public CommonValueGetter(string alias, bool isInput)
            : base(alias, null)
        {
            _getterType = isInput ? CalculationOverridesTypes.InputOverriddenValueGetter : CalculationOverridesTypes.OutputOverriddenValueGetter;
        }

        public override bool IsEmptyState(CalculationOverridesTypes type)
        {
            if (DebugTools.DevAssert(_getterType == type, "Unsupported CalculationOverridesType").Failed)
            {
                return true;
            }
            return ErrorState == null;
        }

        public override void GetComputedValue(IScaledEvaluation shadowEval, CalculationOverridesTypes type)
        {
            if (DebugTools.DevAssert(_getterType == type, "Unsupported CalculationOverridesType").Failed)
            {
                return;
            }
            IOManagerErrorCode iOManagerErrorCode = EvaluationHelper.AnalyzeEvaluationError(shadowEval);
            if (iOManagerErrorCode == IOManagerErrorCode.NoError)
            {
                iOManagerErrorCode = IOManagerErrorCode.ResultTypeIncompatible;
            }
            if (iOManagerErrorCode == IOManagerErrorCode.ResultTypeIncompatible)
            {
                iOManagerErrorCode = GetComputedScalar(shadowEval, out var value, out var units);
                if (iOManagerErrorCode == IOManagerErrorCode.NoError)
                {
                    ErrorState = new UnitedScalarState(value, units, iOManagerErrorCode);
                }
            }
            if (iOManagerErrorCode == IOManagerErrorCode.ResultTypeIncompatible)
            {
                iOManagerErrorCode = GetComputedMatrix(shadowEval, out var value2, out var units2);
                if (iOManagerErrorCode == IOManagerErrorCode.NoError)
                {
                    ErrorState = new UnitedMatrixState(value2, units2, iOManagerErrorCode);
                }
            }
            if (iOManagerErrorCode == IOManagerErrorCode.ResultTypeIncompatible)
            {
                iOManagerErrorCode = GetComputedString(shadowEval, out var value3);
                if (iOManagerErrorCode == IOManagerErrorCode.NoError)
                {
                    ErrorState = new StringState(value3, iOManagerErrorCode);
                }
            }
            if (iOManagerErrorCode != IOManagerErrorCode.NoError)
            {
                ErrorState = new ErrorState(iOManagerErrorCode);
            }
        }

        public override void SetOperationResult(IOManagerErrorCode code, CalculationOverridesTypes type)
        {
            if (DebugTools.DevAssert((uint)code > 0, null).Failed)
            {
                return;
            }
            if (DebugTools.DevAssert(_getterType == type, "Unsupported CalculationOverridesType").Failed)
            {
                return;
            }
            ErrorState = new ErrorState(code);
        }

        public override IOManagerErrorCode GetOperationResult(CalculationOverridesTypes type)
        {
            if (DebugTools.DevAssert(_getterType == type, "Unsupported CalculationOverridesType").Failed)
            {
                return IOManagerErrorCode.UnknownError;
            }
            if (!DebugTools.AssertNotNull(ErrorState, null).Passed)
            {
                return IOManagerErrorCode.UnknownError;
            }
            return ErrorState.ErrorCode;
        }

        public override IEnumerable<CalculationOverridesTypes> OverrideTypes
        {
            get
            {
                yield return _getterType;
            }
        }

        public IUnitedScalarState ScalarState => ErrorState as IUnitedScalarState;

        public IUnitedMatrixState MatrixState => ErrorState as IUnitedMatrixState;

        public IStringState StringState => ErrorState as IStringState;

        public IErrorState ErrorState { get; private set; }
    }

    [MirrorOf("Ptc.Controls.IO.IOManager+InputOutputItemsStates, Ptc.Controls")]
    public partial class InputOutputItemsStates : IInputOutputItemsStates
    {
        public InputOutputItemsStates(IList<IScalarItemWithOriginalState> inputs, IList<IScalarItem> outputs)
        {
            InputItems = inputs;
            OutputItems = outputs;
        }

        [MirrorVerified("PtcControlsIOIOManagerMechanicalDifferentialTests")]
        public IList<IScalarItemWithOriginalState> InputItems { get; private set; }

        [MirrorVerified("PtcControlsIOIOManagerMechanicalDifferentialTests")]
        public IList<IScalarItem> OutputItems { get; private set; }
    }

    [MirrorOf("Ptc.Controls.IO.IOManager+SetValueResults, Ptc.Controls")]
    private partial class SetValueResults : ISetValueResults
    {
        public IOManagerErrorCode GetResult(int index)
        {
            if (index < 0 || index >= Count)
            {
                return IOManagerErrorCode.UnknownError;
            }
            return _operationsResults[index];
        }

        public IOManagerErrorCode GetResult(string alias)
        {
            if (!_aliasToOperationResultsMap.ContainsKey(alias))
            {
                return IOManagerErrorCode.UnknownError;
            }
            return _aliasToOperationResultsMap[alias];
        }

        public SetValueResults(IEnumerable<ISingleValueSetter> setters)
        {
            _operationsResults = setters.Select(setter => setter.OperationResult).ToList();
            _aliasToOperationResultsMap = setters.ToDictionary(setter => setter.Alias, setter => setter.OperationResult);
        }

        [MirrorVerified("PtcControlsIOIOManagerMechanicalDifferentialTests")]
        private readonly IList<IOManagerErrorCode> _operationsResults;

        private readonly IDictionary<string, IOManagerErrorCode> _aliasToOperationResultsMap;

        [MirrorVerified("PtcControlsIOIOManagerMechanicalDifferentialTests")]
        public int Count => _operationsResults.Count;

        [MirrorVerified("PtcControlsIOIOManagerMechanicalDifferentialTests")]
        public IEnumerable<IOManagerErrorCode> AllResults => _operationsResults;
    }

    [MirrorOf("Ptc.Controls.IO.IOManager+UnitlessValueSetter, Ptc.Controls")]
    private abstract partial class UnitlessValueSetter : ISingleValueSetter
    {
        protected UnitlessValueSetter(string alias)
        {
            Alias = alias;
        }

        [MirrorVerified("PtcControlsIOIOManagerMechanicalDifferentialTests")]
        public string Alias { get; private set; }

        [MirrorVerified("PtcControlsIOIOManagerMechanicalDifferentialTests")]
        public IOManagerErrorCode OperationResult { get; set; }

        public abstract IShadowIdDefinition CreateShadowCalculatable(string sExprKey);

        public abstract bool IsValid();
    }

    [MirrorOf("Ptc.Controls.IO.IOManager+UnitedValueSetter, Ptc.Controls")]
    private abstract partial class UnitedValueSetter : UnitlessValueSetter
    {
        protected UnitedValueSetter(string alias, string units)
            : base(alias)
        {
            Units = units;
            ScalingSExpression = EnumerableExtensions.IsNotEmpty<char>(Units) ? InfixMonomialToSExpressionTransform.GetInstance(true).Transform(Units) : string.Empty;
        }

        [MirrorVerified("PtcControlsIOIOManagerMechanicalDifferentialTests")]
        private string Units { get; set; }

        [MirrorVerified("PtcControlsIOIOManagerMechanicalDifferentialTests")]
        protected string ScalingSExpression { get; private set; }

        private bool AreUnitsValid()
        {
            if (!Units.IsNullOrEmpty())
            {
                return ScalingSExpression.IsNotEmpty();
            }
            return true;
        }

        protected abstract bool IsValueValid();

        public override bool IsValid()
        {
            if (AreUnitsValid())
            {
                return IsValueValid();
            }
            return false;
        }
    }

    [MirrorOf("Ptc.Controls.IO.IOManager+SExpressionSetter, Ptc.Controls")]
    private partial class SExpressionSetter : UnitedValueSetter
    {
        private string _sexprScaling;

        public SExpressionSetter(string alias, string sexpr)
            : base(alias, null)
        {
            _sexprValue = sexpr;
            _sexprScaling = null;
            SExpressionHelperUtilites.SplitScalingAndValue(ref _sexprValue, out _sexprScaling);
        }

        [MirrorVerified("PtcControlsIOIOManagerMechanicalDifferentialTests")]
        private string _sexprValue;

        [MirrorVerified("PtcControlsIOIOManagerMechanicalDifferentialTests")]
        protected override bool IsValueValid() => EnumerableExtensions.IsNotEmpty<char>(_sexprValue);

        public override IShadowIdDefinition CreateShadowCalculatable(string sExprKey)
        {
            SexprDefinition sexprDefinition = new SexprDefinition(sExprKey, _sexprValue, _sexprScaling);
            if (!sexprDefinition.IsValid)
            {
                return null;
            }
            return sexprDefinition;
        }
    }

    [MirrorOf("Ptc.Controls.IO.IOManager+UnitedScalarSetter, Ptc.Controls")]
    private partial class UnitedScalarSetter : UnitedValueSetter
    {
        [MirrorVerified("PtcControlsIOIOManagerMechanicalDifferentialTests")]
        private readonly double _value;

        [MirrorVerified("PtcControlsIOIOManagerMechanicalDifferentialTests")]
        public UnitedScalarSetter(string alias, double value, string units) : base(alias, units)
        {
            _value = value;
        }

        [MirrorVerified("PtcControlsIOIOManagerMechanicalDifferentialTests")]
        protected override bool IsValueValid() => DoubleExtension.IsValid(_value);

        public override IShadowIdDefinition CreateShadowCalculatable(string sExprKey)
        {
            if (!SExpressionHelperUtilites.IsIdentifierSExpr(sExprKey))
            {
                return null;
            }
            return new IdDefinition(sExprKey, _value, ScalingSExpression);
        }
    }

    [MirrorOf("Ptc.Controls.IO.IOManager+StringValueSetter, Ptc.Controls")]
    private partial class StringValueSetter : UnitlessValueSetter
    {
        private readonly string _stringValueSExpr;

        public StringValueSetter(string alias, string stringValue)
            : base(alias)
        {
            _stringValueSExpr = stringValue == null ? null : string.Format("\"{0}\"", SExpressionUtility.EscapeStringValue(stringValue));
        }

        public override bool IsValid() => _stringValueSExpr != null;

        public override IShadowIdDefinition CreateShadowCalculatable(string sExprKey)
        {
            if (!SExpressionHelperUtilites.IsIdentifierSExpr(sExprKey))
            {
                return null;
            }
            SexprDefinition sexprDefinition = new SexprDefinition(sExprKey, _stringValueSExpr);
            if (!sexprDefinition.IsValid)
            {
                return null;
            }
            return sexprDefinition;
        }
    }

    [MirrorOf("Ptc.Controls.IO.IOManager+ValuesSetter, Ptc.Controls")]
    private partial class ValuesSetter : IValuesSetter
    {
        private readonly List<ISingleValueSetter> _setters;

        private readonly WeakReference _managerRef;

        public ValuesSetter(IOManager manager)
        {
            _managerRef = new WeakReference(manager);
            _setters = new List<ISingleValueSetter>();
        }

        public void AddScalarValue(string alias, double value, string units)
        {
            _setters.Add(new UnitedScalarSetter(alias, value, units));
        }

        public void AddMatrixValue(string alias, object[,] value, string units)
        {
            _setters.Add(new UnitedMatrixSetter(alias, value, units));
        }

        public void AddStringValue(string alias, string value)
        {
            _setters.Add(new StringValueSetter(alias, value));
        }

        public void AddSExprValue(string alias, string sexpression)
        {
            _setters.Add(new SExpressionSetter(alias, sexpression));
        }

        public ISetValueResults SetValues(int seconds)
        {
            if (!(_managerRef.Target is IOManager iOManager))
            {
                return null;
            }
            iOManager.SetValues(_setters, seconds);
            return new SetValueResults(_setters);
        }

        public IEnumerable<string> Aliases => _setters.Collect(setter => setter.Alias);
    }

    [MirrorOf("Ptc.Controls.IO.IOManager+UnitedMatrixSetter, Ptc.Controls")]
    private partial class UnitedMatrixSetter : UnitedValueSetter
    {
        private readonly double[,] _value;

        public UnitedMatrixSetter(string alias, object[,] value, string units)
            : base(alias, units)
        {
            if (value == null)
            {
                return;
            }
            int rows = value.GetLength(0);
            int cols = value.GetLength(1);
            bool valid = true;
            _value = new double[rows, cols];
            for (int i = 0; i < rows & valid; i++)
            {
                for (int j = 0; j < cols & valid; j++)
                {
                    if (value[i, j] is double)
                    {
                        _value[i, j] = (double)value[i, j];
                    }
                    else
                    {
                        valid = false;
                    }
                }
            }
            if (!valid)
            {
                _value = null;
            }
        }

        public UnitedMatrixSetter(string alias, double[][] value, string units)
            : base(alias, units)
        {
            if (value == null)
            {
                return;
            }
            int length = value.GetLength(0);
            int length2 = value[0].GetLength(0);
            for (int num = 1; num < length; num++)
            {
                if (value[num].GetLength(0) != length2)
                {
                    return;
                }
            }
            _value = new double[length, length2];
            for (int num2 = 0; num2 < length; num2++)
            {
                for (int num3 = 0; num3 < length2; num3++)
                {
                    _value[num2, num3] = value[num2][num3];
                }
            }
        }

        public override IShadowIdDefinition CreateShadowCalculatable(string sExprKey)
        {
            if (!SExpressionHelperUtilites.IsIdentifierSExpr(sExprKey))
            {
                return null;
            }
            return new MatrixDefinition(sExprKey, _value, ScalingSExpression);
        }

        protected override bool IsValueValid()
        {
            if (_value == null)
            {
                return false;
            }
            int length = _value.GetLength(0);
            int length2 = _value.GetLength(1);
            for (int num = 0; num < length; num++)
            {
                for (int num2 = 0; num2 < length2; num2++)
                {
                    if (!_value[num, num2].IsValid())
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
