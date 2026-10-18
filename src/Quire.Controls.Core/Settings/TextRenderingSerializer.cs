#nullable disable
using Quire.Mirror;
using Quire.PersistentData;

namespace Quire.Controls.Core.Settings;

public partial class TextRenderingSerializer
{
    [MirrorVerified("PtcControlsCoreSettingsTextRenderingSerializerMechanicalDifferentialTests")]
    private readonly TextStyleCollectionSerializable _textStyles;

    [MirrorVerified("PtcControlsCoreSettingsTextRenderingSerializerMechanicalDifferentialTests")]
    public TextStyleCollectionSerializable TextStyles => _textStyles;

    [MirrorVerified("PtcControlsCoreSettingsTextRenderingSerializerMechanicalDifferentialTests")]
    public IWorksheetPersistentData WorksheetData { get; }

    [MirrorVerified("PtcControlsCoreSettingsTextRenderingSerializerMechanicalDifferentialTests")]
    private static XmlLanguageConverterWithDefaultValue XmlProofingLanguageConverter { get; }
}
