using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

namespace System.Reflection.Emit.Quire.Mpl;

internal partial class TryCatchBlockInfo
{
    public LabelHandle labelTryStart { get; set; }
    public LabelHandle labelTryEnd { get; set; }
    public LabelHandle labelCatchStart { get; set; }
    public LabelHandle labelCatchEnd { get; set; }
    public LabelHandle endExceptionBlockLabel { get; set; }
    public EntityHandle exceptionToken { get; set; }
}
