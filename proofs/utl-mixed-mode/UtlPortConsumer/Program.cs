using Quire.utl_mgd;

using var ev = new SafeEvent("port-spike");
ev.Set();
if (!ev.WaitOneWithTimeout(100))
    return 1;
ev.Reset();
if (ev.WaitOneWithTimeout(0))
    return 2;

Console.WriteLine("managed consumer loaded the C++/CLI Utl spike and exercised its Win32 event");

var interruptControl = new InterruptControl();
InterruptControl.ThreadInitialize(interruptControl);
InterruptControl.ProhibitInterruption();
InterruptControl.AllowInterruption();
InterruptControl.ThreadInitialize(null);

Console.WriteLine("managed consumer exercised the C++/CLI interrupt control");

using var namedMutex = new SafeMutex($"UtlPortSpikeTest-{Guid.NewGuid():N}", 0);
namedMutex.WaitOne();
namedMutex.ReleaseMutex();

Console.WriteLine("managed consumer exercised the C++/CLI named mutex");
return 0;
