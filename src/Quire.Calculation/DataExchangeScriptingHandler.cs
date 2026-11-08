#nullable disable
using System;
using System.Windows.Threading;
using Quire.Calculation.Core;

namespace Quire.Calculation;

internal partial class DataExchangeScriptingHandler : DataExchangeHandler
{
    private ICalculatableWithDataExchange _calculatableWithDataExchange;

    private IDataExchangeScriptingModel DataExchangeeScriptingModel => _calculatableWithDataExchange.DataExchangeModel as IDataExchangeScriptingModel;

    public DataExchangeScriptingHandler(ICalculatableWithDataExchange calculatableWithDataExchange, Dispatcher dispatcher, Action handleEngineNeedsUIThreadMessagesProcessed, CalculationController.EngineOptions engineOptions)
        : base(calculatableWithDataExchange.DataExchangeModel, dispatcher, handleEngineNeedsUIThreadMessagesProcessed, engineOptions)
    {
        _calculatableWithDataExchange = calculatableWithDataExchange;
    }

    protected override void SetValueDispatch(string id, IValue value, bool isAutoCalc)
    {
        DataExchangeeScriptingModel.SetValue(id, DataExchangeHandler.ConvertFromIValue(value), isAutoCalc);
    }

    protected override IValue GetValueDispatch(string id, bool isCalculate, bool isAutoCalc)
    {
        return DataExchangeHandler.ConvertToIValue(DataExchangeeScriptingModel.GetValue(id, isCalculate, isAutoCalc));
    }

    protected override void CommitIfNeeded()
    {
    }
}
