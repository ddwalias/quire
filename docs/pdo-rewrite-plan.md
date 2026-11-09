# PDO: remove the legacy application

Decision: 2026-11-10, QUI-18. The goal is to mirror every Mathcad Prime 12.0.1.0 assembly with exact Release IL. The Quire worksheet application is legacy.

The former worksheet migration plan is superseded. There is no requirement to preserve the custom worksheet UI, `.qwx` format, import flow, or sample worksheet. Do not merge the old sample migration to preserve these paths.

Remove the custom `WorksheetSettings`, `WorksheetDocument`, `RegionPersistentData`, and `TextRegionPersistentData` types, their custom math/plot/solve-block DTOs, and dependent legacy app members. They have no original counterparts and must not receive invented type-map rows.

Keep all real Mathcad persistence types and serializers, including the mapped `PersistentDataObjects`, `PersistentData`, and `Xml.XmlWorksheetData` members. Removing custom DTOs does not implement any remaining original serializer bodies.

Where legacy application code occupied a mapped type, retain the original type and member declarations. Preserve every previously exact implementation; use the Release IL gate to validate changes. Keep the `Quire` project as the library mirror of `MathcadPrime.dll`.

The checkpoint is [qui-18](mirror/work/qui-18.md). Historical lane counts predate this removal.
