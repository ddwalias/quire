using System.Globalization;
using System.IO;
using System.Xml;
using Quire.AutomationPrivate;
using Quire.Mcd;

namespace Quire.Automation;

public partial class NumericValue : Value, INumericValue, IValue, INumericValuePrivate
{
    public NumVal Number => (NumVal)core;

    public override int Size => 1 + ((Imag != 0.0 && Real != 0.0) ? 1 : 0);

    public double Imag
    {
        get => Number.Data.ip;
        set => Number.Data.ip = value;
    }

    public double Real
    {
        get => Number.Data.rp;
        set => Number.Data.rp = value;
    }

    public int Integer
    {
        get => Number.asInt();
        set
        {
            Number.Data.rp = value;
            Number.Data.ip = 0.0;
        }
    }

    public override string Type => "Numeric";

    public override string AsString
    {
        get
        {
            double num = 0.0;
            double imaginaryPart = 0.0;
            string format;
            if (Real == 0.0 && Imag != 0.0)
            {
                num = Imag;
                format = "{0:R}i";
            }
            else if (Imag == 0.0)
            {
                format = "{0:R}";
                num = Real;
            }
            else
            {
                format = "{0:R}";
                format += ((Imag >= 0.0) ? "+{1:R}i" : "{1:R}i");
                num = Real;
                imaginaryPart = Imag;
            }
            return string.Format(format, num, imaginaryPart);
        }
    }

    public NumericValue()
        : base(new NumVal())
    {
    }

    public NumericValue(NumVal num)
        : base(num, RuntimeOptions.SaveThreadOptions())
    {
    }

    public NumericValue(NumVal num, RuntimeOptions o)
        : base(num, o)
    {
    }

    public NumericValue(int n)
        : base(new NumVal(n))
    {
    }

    public NumericValue(double r)
        : base(new NumVal(r))
    {
    }

    public NumericValue(double r, double i)
        : base(new NumVal(r, i))
    {
    }

    public static string FormatDoubleLegacy(double value, NumberFormatInfo NmFmt)
    {
        if (value == 0.0)
        {
            value = 0.0;
        }
        string text = value.ToString("G15", NmFmt);
        if (value == double.Parse(text, NmFmt))
        {
            return text;
        }
        return value.ToString("G17", NmFmt);
    }

    internal override void WriteValueXml(XmlWriter xml, UnitAction action, double scaleFactor)
    {
        if (Imag != 0.0)
        {
            Value.WriteMathElementStart(xml, "complex");
            if (Real != 0.0)
            {
                Value.WriteMathElementString(xml, "real", FormatDoubleLegacy(Real / scaleFactor, Value.NumFmt));
            }
            Value.WriteMathElementString(xml, "imag", FormatDoubleLegacy(Imag / scaleFactor, Value.NumFmt));
            xml.WriteEndElement();
        }
        else
        {
            Value.WriteMathElementString(xml, "real", FormatDoubleLegacy(Real / scaleFactor, Value.NumFmt));
        }
    }

    internal override void WriteValueRL(TextWriter rl, UnitAction action)
    {
        if (Imag != 0.0)
        {
            if (Real != 0.0)
            {
                rl.Write("(op_add (number {0}) (number {1}i))", FormatDoubleLegacy(Real, Value.NumFmt), FormatDoubleLegacy(Imag, Value.NumFmt));
            }
            else
            {
                rl.Write("(number {0}i)", FormatDoubleLegacy(Imag, Value.NumFmt));
            }
        }
        else
        {
            rl.Write("(number {0})", FormatDoubleLegacy(Real, Value.NumFmt));
        }
    }

    public string UnitsXML(bool simplify)
    {
        Units units = GetUnits(simplify);
        if (units == null)
        {
            return "";
        }
        StringWriter stringWriter = new StringWriter();
        XmlTextWriter xml = new XmlTextWriter(stringWriter);
        WriteUnitsXml(xml, units);
        return stringWriter.ToString();
    }
}
