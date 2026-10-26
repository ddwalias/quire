#nullable disable
using System.IO.Packaging;
using System.Xml.Linq;
using Quire.Controls.Core.IORegions;

namespace Quire.Controls.Core.Serialization;

public partial class IntegrationDataSerializer : IPackageSerializable
{
    private readonly IIORegionContainer _documentArea;

    private readonly IEngineeringDocument _document;

    private const string IntegrationUri = "/mathcad/integration.xml";

    private const string IntegrationType = "application/vnd.openxmlformats-officedocument.mathprocessingml.mathcad.integration+xml";

    private readonly SerializableXmlPackagePart _partSerializer;

    public const string Integration10 = "http://schemas.ptc.com/integration10";

    public const string Regions = "regions";

    public const string Region = "region";

    public const string RegionId = "region-id";

    public const string IOTagType = "ioTagType";

    public const string InputAliases = "inputAliases";

    public const string OutputAliases = "outputAliases";

    public const string Alias = "alias";

    public IntegrationDataSerializer(IEngineeringDocument document, IIORegionContainer area)
    {
        _documentArea = area;
        _document = document;
        _partSerializer = new SerializableXmlPackagePart(new IntegrationElementsSerializer(_documentArea, document.DocumentSerializationHelper), SerializableXmlPackagePart.OnWriteValidationFailedDevAssert);
    }

    public void Write(Package package, bool useTempFile = false)
    {
        _partSerializer.Write(_document.PackageOpsProvider.AddPartToPackage(package, "/mathcad/integration.xml", "application/vnd.openxmlformats-officedocument.mathprocessingml.mathcad.integration+xml"), useTempFile);
    }

    public void Read(Package package)
    {
        PackagePart singlePartByContentType = _document.PackageOpsProvider.GetSinglePartByContentType(package, "application/vnd.openxmlformats-officedocument.mathprocessingml.mathcad.integration+xml");
        if (singlePartByContentType != null)
        {
            _partSerializer.Read(singlePartByContentType);
        }
    }

    public static XName GetXNameWithIntegrationNameSpace(string name)
    {
        return XName.Get(name, "http://schemas.ptc.com/integration10");
    }
}
