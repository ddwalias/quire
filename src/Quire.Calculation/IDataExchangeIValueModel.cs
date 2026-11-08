namespace Quire.Calculation;

public partial interface IDataExchangeIValueModel : global::Quire.Calculation.Core.IDataExchangeModel
{
    void SetValue(string place, global::Quire.IValue value, global::Quire.Calculation.IValueToIResultNodeTransform resultsTransform);
    global::Quire.IValue GetValue(string place);
}
