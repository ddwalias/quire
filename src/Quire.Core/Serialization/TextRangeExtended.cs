#nullable disable
using System.Windows.Documents;
using Quire.Mirror;

namespace Quire.Serialization;

public partial class TextRangeExtended
{
    [MirrorVerified("PtcSerializationTextRangeExtendedMechanicalDifferentialTests")]
    public TextRangeExtended(TextPointer tpStart, TextPointer tpEnd) : base(tpStart, tpEnd) { }
}
