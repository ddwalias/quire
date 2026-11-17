using System;
using System.IO;
using System.IO.Packaging;
using System.Windows.Documents;
using System.Xml;
using Quire.Controls.Core.Serialization;
using Quire.Serialization;
using Quire.Xml;

namespace Quire.Controls.Worksheet;

internal partial class PasteDeS11NProvider : IDeS11NProvider, IBaseS11NProvider
{
    public static PasteDeS11NProvider Instance = new PasteDeS11NProvider();

    private readonly ISerializationHelper _helper = new SerializationHelper();

    private readonly XmlDocument _xmlDocument = new XmlDocument();

    public ITextStyleChangedHandler TextStyleChangedHandler { get; set; }

    public PackagePart PackagePart => null;

    public ISerializationHelper Helper => _helper;

    public XmlNamespaceManager NameSpaceManager => XmlWorksheetData.McdxNameSpaceManager;

    public XmlDocument XmlContentDocument => _xmlDocument;

    public bool UseOverrides => true;

    public string FilePath => "Clipboard";

    public bool SkipRegionsNotAffectingCalculation => false;

    public string UnpackExternalObject(string itemidref)
    {
        throw new NotImplementedException();
    }

    public void UnpackFlowDocument(ref FlowDocument document, string itemidref)
    {
        throw new NotImplementedException();
    }

    public Stream UnpackProtectedPart(string itemIdRef)
    {
        throw new NotImplementedException();
    }
}
