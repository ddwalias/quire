#nullable disable
using System;
using System.IO.Packaging;

namespace Quire.Controls.Core;

public partial interface IPackagePartReferenceManager
{
    string FullFilePath { get; }
    DateTime LastSavedDate { get; }
    event Func<string, PackagePart> GetPackagePartByRefId;
    event Func<IPackagePartReferenceManager, string> GetRefIdForReferencedFile;
    PackagePart WriteToPackage(Package package);
}
