#nullable disable

using System.Collections;

namespace Quire;

public partial class DictionaryEntryWrapper
{
    public object Key { get; set; }

    public object Value { get; set; }

    public DictionaryEntryWrapper(object key, object value)
    {
        Key = key;
        Value = value;
    }

    public DictionaryEntry ToDictionaryEntry()
    {
        return new DictionaryEntry(Key, Value);
    }
}
