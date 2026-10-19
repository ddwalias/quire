#nullable disable
using System.Runtime.InteropServices;

namespace Quire.Engine;

[Serializable]
[ClassInterface(ClassInterfaceType.AutoDual)]
[Guid("6F2963E3-0A1A-11E0-BCD7-11F8E2A8C36C")]
public sealed partial class RedefInfo
{
    private string m_name;
    private string m_oldtype;
    private string m_oldlocation;
    private string m_newtype;

    public RedefInfo() { }

    public string NewType
    {
        get => m_newtype;
        set => m_newtype = value;
    }

    public string OldLocation
    {
        get => m_oldlocation;
        set => m_oldlocation = value;
    }

    public string OldType
    {
        get => m_oldtype;
        set => m_oldtype = value;
    }

    public string Name
    {
        get => m_name;
        set => m_name = value;
    }
}
