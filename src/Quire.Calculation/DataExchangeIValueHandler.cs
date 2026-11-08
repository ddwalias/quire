#nullable disable
using System;
using System.Windows.Threading;

namespace Quire.Calculation;

internal partial class DataExchangeIValueHandler : DataExchangeHandler
{
    private IDataExchangeIValueModel _dataExchangeIValueModel;

    public DataExchangeIValueHandler(IDataExchangeIValueModel model, Dispatcher dispatcher, Action handleEngineNeedsUIThreadMessagesProcessed, CalculationController.EngineOptions engineOptions)
        : base(model, dispatcher, handleEngineNeedsUIThreadMessagesProcessed, engineOptions)
    {
        _dataExchangeIValueModel = model;
    }

    protected override void SetValueDispatch(string id, IValue value, bool isAutoCalc)
    {
        _dataExchangeIValueModel.SetValue(id, value, _engineOptions.ResultsTransform);
    }

    protected override IValue GetValueDispatch(string id, bool isCalculate, bool isAutoCalc)
    {
        return _dataExchangeIValueModel.GetValue(id);
    }
}
