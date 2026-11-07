#nullable disable
using System.Collections.Generic;

namespace Quire.Controls.Core.IORegions;

public partial interface IIORegionContainer
{
    IIORegionManager InputOutputRegionManager { get; set; }
    IEnumerable<IIORegionBase> IORegions { get; }
    bool CanToggleInputRegion();
    bool CanToggleOutputRegion();
    void ToggleInputRegion();
    void ToggleOutputRegion();
}
