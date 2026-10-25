#nullable disable
using System.ComponentModel;
using System.Globalization;
using System.Resources;
using Quire.Mirror;

namespace Quire.Controls.Core.Properties;

internal partial class Resources
{
    [MirrorVerified("PtcControlsCorePropertiesResourcesMechanicalDifferentialTests")]
    internal Resources() { }

    private static ResourceManager resourceMan;

    [MirrorVerified("PtcControlsCorePropertiesResourcesMechanicalDifferentialTests")]
    private static CultureInfo resourceCulture;

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static ResourceManager ResourceManager
    {
        get
        {
            if (resourceMan == null)
            {
                resourceMan = new ResourceManager("Ptc.Controls.Core.Properties.Resources", typeof(Resources).Assembly);
            }
            return resourceMan;
        }
    }

    [MirrorVerified("PtcControlsCorePropertiesResourcesMechanicalDifferentialTests")]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static CultureInfo Culture { get => resourceCulture; set => resourceCulture = value; }
}
