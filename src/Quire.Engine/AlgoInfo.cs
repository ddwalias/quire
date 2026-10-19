#nullable disable
using System.Runtime.InteropServices;

namespace Quire.Engine;

[Serializable]
[ClassInterface(ClassInterfaceType.AutoDual)]
[Guid("2D49A0DF-FCA6-4DD0-9814-32D2B7FF6003")]
public sealed partial class AlgoInfo
{
    private string m_name;
    private int m_tag;
    private int m_region;

    public AlgoInfo() { }

    public int Region
    {
        get => m_region;
        set => m_region = value;
    }

    public int Tag
    {
        get => m_tag;
        set => m_tag = value;
    }

    public string Name
    {
        get => m_name;
        set => m_name = value;
    }
}
