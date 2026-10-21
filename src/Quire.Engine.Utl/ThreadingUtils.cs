using System.Threading;

namespace Quire.utl_mgd;

public abstract partial class ThreadingUtils
{
    public static void Spawn(Function f)
    {
        Thread thread = new Thread(new VFunc(f).Invoke, 4194304);
        thread.IsBackground = true;
        thread.Start();
    }

    public ThreadingUtils()
    {
    }
}
