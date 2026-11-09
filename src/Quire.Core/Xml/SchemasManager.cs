#nullable disable
using System;
using Quire.Mirror;
using Quire.Mirror;

namespace Quire.Xml;

public static partial class SchemasManager
{
    [MirrorVerified("PtcXmlSchemasManagerMechanicalDifferentialTests")]
    public static readonly Dictionary<SchemaNameEnum, SchemaData> ConstantSchemaDatas = new Dictionary<SchemaNameEnum, SchemaData>
    {
        { SchemaNameEnum.App, new SchemaData("Mathcad.app.xsd") },
        { SchemaNameEnum.Worksheet, new SchemaData("Mathcad.worksheet50.xsd") },
        { SchemaNameEnum.Math, new SchemaData("Mathcad.Math50.xsd") },
        { SchemaNameEnum.Presentation, new SchemaData("Mathcad.presentation10.xsd") },
        { SchemaNameEnum.Calculation, new SchemaData("Mathcad.calculation10.xsd") },
        { SchemaNameEnum.Integration, new SchemaData("Mathcad.integration10.xsd") },
        { SchemaNameEnum.Provenance, new SchemaData("Mathcad.provenance10.xsd") },
        { SchemaNameEnum.Units, new SchemaData("Mathcad.units10.xsd") },
        { SchemaNameEnum.Result, new SchemaData("Mathcad.result10.xsd") },
        { SchemaNameEnum.SimpleTypes, new SchemaData("Mathcad.SimpleTypes.xsd") }
    };

    public static List<SchemaData> RuntimeSchemaDatas = new List<SchemaData>();

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

    [MirrorVerified("Release IL")]
    public static SchemaData GetSchemaData(Func<SchemaData, bool> compareFunc)
    {
        SchemaData schemaData = null;
        foreach (SchemaData value in ConstantSchemaDatas.Values)
        {
            if (compareFunc(value))
            {
                schemaData = value;
                break;
            }
        }
        if (schemaData == null)
        {
            foreach (SchemaData runtimeSchemaData in RuntimeSchemaDatas)
            {
                if (compareFunc(runtimeSchemaData))
                {
                    schemaData = runtimeSchemaData;
                    break;
                }
            }
        }
        return schemaData;
    }

    partial class SchemaData
    {
        [MirrorVerified("PtcXmlSchemasManagerMechanicalDifferentialTests")]
        public string SchemaName { get; private set; }
    }

    partial class SchemaData
    {
        [MirrorVerified("Release IL")]
        public SchemaData(string name)
        {
            SchemaName = name;
        }
    }
}
