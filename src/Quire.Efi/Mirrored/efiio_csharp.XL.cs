#nullable disable
#pragma warning disable

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.IsolatedStorage;
using System.IO.Packaging;
using System.Reflection;
using System.Runtime.Loader;
using System.Text.RegularExpressions;
using System.Xml;
using Quire.Efi;
using Quire.Mcd;
using Quire.Mpl.Run;

namespace Quire.efiio_csharp;

public partial class XL
{
    public partial class DocumentContext
    {
        public XmlNamespaceManager nsManager;

        public PackagePart documentPart;

        public XmlDocument sheetDoc;

        public Uri documentUri;

        public Package xlPackage;

        public XmlNameTable nt;

        public XmlDocument stringDoc;

        public PackagePart stringPart;

        public XmlNode stringTableNode;

        public Dictionary<int, string> sharedStrings_IntToString;

        public Dictionary<string, int> sharedStrings_StringToInt;

        public int sharedStringInitialTotalCount;

        public int sharedStringRunningTotalCount;

        public PackagePart sheetPart;

        public DocumentContext(Package package)
        {
            xlPackage = package;
        }
    }

    public partial class IsolatedLoadContext : AssemblyLoadContext
    {
        public IsolatedLoadContext()
            : base(isCollectible: true)
        {
        }

        protected override Assembly Load(AssemblyName assemblyName)
        {
            return null;
        }
    }

    public static bool s_UseSharedStrings = false;

    private static bool s_WriteExcelCreateDirectories = false;

    private static bool s_WriteExcelOverwritesFiles = false;

    private static bool s_WriteExcelEnabled = true;

    private static bool s_WriteExcelKeepOld = false;

    public static bool WriteExcelCreateDirectories
    {
        get
        {
            return s_WriteExcelCreateDirectories;
        }
        set
        {
            s_WriteExcelCreateDirectories = value;
        }
    }

    public static bool WriteExcelOverwritesFiles
    {
        get
        {
            return s_WriteExcelOverwritesFiles;
        }
        set
        {
            s_WriteExcelOverwritesFiles = value;
        }
    }

    public static bool WriteExcelEnabled
    {
        get
        {
            return s_WriteExcelEnabled;
        }
        set
        {
            s_WriteExcelEnabled = value;
        }
    }

    public static bool WriteExcelKeepOld
    {
        get
        {
            return s_WriteExcelKeepOld;
        }
        set
        {
            s_WriteExcelKeepOld = value;
        }
    }

    public static void LoadStringDocIfNeeded(DocumentContext context)
    {
        foreach (PackageRelationship item in context.documentPart.GetRelationshipsByType("http://schemas.openxmlformats.org/officeDocument/2006/relationships/sharedStrings"))
        {
            if (context.stringDoc == null)
            {
                Uri partUri = PackUriHelper.ResolvePartUri(context.documentUri, item.TargetUri);
                PackagePart part = context.xlPackage.GetPart(partUri);
                context.stringDoc = new XmlDocument(context.nt);
                context.stringPart = part;
                context.sharedStrings_IntToString = new Dictionary<int, string>();
                context.sharedStrings_StringToInt = new Dictionary<string, int>();
                if (part != null)
                {
                    context.stringDoc.Load(part.GetStream());
                    context.stringTableNode = context.stringDoc.SelectSingleNode("//s:sst", context.nsManager);
                    int num = -1;
                    for (XmlNode xmlNode = context.stringTableNode.FirstChild; xmlNode != null; xmlNode = xmlNode.NextSibling)
                    {
                        context.sharedStrings_IntToString[++num] = xmlNode.InnerText;
                    }
                    try
                    {
                        context.sharedStringInitialTotalCount = int.Parse(context.stringTableNode.Attributes["count"].Value);
                        break;
                    }
                    catch (Exception)
                    {
                        break;
                    }
                }
                break;
            }
        }
    }

    public static string GetSharedString(int n, DocumentContext context)
    {
        return context.sharedStrings_IntToString[n];
    }

    public static int GetSharedStringId(string s, DocumentContext context)
    {
        if (!s_UseSharedStrings)
        {
            AppException.Raise("GetSharedStringId called when s_UseSharedStrings is false.");
        }
        context.sharedStringRunningTotalCount++;
        if (context.sharedStrings_StringToInt.ContainsKey(s))
        {
            return context.sharedStrings_StringToInt[s];
        }
        int count = context.sharedStrings_StringToInt.Count;
        context.sharedStrings_StringToInt[s] = count;
        context.sharedStrings_IntToString[count] = s;
        return count;
    }

    private static int rowNumber(XmlNode rowNode)
    {
        return int.Parse(rowNode.Attributes["r"].Value);
    }

    private static int columnNumber(XmlNode cellNode)
    {
        string value = cellNode.Attributes["r"].Value;
        Match match;
        if ((match = ExcelSupport.s_A1ReferenceRegex.Match(value)).Success)
        {
            return ExcelSupport.colToInt(match.Result("${col}"));
        }
        if ((match = ExcelSupport.s_R1C1ReferenceRegex.Match(value)).Success)
        {
            return int.Parse(match.Result("${colnum}"));
        }
        AppException.Raise(string.Format("Node's cell ({0}) in bad format in: {1}", value, cellNode));
        return 0;
    }

    public static XmlNode FindSheet(string sheetName, XmlDocument doc, DocumentContext context)
    {
        if (sheetName == null)
        {
            return doc.SelectNodes("//w:sheet", context.nsManager)[0];
        }
        string xpath = string.Format("//w:sheet[@name='{0}']", sheetName);
        return doc.SelectSingleNode(xpath, context.nsManager);
    }

    public static XmlNode FindSheet(int sheetIndex, XmlDocument doc, DocumentContext context, bool for_write)
    {
        XmlNodeList xmlNodeList = doc.SelectNodes("//w:sheet", context.nsManager);
        uint argument = (for_write ? 3u : 2u);
        if (xmlNodeList == null || xmlNodeList.Count <= sheetIndex)
        {
            AppException.Raise(string.Format("bad sheet index", default(ReadOnlySpan<object>)), argument);
        }
        return xmlNodeList[sheetIndex];
    }

    public static void SetDescriptorFromWorksheet(Descriptor desc, XmlDocument sheetDoc, DocumentContext context)
    {
        XmlNode xmlNode = sheetDoc.SelectSingleNode("//w:dimension", context.nsManager);
        if (xmlNode != null)
        {
            XmlAttribute xmlAttribute = xmlNode.Attributes["ref"];
            if (xmlAttribute != null)
            {
                desc.Dimension = xmlAttribute.Value;
            }
        }
    }

    private static XmlNode MakeRowNode(int row, XmlDocument sheetDoc)
    {
        XmlElement xmlElement = sheetDoc.CreateElement("row", "http://schemas.openxmlformats.org/spreadsheetml/2006/main");
        XmlAttribute xmlAttribute = sheetDoc.CreateAttribute("r");
        xmlAttribute.Value = row.ToString();
        xmlElement.Attributes.Append(xmlAttribute);
        return xmlElement;
    }

    private static XmlNode MakeCellNode(int row, int col, XmlDocument sheetDoc)
    {
        XmlElement xmlElement = sheetDoc.CreateElement("c", "http://schemas.openxmlformats.org/spreadsheetml/2006/main");
        XmlAttribute xmlAttribute = sheetDoc.CreateAttribute("r");
        xmlElement.Attributes.Append(xmlAttribute);
        string value = Descriptor.MakeA1Reference(row, col);
        xmlAttribute.Value = value;
        XmlNode xmlNode = sheetDoc.CreateElement("v", "http://schemas.openxmlformats.org/spreadsheetml/2006/main");
        xmlElement.AppendChild(xmlNode);
        xmlNode.InnerText = "";
        return xmlElement;
    }

    public static MatVal readExcel(string file, string descriptor, string cwd, object fill, object blank)
    {
        return readExcel(Path.Combine(cwd, file), descriptor, fill, blank);
    }

    public static MatVal readExcel(string fileName, string descriptor, object fill, object blank)
    {
        Descriptor desc = new Descriptor(descriptor, fill, blank);
        using FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        using Package xlPackage = Package.Open(stream, FileMode.Open, FileAccess.Read);
        return readExcel(xlPackage, desc);
    }

    public static MatVal readExcel(Package xlPackage, string descriptor, object fill, object blank)
    {
        return readExcel(xlPackage, new Descriptor(descriptor, fill, blank));
    }

    public static MatVal readExcel(string file, string cwd, int r, int c, int nr, int nc, object fill, object blank)
    {
        return readExcel(Path.Combine(cwd, file), r, c, nr, nc, fill, blank);
    }

    public static MatVal readExcel(string fileName, int r, int c, int nr, int nc, object fill, object blank)
    {
        using Package xlPackage = Package.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
        return readExcel(xlPackage, r, c, nr, nc, fill, blank);
    }

    public static MatVal readExcel(Package xlPackage, int r, int c, int nr, int nc, object fill, object blank)
    {
        return readExcel(xlPackage, new Descriptor(r, c, nr, nc, fill, blank));
    }

    public static MatVal readExcel(Package xlPackage, Descriptor desc)
    {
        EstablishWorksheetContext(desc, xlPackage, out var sheetDoc, out var context);
        return GetWorksheetContents(sheetDoc, context, desc);
    }

    public static object writeExcel(string file, string descriptor, MatVal V, string cwd)
    {
        return writeExcel(file, descriptor, V, cwd, s_WriteExcelCreateDirectories, s_WriteExcelOverwritesFiles);
    }

    public static bool UsesIsolatedStorage(MatVal V)
    {
        return V.Rows * V.Cols > 10000;
    }

    public static object writeExcel(string file, string descriptor, MatVal V, string cwd, bool create_directories, bool overwrite_files)
    {
        if (UsesIsolatedStorage(V))
        {
            return writeExcelInIsolatedContext(file, descriptor, V, cwd, create_directories, overwrite_files);
        }
        try
        {
            return writeExcelInner(file, descriptor, V, cwd, create_directories, overwrite_files);
        }
        catch (IsolatedStorageException)
        {
            return writeExcelInIsolatedContext(file, descriptor, V, cwd, create_directories, overwrite_files);
        }
    }

    public static void setTypedCellContents(Descriptor desc, string cellType, string cellValue)
    {
        AppException.Raise("Not yet implemented.");
    }

    public static void setCellContents(Descriptor desc, string text)
    {
        if (text == null)
        {
            setCellContents(desc, "");
        }
        else
        {
            setTypedCellContents(desc, "inlineStr", text);
        }
    }

    public static void setCellContents(Descriptor desc, double cellValue)
    {
        if (double.IsNaN(cellValue))
        {
            setCellContents(desc, "");
            return;
        }
        int num = (int)Math.Truncate(cellValue);
        if (cellValue - (double)num == 0.0)
        {
            setTypedCellContents(desc, "n", num.ToString());
        }
        else
        {
            setTypedCellContents(desc, "n", cellValue.ToString());
        }
    }

    public static void SetTypedXmlNodeValue(XmlNode cellNode, string type, string text, DocumentContext context)
    {
        if (cellNode == null)
        {
            AppException.Raise("Setting typed node null?");
        }
        if ((type == "n" || type == "") && text == "NaN")
        {
            SetTypedXmlNodeValue(cellNode, "", "", context);
            return;
        }
        while (cellNode.LastChild != null)
        {
            cellNode.RemoveChild(cellNode.LastChild);
        }
        XmlAttribute xmlAttribute = cellNode.Attributes["t"];
        if (type == "" || type == "n")
        {
            if (xmlAttribute != null)
            {
                cellNode.Attributes.Remove(xmlAttribute);
            }
        }
        else
        {
            if (xmlAttribute == null)
            {
                xmlAttribute = context.sheetDoc.CreateAttribute("t");
                cellNode.Attributes.Append(xmlAttribute);
                xmlAttribute.Value = type;
            }
            xmlAttribute.Value = type;
        }
        if (type == "inlineStr")
        {
            XmlNode xmlNode = context.sheetDoc.CreateElement("is", "http://schemas.openxmlformats.org/spreadsheetml/2006/main");
            cellNode.AppendChild(xmlNode);
            XmlNode xmlNode2 = context.sheetDoc.CreateElement("t", "http://schemas.openxmlformats.org/spreadsheetml/2006/main");
            xmlNode.AppendChild(xmlNode2);
            xmlNode2.InnerText = text;
        }
        else
        {
            XmlNode xmlNode3 = context.sheetDoc.CreateElement("v", "http://schemas.openxmlformats.org/spreadsheetml/2006/main");
            xmlNode3.InnerText = text;
            cellNode.AppendChild(xmlNode3);
        }
    }

    public static void EstablishWorksheetContext(Descriptor desc, Package xlPackage, out XmlDocument sheetDoc, out DocumentContext context)
    {
        EstablishWorksheetContext(desc, xlPackage, out sheetDoc, out context, for_write: false);
    }

    public static string cellNodeContent(XmlNode cellNode, ref DocumentContext context)
    {
        if (cellNode == null)
        {
            return null;
        }
        string type = string.Empty;
        XmlAttribute typeAttribute = cellNode.Attributes["t"];
        if (typeAttribute != null)
        {
            type = typeAttribute.Value;
        }
        string text = null;
        XmlNode xmlNode;
        switch (type)
        {
            case "":
            case "b":
            case "e":
            case "n":
            case "str":
            {
                xmlNode = cellNode.SelectSingleNode("w:v", context.nsManager);
                if (xmlNode != null)
                {
                    text = xmlNode.InnerText;
                }
                break;
            }
            case "s":
            {
                xmlNode = cellNode.SelectSingleNode("w:v", context.nsManager);
                if (xmlNode != null)
                {
                    text = xmlNode.InnerText;
                }
                LoadStringDocIfNeeded(context);
                text = GetSharedString(Convert.ToInt32(text), context);
                break;
            }
            case "inlineStr":
            {
                xmlNode = cellNode.SelectSingleNode("w:is", context.nsManager);
                if (xmlNode != null)
                {
                    text = xmlNode.InnerText;
                }
                break;
            }
            default:
                AppException.Raise(string.Format("Unsupported cellType = '{0}'", type));
                break;
        }
        return text;
    }

    public static MatVal GetWorksheetContents(XmlDocument sheetDoc, DocumentContext context, Descriptor desc)
    {
        XmlNode[,] worksheetNodes = GetWorksheetNodes(desc.BlankRows == 2, sheetDoc, context, desc);
        int left = desc.Left;
        int right = desc.Right;
        _ = desc.Fill;
        int num = worksheetNodes.GetUpperBound(0) + 1;
        int num2 = right - left + 1;
        MatVal matVal = new MatVal(num, num2);
        matVal.dv(Dim.identity);
        for (int i = 0; i < num; i++)
        {
            for (int j = 0; j < num2; j++)
            {
                string text = cellNodeContent(worksheetNodes[i, j], ref context);
                if (text != null && text.Length > 0)
                {
                    double result;
                    NumVal A_ = null;
                    if (double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out result))
                    {
                        matVal.set_2(i, j, result);
                    }
                    else if (Quire.Efi.Core.try_parse_complex(text, ref A_))
                    {
                        matVal.set_2(i, j, A_);
                    }
                    else
                    {
                        matVal.set_2(i, j, text);
                    }
                }
                else
                {
                    matVal.set_2(i, j, desc.Fill);
                }
            }
        }
        return matVal;
    }
}
