#nullable disable
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using Quire.Calculation.Core;
using Quire.Mirror;

namespace Quire.Controls.Core;

public partial class CachedWorksheetPropertiesSource
{
    [MirrorVerified("PtcControlsCoreCachedWorksheetPropertiesSourceMechanicalDifferentialTests")]
    public PageLayoutMode WorksheetPageLayoutMode { get; private set; }

    [MirrorVerified("PtcControlsCoreCachedWorksheetPropertiesSourceMechanicalDifferentialTests")]
    public Thickness PageMargin { get; private set; }

    [MirrorVerified("PtcControlsCoreCachedWorksheetPropertiesSourceMechanicalDifferentialTests")]
    public Color PageBackgroundColor { get; private set; }

    [MirrorVerified("PtcControlsCoreCachedWorksheetPropertiesSourceMechanicalDifferentialTests")]
    public bool HasScriptableControls { get; }

    [MirrorVerified("PtcControlsCoreCachedWorksheetPropertiesSourceMechanicalDifferentialTests")]
    public Size PageSizeValue { get; private set; }

    [MirrorVerified("PtcControlsCoreCachedWorksheetPropertiesSourceMechanicalDifferentialTests")]
    public Size GridCellSize { get; private set; }

    [MirrorVerified("PtcControlsCoreCachedWorksheetPropertiesSourceMechanicalDifferentialTests")]
    public string FilePath { get; private set; }

    [MirrorVerified("PtcControlsCoreCachedWorksheetPropertiesSourceMechanicalDifferentialTests")]
    public bool CacheWorksheet { get; private set; }

    [MirrorVerified("PtcControlsCoreCachedWorksheetPropertiesSourceMechanicalDifferentialTests")]
    public CalculationOptions DocCalculationOptions { get; private set; }

    [MirrorVerified("PtcControlsCoreCachedWorksheetPropertiesSourceMechanicalDifferentialTests")]
    public CalculationVariables DocCalculationVariables { get; private set; }

    [MirrorVerified("PtcControlsCoreCachedWorksheetPropertiesSourceMechanicalDifferentialTests")]
    public string WorksheetName { get; private set; }

    [MirrorVerified("PtcControlsCoreCachedWorksheetPropertiesSourceMechanicalDifferentialTests")]
    public string DisplayFilePath { get; private set; }

    [MirrorVerified("PtcControlsCoreCachedWorksheetPropertiesSourceMechanicalDifferentialTests")]
    public bool SpellCheckEnabled { get; private set; }

    [MirrorVerified("PtcControlsCoreCachedWorksheetPropertiesSourceMechanicalDifferentialTests")]
    public UnitSystems DefaultUnitSystem { get; set; }

    [MirrorVerified("PtcControlsCoreCachedWorksheetPropertiesSourceMechanicalDifferentialTests")]
    public UnitSystems CustomBaseUnitSystem { get; set; }

    [MirrorVerified("PtcControlsCoreCachedWorksheetPropertiesSourceMechanicalDifferentialTests")]
    public bool SimplifyUnitSystem { get; private set; }

    [MirrorVerified("PtcControlsCoreCachedWorksheetPropertiesSourceMechanicalDifferentialTests")]
    public string HelpServerPath { get; private set; }

    [MirrorVerified("PtcControlsCoreCachedWorksheetPropertiesSourceMechanicalDifferentialTests")]
    public event PropertyChangedEventHandler PropertyChanged;
}
