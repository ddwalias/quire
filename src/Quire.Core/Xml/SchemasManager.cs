#nullable disable
using Quire.Mirror;

namespace Quire.Xml;

public static partial class SchemasManager
{
    [MirrorVerified("PtcXmlSchemasManagerMechanicalDifferentialTests")]
    public static readonly Dictionary<SchemaNameEnum, SchemaData> ConstantSchemaDatas;

    [MirrorVerified("PtcXmlSchemasManagerMechanicalDifferentialTests")]
    public static SchemasManager.SchemaData App => ConstantSchemaDatas[SchemaNameEnum.App];

    [MirrorVerified("PtcXmlSchemasManagerMechanicalDifferentialTests")]
    public static SchemaData Worksheet50 => ConstantSchemaDatas[SchemaNameEnum.Worksheet];

    [MirrorVerified("PtcXmlSchemasManagerMechanicalDifferentialTests")]
    public static SchemaData Math50 => ConstantSchemaDatas[SchemaNameEnum.Math];

    [MirrorVerified("PtcXmlSchemasManagerMechanicalDifferentialTests")]
    public static SchemaData Presentation10 => ConstantSchemaDatas[SchemaNameEnum.Presentation];

    [MirrorVerified("PtcXmlSchemasManagerMechanicalDifferentialTests")]
    public static SchemaData Calculation10 => ConstantSchemaDatas[SchemaNameEnum.Calculation];

    [MirrorVerified("PtcXmlSchemasManagerMechanicalDifferentialTests")]
    public static SchemaData Integration10 => ConstantSchemaDatas[SchemaNameEnum.Integration];

    [MirrorVerified("PtcXmlSchemasManagerMechanicalDifferentialTests")]
    public static SchemaData Provenance10 => ConstantSchemaDatas[SchemaNameEnum.Provenance];

    [MirrorVerified("PtcXmlSchemasManagerMechanicalDifferentialTests")]
    public static SchemaData Units10 => ConstantSchemaDatas[SchemaNameEnum.Units];

    [MirrorVerified("PtcXmlSchemasManagerMechanicalDifferentialTests")]
    public static SchemaData Result10 => ConstantSchemaDatas[SchemaNameEnum.Result];

    [MirrorVerified("PtcXmlSchemasManagerMechanicalDifferentialTests")]
    public static SchemaData SimpleTypes => ConstantSchemaDatas[SchemaNameEnum.SimpleTypes];

    partial class SchemaData
    {
        [MirrorVerified("PtcXmlSchemasManagerMechanicalDifferentialTests")]
        public string SchemaName { get; private set; }
    }
}
