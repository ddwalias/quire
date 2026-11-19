namespace Quire;

public partial class AutomationSpiritInputsOutputsStates
{
    private readonly global::Quire.Controls.Core.IORegions.IInputOutputItemsStates _itemsWithStates;
    private global::System.Collections.Generic.IDictionary<string, global::Quire.Controls.Core.IORegions.IScalarItemWithOriginalState> _inputAliasToItemStatesMap;
    private global::System.Collections.Generic.IDictionary<string, global::Quire.Controls.Core.IORegions.IScalarItem> _outputAliasToItemStatesMap;

    public AutomationSpiritInputsOutputsStates(global::Quire.Controls.Core.IORegions.IInputOutputItemsStates itemsWithValues) { _itemsWithStates = itemsWithValues; }
}
