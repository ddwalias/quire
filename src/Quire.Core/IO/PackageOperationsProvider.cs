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
    public PackagePart GetSinglePartByContentType(Package package, string contentType)
    {
        return package.GetParts().FirstOrDefault((PackagePart part) => part.ContentType == contentType);
    }

    [MirrorVerified("Release IL")]
    public List<PackagePart> GetPartsByContentType(Package package, string contentType)
    {
        return package.GetParts().Where((PackagePart part) => part.ContentType == contentType).ToList();
    }

    [MirrorVerified("Release IL")]
    public int CountPartsByContentType(Package package, string contentType)
    {
        return package.GetParts().Count((PackagePart part) => part.ContentType == contentType);
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
    private PackagePart AddWorksheetPartToPackage(Package package, Uri uri)
    {
        PackagePart result = CreatePackagePart(package, uri, MainPartContentType);
        package.CreateRelationship(uri, TargetMode.Internal, MainPartRelashinshipType);
        return result;
    }

    [MirrorVerified("Release IL")]
    private PackagePart CreatePackagePart(Package package, Uri uri, string contentType)
    {
        return package.CreatePart(uri, contentType, IsTypeToCompress(contentType) ? CompressionLevel : CompressionOption.NotCompressed);
    }

    [MirrorVerified("Release IL")]
    public PackagePart AddCoreMetadataPartToPackage(Package package)
    {
        Uri uri = new Uri("/docProps/core.xml", UriKind.Relative);
        PackagePart result = CreatePackagePart(package, uri, CoreMetadataContentType);
        package.CreateRelationship(uri, TargetMode.Internal, CoreMetadataRelashinshipType);
        return result;
    }

    [MirrorVerified("Release IL")]
    public PackagePart AddPartToPackage(Package package, string uriString, string contentType)
    {
        Uri uri = new Uri(uriString, UriKind.Relative);
        return CreatePackagePart(package, uri, contentType);
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
    public static string PackPartAndGetId(PackagePart sourcePackagePart, Func<Pair<string, Uri>> getExistingPartRelationshipId, Action<string, PackagePart> cacheRelationshipIdForNewPart, Func<PackagePart> writeNewPartToPackage, string partRelationshipUri)
    {
        Pair<string, Uri> pair = getExistingPartRelationshipId();
        string first = pair.First;
        if (string.IsNullOrEmpty(first))
        {
            PackagePart packagePart = writeNewPartToPackage();
            first = sourcePackagePart.CreateRelationship(packagePart.Uri, TargetMode.Internal, partRelationshipUri).Id;
            cacheRelationshipIdForNewPart(first, packagePart);
        }
        else if (!sourcePackagePart.RelationshipExists(first))
        {
            sourcePackagePart.CreateRelationship(pair.Second, TargetMode.Internal, "http://schemas.openxmlformats.org/officeDocument/2006/relationships/image", first);
        }
        return first;
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
    public static Stream GetSourceStream(PackagePart packagePart, string itemIdRef)
    {
        if (itemIdRef != null)
        {
            return packagePart.Package.GetPart(packagePart.GetRelationship(itemIdRef).TargetUri).GetStream((FileMode)3, (FileAccess)1);
        }
        return null;
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

    [MirrorVerified("Release IL")]
    public static PackagePart CreateNewImagePart(PackagePart referencingPackagePart, global::System.Windows.Media.Imaging.BitmapSource image, string imageExtension, byte[] originalStreamHash, string uniqueImageName, Uri imageSourceUri)
    {
        string contentType = "image/" + imageExtension;
        string uri = global::System.String.Format("/mathcad/media/{0}.{1}", uniqueImageName, imageExtension);
        PackagePart packagePart = Instance.AddPartToPackage(referencingPackagePart.Package, uri, contentType);
        global::System.Action<Stream> action = (Stream targetStream) => global::Quire.Wpf.ImageUtils.CopyImageToStream(imageExtension, imageSourceUri, targetStream, image, originalStreamHash);
        using (Stream stream = packagePart.GetStream(FileMode.Create))
        {
            action(stream);
        }
        return packagePart;
    }

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

