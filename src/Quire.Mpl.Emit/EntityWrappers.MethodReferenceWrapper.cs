using System.Collections.Generic;
using System.Reflection;

namespace System.Reflection.Emit.Quire.Mpl;

internal partial class EntityWrappers
{
    internal partial class MethodReferenceWrapper
    {
        internal readonly MethodBase method;
        internal int parentToken;

        public MethodReferenceWrapper(MethodBase method)
        {
            this.method = method;
        }

        public override bool Equals(object obj)
        {
            if (obj is MethodReferenceWrapper methodReferenceWrapper &&
                EqualityComparer<string>.Default.Equals(method.Name, methodReferenceWrapper.method.Name) &&
                EqualityComparer<string>.Default.Equals(method.ToString(), methodReferenceWrapper.method.ToString()) &&
                parentToken == methodReferenceWrapper.parentToken)
            {
                return GetHashCode() == methodReferenceWrapper.GetHashCode();
            }
            return false;
        }

        public override int GetHashCode()
        {
            string text = (method.DeclaringType != null) ? method.DeclaringType.FullName : string.Empty;
            return HashCode.Combine(method.Name + "@" + text, method.ToString(), parentToken);
        }
    }
}
