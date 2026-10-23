#nullable disable
#pragma warning disable

namespace Quire.Wpf;

partial class ResourceProvider<TDictionary>
{
    public static void VerifyResources(global::System.Windows.ResourceDictionary dictionary, params global::System.Type[] keysOwnerTypes)
    {
        if (keysOwnerTypes.IsNullOrEmpty())
        {
            global::System.Reflection.FieldInfo field = dictionary.GetType().GetField("KeysOwnerTypes", global::System.Reflection.BindingFlags.Static | global::System.Reflection.BindingFlags.Public);
            keysOwnerTypes = field != null ? (global::System.Type[])field.GetValue(null) : new global::System.Type[1] { dictionary.GetType() };
        }
        global::System.Type[] array = keysOwnerTypes;
        foreach (global::System.Type type in array)
        {
            global::System.Collections.Generic.Dictionary<object, string> dictionary2 = new global::System.Collections.Generic.Dictionary<object, string>();
            if (type.IsEnum)
            {
                foreach (object value in global::System.Enum.GetValues(type))
                {
                    dictionary2.Add(value, value.ToString());
                }
            }
            else
            {
                global::System.Reflection.FieldInfo[] fields = type.GetFields(global::System.Reflection.BindingFlags.Static | global::System.Reflection.BindingFlags.Public);
                foreach (global::System.Reflection.FieldInfo fieldInfo in fields)
                {
                    dictionary2.Add(fieldInfo.GetValue(null), fieldInfo.Name);
                }
            }
            DebugTools.DevAssert(dictionary2.IsNotEmpty(), null);
            foreach (global::System.Collections.Generic.KeyValuePair<object, string> item in dictionary2)
            {
                object obj = null;
                try
                {
                    obj = dictionary[item.Key];
                }
                catch
                {
                }
                DebugTools.DevAssert(obj != null, string.Format("No resource found for {0}.{1}", type.Name, item.Value));
            }
        }
    }

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
