#nullable disable
using System;
using System.Windows;

namespace Quire.Controls.Core.IORegions;

public partial interface IIORegion : IIORegionBase
{
    event Action<DependencyObject, IORegionChangeType> IORegionChanged;
}
