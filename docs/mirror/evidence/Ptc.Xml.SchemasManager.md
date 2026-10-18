# Ptc.Xml.SchemasManager

- **Original:** `Ptc.Core.dll` 12.0.1.0, SHA-256 `d60141dbcf757ae64e7b636416c3421412d6f54e2584b318de58d5d6a6f96d56`, MVID `fc0806aa-db73-489f-8ae4-4729770faca6`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Core`; tests: `Quire.Differential.Tests.PtcXmlSchemasManagerMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `App get` | 0x060003e2 | MemberGen: `ConstantSchemaDatas[SchemaNameEnum.App]` | Mechanical; Release IL equals Mathcad's | Verified |
| `Worksheet50 get` | 0x060003e3 | MemberGen: `ConstantSchemaDatas[SchemaNameEnum.Worksheet]` | Mechanical; Release IL equals Mathcad's | Verified |
| `Math50 get` | 0x060003e4 | MemberGen: `ConstantSchemaDatas[SchemaNameEnum.Math]` | Mechanical; Release IL equals Mathcad's | Verified |
| `Presentation10 get` | 0x060003e5 | MemberGen: `ConstantSchemaDatas[SchemaNameEnum.Presentation]` | Mechanical; Release IL equals Mathcad's | Verified |
| `Calculation10 get` | 0x060003e6 | MemberGen: `ConstantSchemaDatas[SchemaNameEnum.Calculation]` | Mechanical; Release IL equals Mathcad's | Verified |
| `Integration10 get` | 0x060003e7 | MemberGen: `ConstantSchemaDatas[SchemaNameEnum.Integration]` | Mechanical; Release IL equals Mathcad's | Verified |
| `Provenance10 get` | 0x060003e8 | MemberGen: `ConstantSchemaDatas[SchemaNameEnum.Provenance]` | Mechanical; Release IL equals Mathcad's | Verified |
| `Units10 get` | 0x060003e9 | MemberGen: `ConstantSchemaDatas[SchemaNameEnum.Units]` | Mechanical; Release IL equals Mathcad's | Verified |
| `Result10 get` | 0x060003ea | MemberGen: `ConstantSchemaDatas[SchemaNameEnum.Result]` | Mechanical; Release IL equals Mathcad's | Verified |
| `SimpleTypes get` | 0x060003eb | MemberGen: `ConstantSchemaDatas[SchemaNameEnum.SimpleTypes]` | Mechanical; Release IL equals Mathcad's | Verified |
| `SchemaData: SchemaName get` | 0x0600116a | MemberGen: `public string SchemaName { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `SchemaData: SchemaName set` | 0x0600116b | MemberGen: `public string SchemaName { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcXmlSchemasManagerMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
