namespace Quire.depman;

public partial interface Environment
{
    global::System.Collections.IDictionary Lookup(string name);

    global::System.Collections.IDictionary LookupEx(string name, ScopingMode m);

    void ExecuteInterruptibly(global::System.Threading.ThreadStart ts, bool bCoopOnly);

    int GetRegionId();
}
