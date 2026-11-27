namespace Quire.Calculation;

public partial interface IDataExchangeIValueModel : global::Quire.Calculation.Core.IDataExchangeModel
{
    void SetValue(string place, global::Mathcad.IValue value, global::Quire.Calculation.IValueToIResultNodeTransform resultsTransform);
    global::Mathcad.IValue GetValue(string place);
}
