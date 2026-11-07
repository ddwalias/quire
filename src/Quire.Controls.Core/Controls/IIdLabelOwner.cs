#nullable disable
using System.Windows;

namespace Quire.Controls;

public partial interface IIdLabelOwner
{
    void HandleLabeledIdFormatChanged(DependencyPropertyChangedEventArgs args);
    void HandleLabeledIdFormatDictionaryChanged(DependencyPropertyChangedEventArgs args);
}
