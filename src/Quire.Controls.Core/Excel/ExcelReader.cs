#nullable disable
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using DocumentFormat.OpenXml.Packaging;
using Quire.IO;
using Quire.Mirror;

namespace Quire.Controls.Core.Excel;

public partial class ExcelReader
{
    private static readonly Regex addressRegex = new Regex(@"^(?<col>\D+)(?<row>\d+)");

    [MirrorVerified("PtcControlsCoreExcelExcelReaderMechanicalDifferentialTests")]
    public ExcelBook WorkBook { get; private set; }

    private readonly XmlNamespaceManager _nsManager = new XmlNamespaceManager(new NameTable());

    private SpreadsheetDocument _doc;

    private XmlDocument _sharedStringDoc;

    private static int GetRow(string address)
    {
        return Convert.ToInt32(addressRegex.Match(address).Result("${row}"));
    }

    private static string GetColumn(string address)
    {
        return addressRegex.Match(address).Result("${col}");
    }

    public static string GetNextColumn(string column)
    {
        if (string.IsNullOrEmpty(column))
        {
            return "A";
        }
        string text = column.ToUpper();
        int length = text.Length - 1;
        char c = text[text.Length - 1];
        if (c == 'Z')
        {
            text = GetNextColumn(text.Substring(0, length)) + "A";
        }
        else
        {
            text = text.Substring(0, length) + char.ConvertFromUtf32(c + 1);
        }
        return text;
    }

    private string GetSharedString(string sharedStringId)
    {
        if (_sharedStringDoc == null)
        {
            Uri uri = _doc.WorkbookPart.SharedStringTablePart.Uri;
            PackagePart part = _doc.Package.GetPart(uri);
            _sharedStringDoc = new XmlDocument();
            _sharedStringDoc.Load(part.GetStream());
        }
        string result = string.Empty;
        int num = Convert.ToInt32(sharedStringId);
        string xpath = string.Format("//s:sst/s:si[{0}]", num + 1);
        XmlNode xmlNode = _sharedStringDoc.SelectSingleNode(xpath, _nsManager);
        if (xmlNode != null)
        {
            result = xmlNode.InnerText;
        }
        return result;
    }

    private ExcelCell CreateExcelCel(XmlElement element)
    {
        string attribute = element.GetAttribute("r");
        string cellType = element.GetAttribute("t");
        string value = string.Empty;
        XmlNode xmlNode = element.SelectSingleNode("s:v", _nsManager);
        if (xmlNode != null)
        {
            switch (cellType)
            {
                case "b":
                    value = xmlNode.InnerText == "1" ? "TRUE" : "FALSE";
                    break;
                case "s":
                    value = GetSharedString(xmlNode.InnerText);
                    break;
                default:
                    value = xmlNode.InnerText;
                    break;
            }
        }
        return new ExcelCell
        {
            Column = GetColumn(attribute),
            Row = GetRow(attribute),
            Value = value
        };
    }

    private ExcelRow CreateExcelRow(XmlElement element)
    {
        IDictionary<string, ExcelCell> dictionary = new Dictionary<string, ExcelCell>();
        XmlNodeList xmlNodeList = element.SelectNodes("s:c", _nsManager);
        if (xmlNodeList != null)
        {
            foreach (XmlElement item in xmlNodeList)
            {
                ExcelCell excelCell = CreateExcelCel(item);
                if (!string.IsNullOrEmpty(excelCell.Value))
                {
                    dictionary[excelCell.Column] = excelCell;
                }
            }
        }
        return new ExcelRow
        {
            Row = Convert.ToInt32(element.GetAttribute("r")),
            Cells = dictionary
        };
    }

    public static int CompareColumn(string first, string second)
    {
        if (first.Length != second.Length)
        {
            return first.Length - second.Length;
        }
        return first.CompareTo(second);
    }

    public static string MinColumn(string column1, string column2)
    {
        if (CompareColumn(column1, column2) <= 0)
        {
            return column1;
        }
        return column2;
    }

    private static IEnumerable<string> CreateColumns(string startColumn, string endColumn)
    {
        List<string> list = new List<string>();
        string text = startColumn;
        while (CompareColumn(text, endColumn) <= 0)
        {
            list.Add(text);
            text = GetNextColumn(text);
        }
        return list;
    }

    private ExcelSheet CreateExcelSheet(XmlNode element, string name)
    {
        List<ExcelRow> list = new List<ExcelRow>();
        int num = 0;
        string text = "A";
        XmlNode xmlNode = element.SelectSingleNode("s:sheetData", _nsManager);
        if (xmlNode != null)
        {
            XmlNodeList xmlNodeList = xmlNode.SelectNodes("s:row", _nsManager);
            if (xmlNodeList != null)
            {
                foreach (XmlElement item in xmlNodeList)
                {
                    ExcelRow excelRow = CreateExcelRow(item);
                    if (num + 1 < excelRow.Row)
                    {
                        for (int i = num + 1; i < excelRow.Row; i++)
                        {
                            list.Add(new ExcelRow
                            {
                                Row = i
                            });
                        }
                    }
                    list.Add(excelRow);
                    num = excelRow.Row;
                    if (excelRow.Cells.Count() > 0)
                    {
                        string column = excelRow.Cells.Last().Value.Column;
                        if (CompareColumn(column, text) > 0)
                        {
                            text = column;
                        }
                    }
                }
            }
        }
        IEnumerable<string> columns = CreateColumns("A", text);
        return new ExcelSheet
        {
            Rows = list,
            Columns = columns,
            Name = name
        };
    }

    public void LoadExcelBook(string fileName)
    {
        using Stream stream = PackageOperationsProvider.OpenFileStreamForReadWithFullShare(fileName);
        using (_doc = SpreadsheetDocument.Open(stream, isEditable: false))
        {
            List<ExcelSheet> list = new List<ExcelSheet>();
            PackagePart part = _doc.Package.GetPart(_doc.WorkbookPart.Uri);
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(part.GetStream());
            XmlNodeList xmlNodeList = xmlDocument.SelectNodes("s:workbook/s:sheets/s:sheet", _nsManager);
            if (xmlNodeList != null)
            {
                foreach (XmlElement item in xmlNodeList)
                {
                    string attribute = item.GetAttribute("r:id");
                    string cellType = item.GetAttribute("name");
                    OpenXmlPart partById = _doc.WorkbookPart.GetPartById(attribute);
                    PackagePart workbookPart = _doc.Package.GetPart(partById.Uri);
                    XmlDocument worksheetDocument = new XmlDocument();
                    worksheetDocument.Load(workbookPart.GetStream());
                    list.Add(CreateExcelSheet(worksheetDocument.DocumentElement, cellType));
                }
            }
            WorkBook = new ExcelBook
            {
                Sheets = list
            };
        }
    }

    public ExcelReader()
    {
        _nsManager.AddNamespace("s", "http://schemas.openxmlformats.org/spreadsheetml/2006/main");
        _nsManager.AddNamespace("r", "http://schemas.openxmlformats.org/officeDocument/2006/relationships");
    }
}
