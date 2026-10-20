using System.Collections.Generic;
using System.Reflection;

namespace System.Reflection.Emit.Quire.Mpl;

internal partial class EntityWrappers
{
    internal partial class AssemblyReferenceWrapper
    {
        internal readonly Assembly assembly;

        public AssemblyReferenceWrapper(Assembly assembly)
        {
            this.assembly = assembly;
        }

        public override bool Equals(object obj)
        {
            if (obj is AssemblyReferenceWrapper assemblyReferenceWrapper)
            {
                return EqualityComparer<string>.Default.Equals(assembly.GetName().FullName, assemblyReferenceWrapper.assembly.GetName().FullName);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(assembly.GetName().FullName);
        }
    }
}
