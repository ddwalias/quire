using System.Collections.Generic;
using System.Reflection;

namespace System.Reflection.Emit.Quire.Mpl;

internal partial class EntityWrappers
{
    internal partial class AssemblyNameReferenceWrapper
    {
        internal readonly AssemblyName assemblyName;

        public AssemblyNameReferenceWrapper(AssemblyName assemblyName)
        {
            this.assemblyName = assemblyName;
        }

        public override bool Equals(object obj)
        {
            if (obj is AssemblyNameReferenceWrapper assemblyNameReferenceWrapper)
            {
                return EqualityComparer<string>.Default.Equals(assemblyName.FullName, assemblyNameReferenceWrapper.assemblyName.FullName);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(assemblyName.FullName);
        }
    }
}
