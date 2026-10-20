using System.Collections.Generic;

namespace System.Reflection.Emit.Quire.Mpl;

internal partial class EntityWrappers
{
    internal partial class TypeReferenceWrapper
    {
        internal readonly Type type;
        internal int parentToken;

        public TypeReferenceWrapper(Type type)
        {
            this.type = type;
        }

        public override bool Equals(object obj)
        {
            if (obj is TypeReferenceWrapper typeReferenceWrapper &&
                EqualityComparer<string>.Default.Equals(type.Name, typeReferenceWrapper.type.Name) &&
                EqualityComparer<string>.Default.Equals(type.Namespace, typeReferenceWrapper.type.Namespace) &&
                parentToken == typeReferenceWrapper.parentToken)
            {
                return GetHashCode() == typeReferenceWrapper.GetHashCode();
            }
            return false;
        }

        public override int GetHashCode()
        {
            string fullName = type.FullName;
            return HashCode.Combine(type.Name + "@" + fullName, type.Namespace, parentToken);
        }
    }
}
