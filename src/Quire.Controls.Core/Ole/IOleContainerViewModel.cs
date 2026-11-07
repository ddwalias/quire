#nullable disable
using System.Windows;

namespace Quire.Controls.Core.Ole;

public partial interface IOleContainerViewModel
{
    Size MinSize { get; }
    Size DefaultMinSize { get; }
    Size EstablishInitialSize();
}
