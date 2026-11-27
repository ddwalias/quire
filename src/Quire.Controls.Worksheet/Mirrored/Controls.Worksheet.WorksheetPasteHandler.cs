using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Xml;
using Quire.Controls.Core;
using Quire.Controls.Text;
using Quire.Controls.Whiteboard;
using Quire.Core;
using Quire.EquationEditor.Tree;
using Quire.PersistentData;
using Quire.PersistentDataObjects;
using Quire.Serialization;
using Quire.Services;
using Quire.Undo;
using Quire.Wpf;
using Quire.Xml;

namespace Quire.Controls.Worksheet;

public partial class WorksheetPasteHandler
{
    private readonly ResourceDictionary _resourceDictionary;
    private readonly ITextStyleChangedHandler _textStyleChangedHandler;
    private static byte[] _clipboardPrimeObjectHash;
    private static IWhiteboardItemInText _clipboardPrimeObject;

    public WorksheetPasteHandler(ResourceDictionary resourceDictionary, ITextStyleChangedHandler textStyleChangedHandler)
    {
        _resourceDictionary = resourceDictionary;
        _textStyleChangedHandler = textStyleChangedHandler;
    }

    public ResourceDictionary WorksheetMessages => _resourceDictionary;

    private static IWorksheetItemManager ItemManager => WorksheetViewModel.ItemManager;

    private static IControlFactory ControlCreator => WorksheetViewModel.ControlCreator;

    public bool ClipboardContainsRecognizedFormat
    {
        get
        {
            try
            {
                IClipboard current = WpfClipboard.Current;
                return current.HasPrimeObject() || current.HasSExpression() || current.HasXMLSpreadsheet() || current.HasText() || current.HasImage();
            }
            catch
            {
                return false;
            }
        }
    }

    private IWhiteboardItemInText SingleEmbeddableItemForTextFromClipboard
    {
        get
        {
            IClipboard current = WpfClipboard.Current;
            if (current.HasPrimeObject())
            {
                byte[] hash = AppServiceRegistry.GetService<IHashAlgorithmProvider>().ComputeHash(current.GetPrimeXamlStream());
                if (_clipboardPrimeObjectHash == null || !((ReadOnlySpan<byte>)_clipboardPrimeObjectHash).SequenceEqual(hash))
                {
                    _clipboardPrimeObjectHash = hash;
                    if (current.GetPrimeObject() is not WhiteboardItemsCollection items || items.Count != 1)
                    {
                        _clipboardPrimeObject = null;
                    }
                    else
                    {
                        _clipboardPrimeObject = items[0] as IWhiteboardItemInText;
                        if (_clipboardPrimeObject is DependencyObject dependencyObject)
                        {
                            AttachedProperties.ClearPreviousLocation(dependencyObject);
                        }
                    }
                }
            }
            else
            {
                _clipboardPrimeObjectHash = null;
                _clipboardPrimeObject = null;
                if (current.HasSExpression())
                {
                    _clipboardPrimeObject = EquationFromSExpression(current.GetSExpression());
                }
            }
            return _clipboardPrimeObject;
        }
    }

    public bool ClipboardContainsSingleEmbeddableItemForText => WpfClipboard.Current.HasSExpression();

    public IDictionary<IWhiteboardItem, Vector> ProcessClipboardContents(XmlLanguage proofingLanguage, TextFormat currentTextFormat)
    {
        IClipboard clipboard = WpfClipboard.Current;
        List<Exception> exceptions = new List<Exception>();
        IDictionary<IWhiteboardItem, Vector> result = null;
        if (clipboard.HasPrimeObject())
        {
            try
            {
                result = ProcessXamlObject(clipboard.GetPrimeObject(), _textStyleChangedHandler);
                if (result == null)
                {
                    exceptions.Add(new Exception(ErrorResourceKeyToMessage("WorksheetMessages.PrimeObject")));
                }
            }
            catch (Exception innerException)
            {
                exceptions.Add(new Exception(ErrorResourceKeyToMessage("WorksheetMessages.PrimeObject"), innerException));
            }
        }
        if (result == null && clipboard.HasSExpression())
        {
            try
            {
                result = ProcessSExpression(clipboard.GetSExpression());
                if (result == null)
                {
                    exceptions.Add(new Exception(ErrorResourceKeyToMessage("WorksheetMessages.SExpression")));
                }
            }
            catch (Exception innerException2)
            {
                exceptions.Add(new Exception(ErrorResourceKeyToMessage("WorksheetMessages.SExpression"), innerException2));
            }
        }
        if (result == null && clipboard.HasXMLSpreadsheet())
        {
            try
            {
                XmlDocument xmlSpreadsheet = clipboard.GetXMLSpreadsheet();
                if (xmlSpreadsheet != null)
                {
                    result = XmlSpreadsheetPasteHandler.ProcessXmlSpreadsheet(xmlSpreadsheet);
                    if (result == null)
                    {
                        exceptions.Add(new Exception(ErrorResourceKeyToMessage("WorksheetMessages.XMLSpreadsheet")));
                    }
                }
            }
            catch (Exception innerException3)
            {
                exceptions.Add(new Exception(ErrorResourceKeyToMessage("WorksheetMessages.XMLSpreadsheet"), innerException3));
            }
        }
        if (result == null && clipboard.HasText())
        {
            try
            {
                string formattedText = clipboard.GetFormattedText(out string dataFormat, out bool isPlainText);
                string plainText = isPlainText ? formattedText : clipboard.GetText();
                result = ProcessPrimeXml(plainText, _textStyleChangedHandler);
                if (result == null)
                {
                    string textDataFormat = DataFormats.Text;
                    bool hasMultipleLines = plainText.Trim().IndexOf('\n') != -1;
                    try
                    {
                        result = ProcessText(currentTextFormat, formattedText, isPlainText ? textDataFormat : dataFormat, proofingLanguage, hasMultipleLines, _textStyleChangedHandler);
                    }
                    catch
                    {
                        if (isPlainText)
                        {
                            throw;
                        }
                        result = ProcessText(currentTextFormat, plainText, textDataFormat, proofingLanguage, hasMultipleLines, _textStyleChangedHandler);
                    }
                    if (result == null)
                    {
                        exceptions.Add(new Exception(ErrorResourceKeyToMessage("WorksheetMessages.Text")));
                    }
                }
            }
            catch (Exception innerException4)
            {
                exceptions.Add(new Exception(ErrorResourceKeyToMessage("WorksheetMessages.Text"), innerException4));
            }
        }
        if (result == null && clipboard.HasImage())
        {
            try
            {
                result = ProcessImage(clipboard.GetImage());
                if (result == null)
                {
                    exceptions.Add(new Exception(ErrorResourceKeyToMessage("WorksheetMessages.Image")));
                }
            }
            catch (Exception innerException5)
            {
                exceptions.Add(new Exception(ErrorResourceKeyToMessage("WorksheetMessages.Image"), innerException5));
            }
        }
        foreach (Exception exception in exceptions)
        {
            DebugTools.FailAssertion(exception.Message);
        }
        if (result == null)
        {
            throw exceptions.IsNotEmpty()
                ? exceptions.First()
                : new Exception(ErrorResourceKeyToMessage("WorksheetMessages.General"));
        }
        return result;
    }

    private string ErrorResourceKeyToMessage(string errorResourceKey)
    {
        if (!_resourceDictionary.Contains(errorResourceKey))
        {
            return errorResourceKey;
        }
        return _resourceDictionary[errorResourceKey].ToString();
    }

    public IWhiteboardItemInText ProcessSingleEmbeddableItemForTextInClipboard()
    {
        IWhiteboardItemInText item = SingleEmbeddableItemForTextFromClipboard;
        if (DebugTools.AssertNotNull(item, null).Failed)
        {
            return null;
        }
        ItemManager.SetItemIsInPasting(item, true);
        try
        {
            return item.Clone() as IWhiteboardItemInText;
        }
        finally
        {
            ItemManager.ClearItemIsInPasting(item);
        }
    }

    [global::Quire.Mirror.MirrorStub]
    [global::Quire.Mirror.MirrorBlocked("closure ordinal anchor; Modern Roslyn StackOptimizer elides single-use prevLocation local in GetPastedItemLocation")]
    private static IDictionary<IWhiteboardItem, Vector> ProcessXamlObject(object xamlObject, ITextStyleChangedHandler textStyleChangedHandler) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private static string ConvertToValidSExpressionIfANegativeNumber(string value)
    {
        if (double.TryParse(value, out double number) && number < 0.0 && value.StartsWith("-"))
        {
            return "(" + DefaultDescriptorTable.Default["NEGATION"].OperatorSExpName + " " + value.Substring(1) + ")";
        }
        return value;
    }

    private static IDictionary<IWhiteboardItem, Vector> ProcessImage(ImageSource imageSource)
    {
        IImageControl imageControl = ControlCreator.CreateImageControl(false);
        imageControl.Source = imageSource;
        return new Dictionary<IWhiteboardItem, Vector>
        {
            {
                imageControl,
                new Vector(0.0, 0.0)
            }
        };
    }

    private static IDictionary<IWhiteboardItem, Vector> ProcessPrimeXml(string primeXml, ITextStyleChangedHandler textStyleChangedHandler = null)
    {
        try
        {
            XmlDocument doc = PasteDeS11NProvider.Instance.XmlContentDocument;
            SpiritResolver.InitializeDocument(doc, SchemasManager.Worksheet50);
            doc.LoadXml(primeXml);
            XmlElement docElement = doc.DocumentElement;
            if (docElement == null)
            {
                return null;
            }
            // Local variable in try scope preserves Mathcad slot 1 before synthesized try-return local.
            string localName = docElement.LocalName;
            switch (localName)
            {
                case "worksheet":
                {
                    worksheetRegionCollectionSerializer serializer = new worksheetRegionCollectionSerializer();
                    serializer.Deserialize(docElement, PasteDeS11NProvider.Instance.UseOverrides);
                    return ProcessRegionPersistentDataList(DeserializeIRegions(serializer.Regions));
                }
                case "region":
                {
                    IRegionPersistentData regionData = DeserializeIRegion(worksheetRegionType.Deserialize(docElement, PasteDeS11NProvider.Instance.UseOverrides));
                    if (regionData == null)
                    {
                        return null;
                    }
                    IWhiteboardItem item = (IWhiteboardItem)regionData.Accept(InitializedRegionFactory.Instance);
                    if (item == null)
                    {
                        return null;
                    }
                    return new Dictionary<IWhiteboardItem, Vector>
                    {
                        {
                            item,
                            new Vector(0.0, 0.0)
                        }
                    };
                }
            }
        }
        catch (XmlException)
        {
            return null;
        }
        return null;
    }

    private static IEnumerable<IRegionPersistentData> DeserializeIRegions(IEnumerable<IRegionType> regionTypes)
    {
        return regionTypes.Select(DeserializeIRegion).Where((IRegionPersistentData data) => data != null).ToList();
    }

    private static IRegionPersistentData DeserializeIRegion(IRegionType regionType)
    {
        try
        {
            return RegionFactory.Instance.ConvertFromIRegionType(PasteDeS11NProvider.Instance, null, regionType, null);
        }
        catch (Exception)
        {
            return null;
        }
    }

    private static IDictionary<IWhiteboardItem, Vector> ProcessRegionPersistentDataList(IEnumerable<IRegionPersistentData> dataList)
    {
        if (dataList.IsNullOrEmpty())
        {
            return null;
        }
        Point pastedItemsTopLeft = new Point(double.MaxValue, double.MaxValue);
        foreach (IRegionPersistentData data in dataList)
        {
            Point location = data.Location;
            if (location.X < pastedItemsTopLeft.X)
            {
                pastedItemsTopLeft.X = location.X;
            }
            if (location.Y < pastedItemsTopLeft.Y)
            {
                pastedItemsTopLeft.Y = location.Y;
            }
        }
        Dictionary<IWhiteboardItem, Vector> addItemsWithOffset = new Dictionary<IWhiteboardItem, Vector>();
        dataList.Each(delegate (IRegionPersistentData data)
        {
            addItemsWithOffset.Add((IWhiteboardItem)data.Accept(InitializedRegionFactory.Instance), data.Location - pastedItemsTopLeft);
        });
        return addItemsWithOffset;
    }

    private static IEquationControl EquationFromSExpression(string sexp)
    {
        IEquationControl equationControl = ControlCreator.CreateEquationControl(null);
        try
        {
            using (new CommandScope(new CompoundCommand()))
            {
                equationControl.EquationSExpression = ConvertToValidSExpressionIfANegativeNumber(sexp);
            }
            equationControl.NavigateRegionEnd();
            ItemManager.ActivateItem(equationControl);
        }
        catch
        {
            return null;
        }
        return equationControl;
    }

    private static IDictionary<IWhiteboardItem, Vector> ProcessSExpression(string sexp)
    {
        IEquationControl equationControl = EquationFromSExpression(sexp);
        if (equationControl == null)
        {
            return null;
        }
        return new Dictionary<IWhiteboardItem, Vector>
        {
            {
                equationControl,
                new Vector(0.0, 0.0)
            }
        };
    }

    private static IDictionary<IWhiteboardItem, Vector> ProcessText(TextFormat currentTextFormat, string formattedText, string dataFormat, XmlLanguage proofingLanguage, bool hasMultipleLines, ITextStyleChangedHandler textStyleChangedHandler = null)
    {
        if (hasMultipleLines)
        {
            IParagraphControl paragraphControl = ControlCreator.CreateTextBlockControl(currentTextFormat, proofingLanguage, formattedText, dataFormat, textStyleChangedHandler);
            paragraphControl.NavigateIntoItem(Direction.Left);
            ItemManager.ActivateItem(paragraphControl);
            return new Dictionary<IWhiteboardItem, Vector>
            {
                {
                    paragraphControl,
                    new Vector(0.0, 0.0)
                }
            };
        }
        IRichTextBase richTextBase = ControlCreator.CreateTextboxControl(currentTextFormat, proofingLanguage, formattedText, dataFormat, false, textStyleChangedHandler);
        richTextBase.NavigateIntoItem(Direction.Left);
        ItemManager.ActivateItem(richTextBase);
        return new Dictionary<IWhiteboardItem, Vector>
        {
            {
                richTextBase,
                new Vector(0.0, 0.0)
            }
        };
    }
}
