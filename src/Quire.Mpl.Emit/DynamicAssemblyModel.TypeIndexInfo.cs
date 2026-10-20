#nullable enable
using System.Collections.Generic;

namespace System.Reflection.Emit.Quire.Mpl;

internal partial class DynamicAssemblyModel
{
    internal partial class TypeIndexInfo
    {
        public int typeIndex;
        public int fieldIndex;
        public int methodIndex;
        public List<int> offsets = new List<int>();

        public TypeIndexInfo()
        {
        }
    }
}
