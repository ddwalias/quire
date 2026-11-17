#nullable disable
#pragma warning disable

using System;
using System.Text;
using System.Text.RegularExpressions;
using Quire.Mpl.Run;

namespace Quire.efiio_csharp;

public partial class ExcelSupport
{
    protected const string s_DocumentRelationshipType = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument";

    protected const string s_SharedStringsRelationshipType = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/sharedStrings";

    protected const string s_SharedStringSchema = "http://schemas.openxmlformats.org/spreadsheetml/2006/main";

    protected const string s_WorksheetSchema = "http://schemas.openxmlformats.org/spreadsheetml/2006/main";

    protected static Regex s_A1DimensionRegex = new Regex("^(?<left>\\D+)(?<top>\\d+):(?<right>\\D+)(?<bottom>\\d+)$");

    protected static Regex s_R1C1DimensionRegex = new Regex("^[Rr](?<topnum>\\d+)[Cc](?<leftnum>\\d+):[Rr](?<bottomnum>\\d+)[Cc](?<rightnum>\\d+)$");

    protected static Regex s_A1ReferenceRegex = new Regex("^(?<col>\\D+)(?<row>\\d+)$");

    protected static Regex s_R1C1ReferenceRegex = new Regex("^[Rr](?<rownum>\\d+)[Cc](?<colnum>\\d+)$");

    protected static Regex s_RowColRange = new Regex("^(?<start>\\d+)((?<range>-)(?<end>\\d*))?$");

    protected void swap<T>(ref T var1, ref T var2)
    {
        T val = var1;
        var1 = var2;
        var2 = val;
    }

    public static bool ParseRowColRange(string s, out int start, out int end)
    {
        start = -1;
        end = -1;
        try
        {
            Match match;
            if ((match = s_RowColRange.Match(s)).Success)
            {
                string s2 = match.Result("${start}");
                start = int.Parse(s2);
                if (match.Result("${range}").Length >= 1)
                {
                    string text = match.Result("${end}");
                    end = ((text.Length > 0) ? int.Parse(text) : (-1));
                }
                else
                {
                    end = start;
                }
                return true;
            }
            return false;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public static string nextCol(string col)
    {
        return intToCol(colToInt(col) + 1);
    }

    public static string prevCol(string col)
    {
        return intToCol(colToInt(col) - 1);
    }

    public static int colToInt(string col)
    {
        return colToInt(col, 0, col.Length);
    }

    public static int colToInt(string column_ref, int start, int end)
    {
        if (string.IsNullOrEmpty(column_ref))
        {
            AppException.Raise("Null or empty row identifier");
        }
        int num = 1;
        int num2 = 0;
        int num3 = end - 1;
        while (num3 >= start)
        {
            char c = char.ToUpperInvariant(column_ref[num3]);
            if (!char.IsLetter(c))
            {
                AppException.Raise("Invalid char");
            }
            int num4 = c - 65 + 1;
            num2 += num4 * num;
            num3--;
            num *= 26;
        }
        return num2;
    }

    public static string intToCol(int num)
    {
        StringBuilder stringBuilder = new StringBuilder();
        while (num > 0)
        {
            int num2 = num / 26;
            int num3 = num % 26;
            if (num3 == 0)
            {
                num = num2 - 1;
                stringBuilder.Append('Z');
            }
            else
            {
                num = num2;
                stringBuilder.Append((char)(64 + num3));
            }
        }
        return reverse(stringBuilder).ToString();
    }

    public static StringBuilder reverse(StringBuilder sb)
    {
        int num = sb.Length / 2;
        int num2 = sb.Length;
        for (int i = 0; i < num; i++)
        {
            char value = sb[i];
            sb[i] = sb[--num2];
            sb[num2] = value;
        }
        return sb;
    }

    public static object testColumns()
    {
        return testColumns(1000000);
    }

    public static object testColumns(int z)
    {
        string arg = null;
        try
        {
            for (int i = 1; i <= z; i++)
            {
                string text = intToCol(i);
                arg = text;
                int num = colToInt(text);
                if (num != i)
                {
                    AppException.Raise(string.Format("Lose: {0} {1} {2}", i, text, num));
                }
            }
        }
        catch (Exception ex)
        {
            return string.Format("Failed: {0}", ex.Message);
        }
        return string.Format("PASSED: {0} = {1}", z, arg);
    }
}
