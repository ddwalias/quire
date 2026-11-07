#nullable disable
#pragma warning disable
using System.ComponentModel;
using System.Globalization;
using System.Resources;

namespace Quire.Controls.Include.Properties;

internal partial class Resources
{
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    internal Resources() { }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static ResourceManager ResourceManager
    {
        get
        {
            if (resourceMan == null)
            {
                resourceMan = new ResourceManager("Ptc.Controls.Include.Properties.Resources", typeof(Resources).Assembly);
            }
            return resourceMan;
        }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static CultureInfo Culture
    {
        get => resourceCulture;
        set => resourceCulture = value;
    }
}
