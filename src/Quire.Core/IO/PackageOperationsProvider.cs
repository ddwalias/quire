#nullable disable
using System.IO;
using System.IO.Packaging;
using Quire.Mirror;

namespace Quire.IO;

public partial class PackageOperationsProvider
{
    [MirrorVerified("PtcIOPackageOperationsProviderMechanicalDifferentialTests")]
    public Package GetPackageForWrite(string packageName) => Package.Open(packageName, (FileMode)2);

    [MirrorVerified("PtcIOPackageOperationsProviderMechanicalDifferentialTests")]
    public Package GetPackageForWrite(FileStream stream) => Package.Open(stream, (FileMode)2);

    [MirrorVerified("PtcIOPackageOperationsProviderMechanicalDifferentialTests")]
    public Package GetPackageForRead(string packageName) => Package.Open(packageName, (FileMode)3, (FileAccess)1, (FileShare)1);

    [MirrorVerified("Release IL")]
    public void DeletePartFromPackage(Package package, string uriString)
    {
        Uri partUri = new Uri(uriString, UriKind.Relative);
        package.DeletePart(partUri);
    }

    [MirrorVerified("Release IL")]
    private static PackagePart GetPartIfExists(Package package, Uri partName)
    {
        try
        {
            if (package.PartExists(partName))
            {
                return package.GetPart(partName);
            }
        }
        catch (Exception ex)
        {
            DebugTools.DevAssert(false, ex.Message);
        }
        return null;
    }

    [MirrorVerified("Release IL")]
    public PackagePart GetSinglePartByRelationshipType(PackagePart sourcePart, string relationshipType)
    {
        Uri uriForFirstRelashionshipInCollection = GetUriForFirstRelashionshipInCollection(sourcePart.GetRelationshipsByType(relationshipType));
        if (uriForFirstRelashionshipInCollection == null)
        {
            throw new Exception(string.Format("Part doesn't contain relationship of type {0}", relationshipType));
        }
        if (!sourcePart.Package.PartExists(uriForFirstRelashionshipInCollection))
        {
            throw new Exception(string.Format("Part doesn't contain part by URI {0}", uriForFirstRelashionshipInCollection.OriginalString));
        }
        return sourcePart.Package.GetPart(uriForFirstRelashionshipInCollection);
    }

    [MirrorVerified("Release IL")]
    private static Uri GetUriForFirstRelashionshipInCollection(IEnumerable<PackageRelationship> relshipCollection)
    {
        IEnumerator<PackageRelationship> enumerator = relshipCollection.GetEnumerator();
        enumerator.MoveNext();
        PackageRelationship relationship = enumerator.Current;
        if (relationship == null)
        {
            return null;
        }
        return relationship.TargetUri;
    }

    [MirrorVerified("Release IL")]
    private bool IsTypeToCompress(string contentType)
    {
        if (!_compressedTypes.Contains(contentType))
        {
            return contentType.StartsWith("image/");
        }
        return true;
    }

    [MirrorVerified("Release IL")]
    public PackagePart GetSinglePartByRelationshipType(Package package, string relationshipType)
    {
        Uri uriForFirstRelashionshipInCollection = GetUriForFirstRelashionshipInCollection(package.GetRelationshipsByType(relationshipType));
        if (uriForFirstRelashionshipInCollection == null)
        {
            throw new Exception(string.Format("Package doesn't contain relationship of type {0}", relationshipType));
        }
        if (!package.PartExists(uriForFirstRelashionshipInCollection))
        {
            throw new Exception(string.Format("Package doesn't contain part by URI {0}", uriForFirstRelashionshipInCollection.OriginalString));
        }
        return package.GetPart(uriForFirstRelashionshipInCollection);
    }

    [MirrorVerified("PtcIOPackageOperationsProviderMechanicalDifferentialTests")]
    private static readonly Uri _worksheet;

    [MirrorVerified("PtcIOPackageOperationsProviderMechanicalDifferentialTests")]
    public PackagePart GetMainPartFromPackage(Package package) => package.GetPart(_worksheet);

    [MirrorVerified("PtcIOPackageOperationsProviderMechanicalDifferentialTests")]
    private static readonly Uri _header;

    [MirrorVerified("PtcIOPackageOperationsProviderMechanicalDifferentialTests")]
    public PackagePart GetHeaderPartFromPackage(Package package) => GetPartIfExists(package, _header);

    [MirrorVerified("PtcIOPackageOperationsProviderMechanicalDifferentialTests")]
    private static readonly Uri _footer;

    [MirrorVerified("PtcIOPackageOperationsProviderMechanicalDifferentialTests")]
    public PackagePart GetFooterPartFromPackage(Package package) => GetPartIfExists(package, _footer);

    [MirrorVerified("PtcIOPackageOperationsProviderMechanicalDifferentialTests")]
    private static readonly Uri _headersContainer;

    [MirrorVerified("PtcIOPackageOperationsProviderMechanicalDifferentialTests")]
    public PackagePart GetHeadersContainerPartFromPackage(Package package) => GetPartIfExists(package, _headersContainer);

    [MirrorVerified("PtcIOPackageOperationsProviderMechanicalDifferentialTests")]
    private static readonly Uri _footersContainer;

    [MirrorVerified("PtcIOPackageOperationsProviderMechanicalDifferentialTests")]
    public PackagePart GetFootersContainerPartFromPackage(Package package) => GetPartIfExists(package, _footersContainer);

    [MirrorVerified("PtcIOPackageOperationsProviderMechanicalDifferentialTests")]
    public PackagePart GetCoreMetadataPartFromPackage(Package package) => GetSinglePartByRelationshipType(package, "http://schemas.openxmlformats.org/package/2006/relationships/metadata/core-properties");

    [MirrorVerified("PtcIOPackageOperationsProviderMechanicalDifferentialTests")]
    public PackagePart AddMainPartToPackage(Package package) => AddWorksheetPartToPackage(package, _worksheet);

    [MirrorVerified("PtcIOPackageOperationsProviderMechanicalDifferentialTests")]
    public PackagePart AddHeaderPartToPackage(Package package) => AddWorksheetPartToPackage(package, _header);

    [MirrorVerified("PtcIOPackageOperationsProviderMechanicalDifferentialTests")]
    public PackagePart AddFooterPartToPackage(Package package) => AddWorksheetPartToPackage(package, _footer);

    [MirrorVerified("PtcIOPackageOperationsProviderMechanicalDifferentialTests")]
    public PackagePart AddHeadersContainerPartToPackage(Package package) => AddWorksheetPartToPackage(package, _headersContainer);

    [MirrorVerified("PtcIOPackageOperationsProviderMechanicalDifferentialTests")]
    public PackagePart AddFootersContainerPartToPackage(Package package) => AddWorksheetPartToPackage(package, _footersContainer);

    [MirrorVerified("PtcIOPackageOperationsProviderMechanicalDifferentialTests")]
    public static FileStream OpenFileStreamForReadWithFullShare(string path) => File.Open(path, (FileMode)3, (FileAccess)1, (FileShare)3);

    [MirrorVerified("PtcIOPackageOperationsProviderMechanicalDifferentialTests")]
    public CompressionOption CompressionLevel { get; set; }
}
