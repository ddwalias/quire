#nullable disable
using System;
using System.Windows;

namespace Quire.Controls.Core;

public partial interface IIndirectMessageAdornerOwner
{
    bool IsAdornedElementReady { get; }
    event Action<object, RoutedEventArgs> OnAdornedElementReady;
    FrameworkElement GetAdornedElement();
}
