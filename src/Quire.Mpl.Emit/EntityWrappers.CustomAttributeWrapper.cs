using System.Reflection;

namespace System.Reflection.Emit.Quire.Mpl;

internal partial class EntityWrappers
{
    internal partial class CustomAttributeWrapper
    {
        internal ConstructorInfo constructorInfo;
        internal byte[] binaryAttribute;
        internal int conToken;

        public CustomAttributeWrapper(ConstructorInfo constructorInfo, byte[] binaryAttribute)
        {
            this.constructorInfo = constructorInfo;
            this.binaryAttribute = binaryAttribute;
        }
    }
}
