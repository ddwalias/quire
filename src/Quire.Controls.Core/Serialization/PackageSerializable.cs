#nullable disable
using System.IO.Packaging;
using Quire.Controls.Core.Settings;
using Quire.Core.Serialization;
using Quire.IO;
using Quire.Serialization;

namespace Quire.Controls.Core.Serialization;

public partial class PackageSerializable : IPackageSerializable
{
    private readonly string _uri;

    private readonly string _type;

    private readonly IPackagePartSerializable _packagePartSerializable;

    private readonly IPackageOperationsProvider _provider;

    private const string ApplicationPropertiesUri = "/docProps/app.xml";

    private const string ApplicationPropertiesType = "application/mathcad.extended-properties+xml";

    private const string PresentationUri = "/mathcad/settings/presentation.xml";

    private const string PresentationType = "application/vnd.openxmlformats-officedocument.mathprocessingml.mathcad.settings.presentation+xml";

    private const string CalculationUri = "/mathcad/settings/calculation.xml";

    private const string CalculationType = "application/vnd.openxmlformats-officedocument.mathprocessingml.mathcad.settings.calculation+xml";

    private PackageSerializable(string uri, string type, IXmlAndXElementSerializable xmlSerializable, IPackageOperationsProvider packageOperationsProvider)
    {
        _type = type;
        _uri = uri;
        _packagePartSerializable = new SerializableXmlPackagePart(xmlSerializable, SerializableXmlPackagePart.OnWriteValidationFailedDevAssert);
        _provider = packageOperationsProvider;
    }

    public void Write(Package package, bool useTempFile = false)
    {
        _packagePartSerializable.Write(_provider.AddPartToPackage(package, _uri, _type), useTempFile);
    }

    public void Read(Package package)
    {
        PackagePart singlePartByContentType = _provider.GetSinglePartByContentType(package, _type);
        if (singlePartByContentType != null)
        {
            _packagePartSerializable.Read(singlePartByContentType);
        }
    }

    public static IPackageSerializable GetApplicationPropertiesSerializer(IPackageOperationsProvider provider, ISerializationHelper helper)
    {
        XmlToXElementSerializableAdapter xmlSerializable = new XmlToXElementSerializableAdapter(new ApplicationPropertiesXmlSerializable(helper));
        return new PackageSerializable("/docProps/app.xml", "application/mathcad.extended-properties+xml", xmlSerializable, provider);
    }

    public static PackageSerializable GetCalculationSettingsSerializable(IEngineeringDocument document)
    {
        return new PackageSerializable("/mathcad/settings/calculation.xml", "application/vnd.openxmlformats-officedocument.mathprocessingml.mathcad.settings.calculation+xml", new CalculationSettings(document, "http://schemas.ptc.com/mathcad/settings/calculation10"), document.PackageOpsProvider);
    }

    public static PackageSerializable GetPresentationSettingsSerializable(IEngineeringDocument document)
    {
        return new PackageSerializable("/mathcad/settings/presentation.xml", "application/vnd.openxmlformats-officedocument.mathprocessingml.mathcad.settings.presentation+xml", new McdxPresentationSettings(document), document.PackageOpsProvider);
    }
}
