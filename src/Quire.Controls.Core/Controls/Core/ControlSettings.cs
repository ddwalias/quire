#nullable disable
namespace Quire.Controls.Core;

public partial class ControlSettings
{
    public static readonly ControlSettings Instance = new ControlSettings();

    public bool HandleFocusForIme
    {
        get
        {
            return Properties.Settings.Default.HandleFocusForIme;
        }
        set
        {
            Properties.Settings.Default.HandleFocusForIme = value;
            Properties.Settings.Default.Save();
        }
    }

    public bool EnableFontCaching
    {
        get
        {
            return Properties.Settings.Default.EnableFontCaching;
        }
        set
        {
            Properties.Settings.Default.EnableFontCaching = value;
            Properties.Settings.Default.Save();
        }
    }

    public int MaxNumberOf2dPoints => Properties.Settings.Default.MaxNumberOf2dPoints;

    public int MaxNumberOf3dPoints => Properties.Settings.Default.MaxNumberOf3dPoints;

    public int MinPointsForDensity => Properties.Settings.Default.MinPointsForDensity;

    public int MinPointsForForceDensity => Properties.Settings.Default.MinPointsForForceDensity;

    public int DensityPointsPrintDPI => Properties.Settings.Default.DensityPointsPrintDPI;

    private ControlSettings()
    {
    }
}
