#nullable disable
using System.Windows;
using Quire.Mirror;

namespace Quire.Controls;

internal partial class WorksheetSectionDeserializationStrategy
{
    [MirrorVerified("PtcControlsWorksheetSectionDeserializationStrategyMechanicalDifferentialTests")]
    public override void AddRegion(UIElement element, Point location) => throw new NotImplementedException();

    [MirrorVerified("PtcControlsWorksheetSectionDeserializationStrategyMechanicalDifferentialTests")]
    public override void DeleteRegion(UIElement control) => throw new NotImplementedException();

    [MirrorVerified("PtcControlsWorksheetSectionDeserializationStrategyMechanicalDifferentialTests")]
    public override IList<UIElement> Regions => throw new NotImplementedException();
}
