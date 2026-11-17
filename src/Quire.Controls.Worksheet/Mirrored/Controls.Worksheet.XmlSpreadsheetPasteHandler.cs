using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Xml;
using Quire.Controls.Core;
using Quire.Controls.Plots.ViewModel;

namespace Quire.Controls.Worksheet;

internal static partial class XmlSpreadsheetPasteHandler
{
    private static IControlFactory ControlCreator => WorksheetViewModel.ControlCreator;

    private static string AddQuotationMarks(string value)
    {
        value = value.Replace("\"", "\\\"");
        return string.Format("\"{0}\"", value);
    }

    private static string FormatNumber(string value, XmlNode cellNode, XmlNamespaceManager nsManager)
    {
        double value2 = double.Parse(value, NumberFormatInfo.InvariantInfo);
        string excelNumberFormat = GetExcelNumberFormat(cellNode, nsManager);
        return FormatNumber(value2, excelNumberFormat);
    }

    private static string FormatNumber(double value, string excelNumberFormat)
    {
        NumberFormat numberFormat;
        switch (excelNumberFormat)
        {
            case null:
                {
                    int value2;
                    if (value >= Math.Pow(10.0, 11.0))
                    {
                        value2 = 5;
                    }
                    else
                    {
                        int num = ((value == 0.0) ? 1 : ((int)Math.Ceiling(Math.Log10(Math.Abs(value)))));
                        value2 = 10 - num;
                    }
                    numberFormat = new NumberFormat
                    {
                        DecimalPlaces = value2,
                        ShowTrailingDecimalZeros = false,
                        ExponentialThreshold = 11,
                        NumberStyle = NumberFormat.NumberStyles.General
                    };
                    break;
                }
            case "Fixed":
                numberFormat = new NumberFormat
                {
                    DecimalPlaces = 2,
                    ShowTrailingDecimalZeros = true,
                    NumberStyle = NumberFormat.NumberStyles.Decimal
                };
                break;
            case "Scientific":
                numberFormat = new NumberFormat
                {
                    DecimalPlaces = 2,
                    ShowTrailingDecimalZeros = true,
                    NumberStyle = NumberFormat.NumberStyles.Scientific
                };
                break;
            case "Percent":
                numberFormat = new NumberFormat
                {
                    DecimalPlaces = 2,
                    ShowTrailingDecimalZeros = true,
                    NumberStyle = NumberFormat.NumberStyles.Percent
                };
                break;
            default:
                {
                    string text = FixExcelNumberFormat(excelNumberFormat);
                    if (IsNumberFormat(value, text))
                    {
                        numberFormat = CreateNumberFormatFromExcelFormat(value, text, NumberFormat.NumberStyles.Decimal);
                        break;
                    }
                    if (IsFloatFormat(value, text))
                    {
                        numberFormat = CreateNumberFormatFromExcelFormat(value, text, NumberFormat.NumberStyles.Scientific);
                        break;
                    }
                    if (IsPrecentNumberFormat(value, text))
                    {
                        numberFormat = CreateNumberFormatFromExcelFormat(value, text, NumberFormat.NumberStyles.Percent);
                        break;
                    }
                    return AddQuotationMarks(value.ToString(text));
                }
        }
        SExpressionFormatter formatter = new SExpressionFormatter(numberFormat);
        return formatter.FormatReal(value);
    }

    private static NumberFormat CreateNumberFormatFromExcelFormat(double value, string excelNumberFormat, NumberFormat.NumberStyles numberStyle)
    {
        GetDecimalPlacesAndShowTralingZeros(value, excelNumberFormat, out var decimalPlaces, out var showTrailingZeros);
        return new NumberFormat
        {
            DecimalPlaces = decimalPlaces,
            ShowTrailingDecimalZeros = showTrailingZeros,
            NumberStyle = numberStyle
        };
    }

    private static string FixExcelNumberFormat(string excelNumberFormat)
    {
        return Regex.Replace(excelNumberFormat, "\\[.*\\]", "");
    }

    private static string GetExcelNumberFormat(XmlNode cellNode, XmlNamespaceManager nsManager)
    {
        string arg = "Default";
        if (cellNode.Attributes != null)
        {
            XmlAttribute xmlAttribute = cellNode.Attributes["ss:StyleID"];
            if (xmlAttribute != null)
            {
                arg = xmlAttribute.Value;
            }
        }
        XmlNode formatNode = cellNode.SelectSingleNode(string.Format("/ss:Workbook/ss:Styles/ss:Style[@ss:ID='{0}']/ss:NumberFormat/@ss:Format", arg), nsManager);
        return (formatNode != null) ? formatNode.Value : null;
    }

    private static bool IsNumberFormat(double value, string format)
    {
        double result;
        return double.TryParse(value.ToString(format, NumberFormatInfo.InvariantInfo), NumberStyles.Number, NumberFormatInfo.InvariantInfo, out result);
    }

    private static bool IsFloatFormat(double value, string format)
    {
        double result;
        return double.TryParse(value.ToString(format, NumberFormatInfo.InvariantInfo), NumberStyles.Float, NumberFormatInfo.InvariantInfo, out result);
    }

    private static bool IsPrecentNumberFormat(double value, string format)
    {
        if (format[format.Length - 1] == '%')
        {
            return IsNumberFormat(value, format.Remove(format.Length - 1));
        }
        return false;
    }

    private static void GetDecimalPlacesAndShowTralingZeros(double value, string format, out int decimalPlaces, out bool showTrailingZeros)
    {
        string[] array = format.Split(';');
        switch (array.Length)
        {
            case 1:
                format = array[0];
                break;
            case 2:
                format = (value >= 0.0) ? array[0] : array[1];
                break;
            default:
                format = (value > 0.0) ? array[0] : ((value < 0.0) ? array[1] : array[2]);
                break;
        }
        Regex regex = new Regex("\\.([0#])*");
        Match match = regex.Match(format);
        if (match.Success)
        {
            CaptureCollection captures = match.Groups[1].Captures;
            decimalPlaces = captures.Count;
            showTrailingZeros = captures[captures.Count - 1].Value == "0";
        }
        else
        {
            decimalPlaces = 0;
            showTrailingZeros = false;
        }
    }
}
