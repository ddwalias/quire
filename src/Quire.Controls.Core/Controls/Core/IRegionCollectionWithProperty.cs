#nullable disable
using System.Windows.Input;

namespace Quire.Controls.Core;

public partial interface IRegionCollectionWithProperty
{
    bool AreAllTextRegions { get; }
    bool AreAllMathRegions { get; }
    bool HideFromCalculationContext { get; }
    void DispatchCommand(ICommand command, object parameter, PropertiesForUpdate propertyForUpdate);
}
