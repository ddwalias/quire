#nullable disable
using System;
using System.Collections.Generic;
using Quire.Calculation.Core;
using Quire.Wpf;

namespace Quire.Controls.Core.IORegions;

public partial interface IIORegionManager
{
    IIOItemCollection Inputs { get; }
    IIOItemCollection Outputs { get; }
    IShadowCalculationNotifier ShadowCalculationsChangedNotifier { get; }
    bool GetAndSetOperationsCompareAliasesCaseInsensitively { set; }
    int DefaultCalculationTimeout { get; set; }
    event Action<IEngineeringDocument, List<string>, List<string>> IORegionsSelected;
    event Action IODataChanged;
    bool IsValidAlias(string alias, bool asInput);
    void HandleElementAddedToParentContainer(IIORegion region);
    void HandleElementRemovedFromParentContainer(IIORegion region);
    void HandleElementsListSelectedInParentContainer(IEnumerable<IIORegionBase> ioItemsList);
    IList<ICalculatable> GetCalculationOverrides(IIORegion region);
    void ToggleInputRegions(IEnumerable<IIORegion> inputRegions);
    void ToggleOutputRegions(IEnumerable<IIORegion> outputRegions);
    bool CanToggleRegionsAsInput(IEnumerable<IIORegion> regions);
    bool CanToggleRegionsAsOutput(IEnumerable<IIORegion> regions);
    IOManagerErrorCode SetValue(string alias, double value, string units);
    IOManagerErrorCode SetValue(string alias, double[][] value, string units);
    IOManagerErrorCode SetSExprValue(string alias, string sExpression);
    IOManagerErrorCode SetStringValue(string alias, string stringValue);
    IScalarItem InputOutputGetRealValue(string alias, bool isInp);
    IMatrixItem InputOutputGetMatrixValue(string alias, bool isInp);
    IScalarItem InputOutputGetRealValueAs(string alias, bool isInp, string units);
    IMatrixItem InputOutputGetMatrixValueAs(string alias, bool isInp, string units);
    IInputOutputItemsStates CollectScalarInputsOutputsStates(GetInputValueTypes inputTypes, GetOutputValueTypes outputTypes, Func<INotifyProgress, bool> waitForCalculationStopped);
    IValuesSetter CreateScaledValueSettersCollection();
    string InputGetSExprValue(string alias);
    IStringItem InputOutputGetStringValue(string alias, bool isInp);
    ICommonValueItem InputOutputGetCalculatedValue(string alias, bool isInp);
}
