namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.Controls.Core.FileLoadResult</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Ptc.Controls.Core.FileLoadResult.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class PtcControlsCoreFileLoadResultMechanicalDifferentialTests
{
    [SkippableFact]
    public void ctor_060002bd() => Mechanical.Compare(typeof(global::Quire.Controls.Core.FileLoadResult), 0x060002bd);

    [SkippableFact]
    public void LoadException_get_060002b8() => Mechanical.Compare(typeof(global::Quire.Controls.Core.FileLoadResult), 0x060002b8);

    [SkippableFact]
    public void LoadException_set_060002b7() => Mechanical.Compare(typeof(global::Quire.Controls.Core.FileLoadResult), 0x060002b7);

    [SkippableFact]
    public void ErrorsOccurredWhenLoadingRegions_get_060002b9() => Mechanical.Compare(typeof(global::Quire.Controls.Core.FileLoadResult), 0x060002b9);

    [SkippableFact]
    public void ErrorsOccurredWhenLoadingRegions_set_060002ba() => Mechanical.Compare(typeof(global::Quire.Controls.Core.FileLoadResult), 0x060002ba);

    [SkippableFact]
    public void DeserializationExceptions_get_060002bb() => Mechanical.Compare(typeof(global::Quire.Controls.Core.FileLoadResult), 0x060002bb);

    [SkippableFact]
    public void DeserializationExceptions_set_060002bc() => Mechanical.Compare(typeof(global::Quire.Controls.Core.FileLoadResult), 0x060002bc);
}
