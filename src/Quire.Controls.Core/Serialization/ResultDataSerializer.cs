#nullable disable
using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.IO.Packaging;
using System.Xml.Linq;
using Quire.Calculation.Core;
using Quire.IO;

namespace Quire.Controls.Core.Serialization;

public partial class ResultDataSerializer : IPackageSerializable
{
    private readonly IPackageOperationsProvider _packageOpsProvider;

    private const string ResultUri = "/mathcad/result.xml";

    private const string ResultType = "application/vnd.openxmlformats-officedocument.mathprocessingml.mathcad.result+xml";

    private readonly SerializableXmlPackagePart _partSerializer;

    public const string Result10 = "http://schemas.mathsoft.com/result10";

    public ResultDataSerializer(IPackageOperationsProvider packageOpsProvider)
    {
        _packageOpsProvider = packageOpsProvider;
        _partSerializer = new SerializableXmlPackagePart(new ResultElementsSerializer(), SerializableXmlPackagePart.OnWriteValidationFailedDevAssert);
    }

    public void Write(Package package, bool useTempFile = false)
    {
        try
        {
            _partSerializer.Write(_packageOpsProvider.AddPartToPackage(package, "/mathcad/result.xml", "application/vnd.openxmlformats-officedocument.mathprocessingml.mathcad.result+xml"), useTempFile);
        }
        catch (OutOfMemoryException)
        {
            GC.Collect();
            _packageOpsProvider.DeletePartFromPackage(package, "/mathcad/result.xml");
        }
        catch (IsolatedStorageException)
        {
            DebugTools.DevAssert(false, "IsolatedStorageException problem is assumed to be fixed and not supposed not be thrown.");
            _packageOpsProvider.DeletePartFromPackage(package, "/mathcad/result.xml");
        }
        catch (IOException)
        {
            DebugTools.DevAssert(false, "IOException is not supposed to be thrown.");
            _packageOpsProvider.DeletePartFromPackage(package, "/mathcad/result.xml");
        }
    }

    public void Read(Package package)
    {
        PackagePart singlePartByContentType = _packageOpsProvider.GetSinglePartByContentType(package, "application/vnd.openxmlformats-officedocument.mathprocessingml.mathcad.result+xml");
        if (singlePartByContentType != null)
        {
            try
            {
                _partSerializer.Read(singlePartByContentType);
            }
            catch (Exception ex)
            {
                DebugTools.DevAssert(false, ex.Message);
            }
        }
    }

    public static void ClearResults()
    {
        ResultManager.Instance.Clear();
    }

    public static XName GetXNameWithResultNameSpace(string name)
    {
        return XName.Get(name, "http://schemas.mathsoft.com/result10");
    }
}
