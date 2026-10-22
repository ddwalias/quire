#nullable disable
#pragma warning disable

namespace Quire.Wpf;

partial interface IPropertyChangedEventArgsExtended
{
    object OldValue { get; }
    object NewValue { get; }
}
