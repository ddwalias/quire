using System;
using System.IO.Packaging;
using Quire.Controls.Include;
using Quire.PersistentDataObjects;

namespace Quire.Controls.Worksheet;

public partial class IncludeRegionPersistentData : RegionPersistentData
{
    private readonly IncludeWorksheetControl _include;

    public IncludeRegionPersistentData(IncludeWorksheetControl include)
        : base(include)
    {
        _include = include;
    }

    private PackagePart HandleGetPackagePartByRefId(string refId)
    {
        try
        {
            Uri targetUri = PackagePart.GetRelationship(refId).TargetUri;
            return PackagePart.Package.GetPart(targetUri);
        }
        catch (Exception)
        {
            return null;
        }
    }

    protected override void Deserialize()
    {
        _include.GetPackagePartByRefId += HandleGetPackagePartByRefId;
        HideFromCalculationContext = ((includeWorksheetType)RegionData.Item).disablecalc;
        base.Deserialize();
        _include.GetPackagePartByRefId -= HandleGetPackagePartByRefId;
    }

    public override void DeserializeEpilog()
    {
        _include.DeserializeEpilog();
    }
}
