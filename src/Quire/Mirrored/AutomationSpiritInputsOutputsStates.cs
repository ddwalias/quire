using System.Linq;
namespace Quire;

public partial class AutomationSpiritInputsOutputsStates
{
    private readonly global::Quire.Controls.Core.IORegions.IInputOutputItemsStates _itemsWithStates;
    private global::System.Collections.Generic.IDictionary<string, global::Quire.Controls.Core.IORegions.IScalarItemWithOriginalState> _inputAliasToItemStatesMap;
    private global::System.Collections.Generic.IDictionary<string, global::Quire.Controls.Core.IORegions.IScalarItem> _outputAliasToItemStatesMap;

    public AutomationSpiritInputsOutputsStates(global::Quire.Controls.Core.IORegions.IInputOutputItemsStates itemsWithValues) { _itemsWithStates = itemsWithValues; }

    private global::Quire.Controls.Core.IORegions.IScalarItemWithOriginalState GetInputItem(int idx, out string alias)
    {
        global::Quire.Controls.Core.IORegions.IScalarItemWithOriginalState item;
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

    private global::Quire.Controls.Core.IORegions.IScalarItem GetOutputItem(int idx, out string alias)
    {
        global::Quire.Controls.Core.IORegions.IScalarItem item;
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

    private global::Quire.Controls.Core.IORegions.IScalarItemWithOriginalState GetInputItem(string alias)
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

    private global::Quire.Controls.Core.IORegions.IScalarItem GetOutputItem(string alias)
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
