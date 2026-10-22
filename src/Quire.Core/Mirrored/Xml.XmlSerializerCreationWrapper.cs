#nullable disable
#pragma warning disable

namespace Quire.Xml;

partial class XmlSerializerCreationWrapper
{
    private readonly global::System.Collections.Hashtable _duplicatedEnvVars = new();
    public XmlSerializerCreationWrapper()
    {
        global::System.Collections.Hashtable hashtable = new(global::System.StringComparer.OrdinalIgnoreCase);
        foreach (global::System.Collections.DictionaryEntry environmentVariable in global::System.Environment.GetEnvironmentVariables())
        {
            string text = environmentVariable.Key.ToString();
            string value = environmentVariable.Value.ToString();
            if (hashtable.ContainsKey(text))
            {
                _duplicatedEnvVars.Add(text, value);
                global::System.Environment.SetEnvironmentVariable(text, null);
            }
            else
            {
                hashtable.Add(text, value);
            }
        }
    }

    public void Dispose()
    {
        foreach (global::System.Collections.DictionaryEntry duplicatedEnvVar in _duplicatedEnvVars)
        {
            global::System.Environment.SetEnvironmentVariable(duplicatedEnvVar.Key.ToString(), duplicatedEnvVar.Value.ToString());
        }
    }
}
