#nullable disable
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using Quire.Calculation.Core;
using Quire.Mirror;

namespace Quire.Controls.Core;

internal partial class DummyWorksheetPropertiesSource
{
    private static DummyWorksheetPropertiesSource _instance;
    private static readonly PageLayoutMode _worksheetPageLayoutMode = default;
    private static readonly Thickness _pageMargin = new Thickness(1);
    private static readonly Size _pageSizeValue = new Size(1, 1);
    private static readonly Size _gridCellSize = new Size(1, 1);
    private static readonly CalculationOptions _docCalculationOptions = new CalculationOptions();
    private static readonly CalculationVariables _docCalculationVariables = new CalculationVariables();

    [MirrorVerified("PtcControlsCoreDummyWorksheetPropertiesSourceMechanicalDifferentialTests")]
    private DummyWorksheetPropertiesSource() { }

    [MirrorVerified("PtcControlsCoreDummyWorksheetPropertiesSourceMechanicalDifferentialTests")]
    public Color PageBackgroundColor { get; }

    [MirrorVerified("PtcControlsCoreDummyWorksheetPropertiesSourceMechanicalDifferentialTests")]
    public bool HasScriptableControls { get; }

    [MirrorVerified("PtcControlsCoreDummyWorksheetPropertiesSourceMechanicalDifferentialTests")]
    public event PropertyChangedEventHandler PropertyChanged;

    public PageLayoutMode WorksheetPageLayoutMode
    {
        get
        {
            DebugTools.FailAssertion();
            return _worksheetPageLayoutMode;
        }
    }

    public Thickness PageMargin
    {
        get
        {
            DebugTools.FailAssertion();
            return _pageMargin;
        }
    }

    public Size PageSizeValue
    {
        get
        {
            DebugTools.FailAssertion();
            return _pageSizeValue;
        }
    }

    public Size GridCellSize
    {
        get
        {
            DebugTools.FailAssertion();
            return _gridCellSize;
        }
    }

    public string FilePath
    {
        get
        {
            DebugTools.FailAssertion();
            return null;
        }
    }

    public bool CacheWorksheet
    {
        get
        {
            DebugTools.FailAssertion();
            return true;
        }
    }

    public CalculationOptions DocCalculationOptions
    {
        get
        {
            DebugTools.FailAssertion();
            return _docCalculationOptions;
        }
    }

    public CalculationVariables DocCalculationVariables
    {
        get
        {
            DebugTools.FailAssertion();
            return _docCalculationVariables;
        }
    }

    public string WorksheetName
    {
        get
        {
            DebugTools.FailAssertion();
            return string.Empty;
        }
    }

    public string DisplayFilePath
    {
        get
        {
            DebugTools.FailAssertion();
            return string.Empty;
        }
    }

    public bool SpellCheckEnabled
    {
        get
        {
            DebugTools.FailAssertion();
            return true;
        }
    }

    public UnitSystems DefaultUnitSystem
    {
        get
        {
            DebugTools.FailAssertion();
            return default;
        }
        set
        {
        }
    }

    public UnitSystems CustomBaseUnitSystem
    {
        get
        {
            DebugTools.FailAssertion();
            return default;
        }
        set
        {
        }
    }

    public bool SimplifyUnitSystem
    {
        get
        {
            DebugTools.FailAssertion();
            return true;
        }
    }

    public string HelpServerPath
    {
        get
        {
            DebugTools.FailAssertion();
            return string.Empty;
        }
    }

    public static DummyWorksheetPropertiesSource GetInstance()
    {
        return _instance ??= new DummyWorksheetPropertiesSource();
    }
}
