#nullable disable
#pragma warning disable

namespace Quire.Wpf;

partial class ResourceProvider<TDictionary>
{
    public static TDictionary Instance
    {
        get
        {
            if ((object)_dictionary == null)
            {
                _dictionary = global::System.Activator.CreateInstance<TDictionary>();
                VerifyResources(_dictionary, global::System.Array.Empty<global::System.Type>());
            }
            return _dictionary;
        }
    }
}
