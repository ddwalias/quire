using Quire.utl_mgd;

using var ev = new SafeEvent("port-spike");
ev.Set();
if (!ev.WaitOneWithTimeout(100))
    return 1;
ev.Reset();
if (ev.WaitOneWithTimeout(0))
    return 2;

Console.WriteLine("managed consumer loaded the C++/CLI Utl spike and exercised its Win32 event");
return 0;
