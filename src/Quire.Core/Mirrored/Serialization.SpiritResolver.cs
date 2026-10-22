#nullable disable
#pragma warning disable

namespace Quire.Serialization;

partial class SpiritResolver
{
    public static global::Quire.Serialization.SpiritResolver Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new global::Quire.Serialization.SpiritResolver();
            }
            return _instance;
        }
    }
}
