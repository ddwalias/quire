using System.Collections.Generic;
using System.Reflection;

namespace System.Reflection.Emit.Quire.Mpl;

internal partial class EntityWrappers
{
    internal partial class ConstructorReferenceWrapper
    {
        internal readonly ConstructorInfo method;
        internal int parentToken;

        public ConstructorReferenceWrapper(ConstructorInfo method)
        {
            this.method = method;
        }

        public override bool Equals(object obj)
        {
            if (obj is ConstructorReferenceWrapper constructorReferenceWrapper &&
                EqualityComparer<string>.Default.Equals(method.Name, constructorReferenceWrapper.method.Name) &&
                EqualityComparer<string>.Default.Equals(method.ToString(), constructorReferenceWrapper.method.ToString()) &&
                parentToken == constructorReferenceWrapper.parentToken)
            {
                return GetHashCode() == constructorReferenceWrapper.GetHashCode();
            }
            return false;
        }

        public override int GetHashCode()
        {
            string text = (method.DeclaringType != null) ? method.DeclaringType.FullName : string.Empty;
            return HashCode.Combine(method.Name + "@" + text, method.ToString(), parentToken);
        }
    }

    internal partial class FieldReferenceWrapper
    {
        internal readonly FieldInfo field;
        internal int parentToken;

        public FieldReferenceWrapper(FieldInfo field)
        {
            this.field = field;
        }

        public override bool Equals(object obj)
        {
            if (obj is FieldReferenceWrapper fieldReferenceWrapper &&
                EqualityComparer<string>.Default.Equals(field.Name, fieldReferenceWrapper.field.Name) &&
                EqualityComparer<string>.Default.Equals(field.ToString(), fieldReferenceWrapper.field.ToString()) &&
                parentToken == fieldReferenceWrapper.parentToken)
            {
                return GetHashCode() == fieldReferenceWrapper.GetHashCode();
            }
            return false;
        }

        public override int GetHashCode()
        {
            string text = (field.DeclaringType != null) ? field.DeclaringType.FullName : string.Empty;
            return HashCode.Combine(field.Name + "@" + text, field.ToString(), parentToken);
        }
    }
}
