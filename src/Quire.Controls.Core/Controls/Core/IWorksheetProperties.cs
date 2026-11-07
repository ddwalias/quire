#nullable disable
using System.Windows;
using System.Windows.Media;
using Quire.Calculation.Core;

namespace Quire.Controls.Core;

public partial interface IWorksheetProperties
{
    PageLayoutMode WorksheetPageLayoutMode { get; }
    Thickness PageMargin { get; }
    Color PageBackgroundColor { get; }
    bool HasScriptableControls { get; }
    Size PageSizeValue { get; }
    Size GridCellSize { get; }
    string FilePath { get; }
    bool CacheWorksheet { get; }
    CalculationOptions DocCalculationOptions { get; }
    CalculationVariables DocCalculationVariables { get; }
    string WorksheetName { get; }
    string DisplayFilePath { get; }
    bool SpellCheckEnabled { get; }
    UnitSystems DefaultUnitSystem { get; set; }
    UnitSystems CustomBaseUnitSystem { get; set; }
    bool SimplifyUnitSystem { get; }
    string HelpServerPath { get; }
}
