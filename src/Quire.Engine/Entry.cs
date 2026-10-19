#nullable disable

namespace Quire.Engine;

internal partial struct Entry
{
    public int h;
    public NodeStatus s;
    public int client_cookie;
    public int trigger_handle;
    public int trigger_cookie;

    public Entry(int _h, NodeStatus _s, int _cc, int _th, int _tc)
    {
        h = _h;
        s = _s;
        client_cookie = _cc;
        trigger_handle = _tc;
        trigger_cookie = _tc;
    }
}
