using System.Collections.Generic;
using System.Linq;
using Quire.Controls.Core.IORegions;

namespace Quire.AutomationServiceEngNotePlus;

public partial class AutomationSpiritInputsOutputsStates
{
    private readonly IInputOutputItemsStates _itemsWithStates;
    private IDictionary<string, IScalarItemWithOriginalState> _inputAliasToItemStatesMap;
    private IDictionary<string, IScalarItem> _outputAliasToItemStatesMap;

    public AutomationSpiritInputsOutputsStates(IInputOutputItemsStates itemsWithValues) { _itemsWithStates = itemsWithValues; }

    public int ScalarInputsCount
    {
        get
        {
            try
            {
                return (_itemsWithStates != null && _itemsWithStates.InputItems != null) ? _itemsWithStates.InputItems.Count : 0;
            }
            catch
            {
            }

            return 0;
        }
    }

    public int ScalarOutputsCount
    {
        get
        {
            try
            {
                return (_itemsWithStates != null && _itemsWithStates.OutputItems != null) ? _itemsWithStates.OutputItems.Count : 0;
            }
            catch
            {
            }

            return 0;
        }
    }

    public int GetInputScalarStateByIndex(int indexArg, out string aliasArg, out double valueArg, out string unitsArg)
    {
        IOManagerErrorCode result;
        try
        {
            result = GetInputItem(indexArg, out aliasArg).GetScalarState(out valueArg, out unitsArg);
        }
        catch
        {
            result = IOManagerErrorCode.UnknownError;
            aliasArg = null;
            valueArg = double.NaN;
            unitsArg = null;
        }

        return (int)result;
    }

    public int GetOutputScalarStateByIndex(int indexArg, out string aliasArg, out double valueArg, out string unitsArg)
    {
        IOManagerErrorCode result;
        try
        {
            result = GetOutputItem(indexArg, out aliasArg).GetScalarState(out valueArg, out unitsArg);
        }
        catch
        {
            result = IOManagerErrorCode.UnknownError;
            aliasArg = null;
            valueArg = double.NaN;
            unitsArg = null;
        }

        return (int)result;
    }

    public int GetInputScalarStateByAlias(string aliasArg, out double valueArg, out string unitsArg)
    {
        IOManagerErrorCode result;
        try
        {
            result = GetInputItem(aliasArg).GetScalarState(out valueArg, out unitsArg);
        }
        catch
        {
            result = IOManagerErrorCode.UnknownError;
            valueArg = double.NaN;
            unitsArg = null;
        }

        return (int)result;
    }

    public int GetOutputScalarStateByAlias(string aliasArg, out double valueArg, out string unitsArg)
    {
        IOManagerErrorCode result;
        try
        {
            result = GetOutputItem(aliasArg).GetScalarState(out valueArg, out unitsArg);
        }
        catch
        {
            result = IOManagerErrorCode.UnknownError;
            valueArg = double.NaN;
            unitsArg = null;
        }

        return (int)result;
    }

    private IScalarItemWithOriginalState GetInputItem(int idx, out string alias)
    {
        IScalarItemWithOriginalState item;
        if (idx >= 0 && idx < ScalarInputsCount)
        {
            item = _itemsWithStates.InputItems[idx];
            alias = item.Alias;
        }
        else
        {
            item = null;
            alias = null;
        }

        return item;
    }

    private IScalarItem GetOutputItem(int idx, out string alias)
    {
        IScalarItem item;
        if (idx >= 0 && idx < ScalarOutputsCount)
        {
            item = _itemsWithStates.OutputItems[idx];
            alias = item.Alias;
        }
        else
        {
            item = null;
            alias = null;
        }

        return item;
    }

    private IScalarItemWithOriginalState GetInputItem(string alias)
    {
        if (_inputAliasToItemStatesMap == null && _itemsWithStates.InputItems.Any())
        {
            _inputAliasToItemStatesMap = _itemsWithStates.InputItems.ToDictionary(item => item.Alias);
        }

        if (_inputAliasToItemStatesMap == null || !_inputAliasToItemStatesMap.ContainsKey(alias))
        {
            return null;
        }

        return _inputAliasToItemStatesMap[alias];
    }

    private IScalarItem GetOutputItem(string alias)
    {
        if (_outputAliasToItemStatesMap == null && _itemsWithStates.OutputItems.Any())
        {
            _outputAliasToItemStatesMap = _itemsWithStates.OutputItems.ToDictionary(item => item.Alias);
        }

        if (_outputAliasToItemStatesMap == null || !_outputAliasToItemStatesMap.ContainsKey(alias))
        {
            return null;
        }

        return _outputAliasToItemStatesMap[alias];
    }
}
