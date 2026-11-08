#nullable disable
using System.ComponentModel;
using System.Globalization;
using System.Resources;
using Quire.Mirror;

namespace Quire.Controls.Properties;

public partial class Resources
{
    [MirrorVerified("PtcControlsPropertiesResourcesMechanicalDifferentialTests")]
    internal Resources() { }

    [MirrorVerified("PtcControlsPropertiesResourcesMechanicalDifferentialTests")]
    private static CultureInfo resourceCulture;

    [MirrorVerified("PtcControlsPropertiesResourcesMechanicalDifferentialTests")]
    public static CultureInfo Culture { get => resourceCulture; set => resourceCulture = value; }

    private static ResourceManager resourceMan;

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static ResourceManager ResourceManager
    {
        get
        {
            if (resourceMan == null)
            {
                resourceMan = new ResourceManager("Ptc.Controls.Properties.Resources", typeof(Resources).Assembly);
            }
            return resourceMan;
        }
    }

    public static string Command_ActivateRegion => ResourceManager.GetString("Command_ActivateRegion", resourceCulture);

    public static string Command_DeleteActiveRegion => ResourceManager.GetString("Command_DeleteActiveRegion", resourceCulture);

    public static string Command_NewImageRegion => ResourceManager.GetString("Command_NewImageRegion", resourceCulture);

    public static string Command_NewMathRegion => ResourceManager.GetString("Command_NewMathRegion", resourceCulture);

    public static string Command_NewTextRegion => ResourceManager.GetString("Command_NewTextRegion", resourceCulture);

    public static string Command_RecalculateWorksheet => ResourceManager.GetString("Command_RecalculateWorksheet", resourceCulture);

    public static string OpenFile_Filter_Images => ResourceManager.GetString("OpenFile_Filter_Images", resourceCulture);
}
