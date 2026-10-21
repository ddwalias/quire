#nullable disable
using System;
using System.Reflection;
using Quire.Mirror;

namespace Quire;

public partial class ReflectionVisitor
{
    [MirrorVerified("Release IL")]
    public void Visit(object visitable)
    {
        try
        {
            MethodInfo method = GetType().GetMethod("Visit", BindingFlags.Instance | BindingFlags.Public | BindingFlags.ExactBinding, Type.DefaultBinder, new Type[1] { visitable.GetType() }, new ParameterModifier[0]);
            if (method != null && (method != _lastMethod || visitable != _lastVisitable))
            {
                _lastMethod = method;
                _lastVisitable = visitable;
                method.Invoke(this, new object[1] { visitable });
            }
        }
        catch (TargetInvocationException ex)
        {
            if (ex.InnerException != null)
            {
                throw ex.InnerException;
            }
            throw;
        }
    }

    [MirrorVerified("PtcReflectionVisitorMechanicalDifferentialTests")]
    protected ReflectionVisitor() { }
}
