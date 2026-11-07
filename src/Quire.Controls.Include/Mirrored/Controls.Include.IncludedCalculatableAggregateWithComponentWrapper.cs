#nullable disable
#pragma warning disable
using Quire.Calculation.Core;

namespace Quire.Controls.Include;

public partial class IncludedCalculatableAggregateWithComponentWrapper
{
    public IncludedCalculatableAggregateWithComponentWrapper(ICalculatableAggregateWithComponent wrappedCalculatableAggregate)
        : base(wrappedCalculatableAggregate)
    {
    }

    public ICalculatableWithDataExchange WrappedCalculatableWithComponent => (ICalculatableWithDataExchange)WrappedCalculatable;

    public IDataExchangeModel DataExchangeModel => WrappedCalculatableWithComponent.DataExchangeModel;

    public int? ComponentHandle
    {
        get
        {
            return WrappedCalculatableWithComponent.ComponentHandle;
        }
        set
        {
            WrappedCalculatableWithComponent.ComponentHandle = value;
        }
    }

    public int ComponentCookieMultiThreaded => WrappedCalculatableWithComponent.ComponentCookieMultiThreaded;

    public int IncrementComponentCookieMultiThreaded(bool isAutoCalc = false)
    {
        return WrappedCalculatableWithComponent.IncrementComponentCookieMultiThreaded();
    }
}
