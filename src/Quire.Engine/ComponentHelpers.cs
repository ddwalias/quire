#nullable disable
using Quire;
using Quire.Mpl.Run;

namespace Quire.Engine;

public partial struct ComponentHelpers
{
    public static int Lock(IMathcadComponent2 c)
    {
        if (c.LockComponent())
        {
            return 1;
        }
        throw new AppException("cannot_lock_component");
    }

    public static int Unlock(IMathcadComponent2 c)
    {
        c.UnlockComponent();
        return 0;
    }

    public static int SetValue(IMathcadComponent2 c, uint tag1, uint tag2, string place, object v)
    {
        try
        {
            c.SetValue(place, (IValue)(v as IValue));
            return 0;
        }
        catch (AppException ex)
        {
            if (ex.Argument == 1)
            {
                var inner = new AppException(ex.Message, 2u);
                throw new AppException(inner, tag1);
            }
            throw new AppException(ex, tag2);
        }
    }

    public static object GetValue(IMathcadComponent2 c, string place)
    {
        try
        {
            return c.GetValue(place);
        }
        catch (AppException ex)
        {
            if (ex.Argument == 1)
            {
                ex = new AppException(ex.Message, 2u);
            }
            throw ex;
        }
    }
}
