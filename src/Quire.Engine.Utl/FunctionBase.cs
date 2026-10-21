namespace Quire.utl_mgd;

public abstract partial class FunctionBase
{
    public FunctionBase()
    {
    }

    public virtual object Invoke0() => Invoke(null);

    public virtual object Invoke1(object A_0) => Invoke(new object[1] { A_0 });

    public virtual object Invoke2(object A_0, object A_1) => Invoke(new object[2] { A_0, A_1 });

    public virtual object Invoke3(object A_0, object A_1, object A_2) => Invoke(new object[3] { A_0, A_1, A_2 });

    public virtual object Invoke4(object A_0, object A_1, object A_2, object A_3) => Invoke(new object[4] { A_0, A_1, A_2, A_3 });

    public virtual object Invoke5(object A_0, object A_1, object A_2, object A_3, object A_4) => Invoke(new object[5] { A_0, A_1, A_2, A_3, A_4 });

    public virtual object Invoke6(object A_0, object A_1, object A_2, object A_3, object A_4, object A_5) => Invoke(new object[6] { A_0, A_1, A_2, A_3, A_4, A_5 });

    public virtual object Invoke7(object A_0, object A_1, object A_2, object A_3, object A_4, object A_5, object A_6) => Invoke(new object[7] { A_0, A_1, A_2, A_3, A_4, A_5, A_6 });
}
