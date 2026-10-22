#nullable disable
#pragma warning disable

namespace Quire.Calculation.Core;

partial interface ICalculatableWithDataExchange
{
    global::Quire.Calculation.Core.IDataExchangeModel DataExchangeModel { get; }
    global::System.Nullable<int> ComponentHandle { get; set; }
    int ComponentCookieMultiThreaded { get; }
    int IncrementComponentCookieMultiThreaded(bool isAutoCalc = false);
}
