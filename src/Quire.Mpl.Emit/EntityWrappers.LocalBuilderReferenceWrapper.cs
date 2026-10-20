namespace System.Reflection.Emit.Quire.Mpl;

internal partial class EntityWrappers
{
    internal partial class LocalBuilderReferenceWrapper
    {
        internal readonly LocalBuilder localBuilder;
        internal int parentToken;
        private int hashCode;

        public LocalBuilderReferenceWrapper(LocalBuilder LocalBuilder)
        {
            localBuilder = LocalBuilder;
            hashCode = localBuilder.GetLocalIndex();
        }

        public override bool Equals(object obj)
        {
            if (obj is LocalBuilderReferenceWrapper localBuilderReferenceWrapper && parentToken == localBuilderReferenceWrapper.parentToken)
            {
                return GetHashCode() == localBuilderReferenceWrapper.GetHashCode();
            }
            return false;
        }

        public override int GetHashCode()
        {
            return hashCode;
        }
    }
}
