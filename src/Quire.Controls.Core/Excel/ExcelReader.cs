#nullable disable
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Quire.Mirror;

namespace Quire.Controls.Core.Excel;

public partial class ExcelReader
{
    private static readonly Regex addressRegex = new Regex(@"^(?<col>\D+)(?<row>\d+)");

    [MirrorVerified("PtcControlsCoreExcelExcelReaderMechanicalDifferentialTests")]
    public ExcelBook WorkBook { get; private set; }

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
}
