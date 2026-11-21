using System.Globalization;
using System.IO;
using System.Xml;
using Quire.Mcd;
using Quire.Mpl.Run;

namespace Quire.Automation;

public abstract partial class ValueProxy : IValueProxy
{
    protected readonly bool m_Simplify;

    internal RuntimeOptions m_options;

    protected Dim Dim
    {
        get
        {
            DimVal dimVal = Core as DimVal;
            if (dimVal != null && dimVal.m_dim != null)
            {
                return dimVal.m_dim;
            }
            return Dim.identity;
        }
    }

    public bool Simplify => m_Simplify;

    protected Units Unit
    {
        get
        {
            if (!m_Simplify)
            {
                return m_options.m_unit_system.Normalize(Dim.Coordinates);
            }
            return m_options.m_unit_system.Simplify(Dim.Coordinates);
        }
    }

    public abstract object Core { get; }

    public virtual IScalarValueProxy AsScalar => null;

    public virtual IMatrixValueProxy AsMatrix => null;

    public virtual IFunctionValueProxy AsFunction => null;

    public virtual IStringValueProxy AsString => null;

    public virtual ISymbolicValueProxy AsSymbolic => null;

    public UnitPower[] Units => GetUnits();

    public string Xml => GetAutomationValue().XML(m_Simplify);

    protected UnitPower[] GetUnits()
    {
        Units unit = Unit;
        int num = unit.Powers.Length;
        UnitPower[] array = new UnitPower[num];
        for (int i = 0; i < num; i++)
        {
            Units.Power power = unit.Powers[i];
            array[i] = new UnitPower(power.Base.Name, power.Exponent);
        }
        return array;
    }

    protected ValueProxy(bool bSimplify, RuntimeOptions o)
    {
        m_Simplify = bSimplify;
        m_options = o;
    }

    public override string ToString()
    {
        return "{ValueProxy for " + Core?.ToString() + "}";
    }

    private Value GetAutomationValue()
    {
        return ValueFactory.ToAutomation(Core, "", m_options) as Value;
    }

    public void WriteXml(TextWriter writer, bool simplify, double scaleFactor)
    {
        GetAutomationValue().WriteXml(writer, simplify, scaleFactor);
    }

    public void WriteXml(TextWriter writer, bool simplify)
    {
        GetAutomationValue().WriteXml(writer, simplify);
    }

    public void WriteXml(TextWriter writer)
    {
        GetAutomationValue().WriteXml(writer, m_Simplify);
    }

    public void WriteXml(XmlWriter xml, bool simplify, double scaleFactor)
    {
        GetAutomationValue().WriteXml(xml, simplify);
    }

    public void WriteXml(XmlWriter xml, bool simplify)
    {
        GetAutomationValue().WriteXml(xml, simplify);
    }

    public void WriteXml(XmlWriter xml)
    {
        GetAutomationValue().WriteXml(xml, m_Simplify);
    }

    public void WriteUnitsXml(XmlWriter xml)
    {
        UnitPower[] units = Units;
        if (units == null)
        {
            return;
        }
        xml.WriteStartElement("u:unitMonomial");
        xml.WriteAttributeString("system", m_options.m_unit_system.Name);
        UnitPower[] array = units;
        for (int i = 0; i < array.Length; i++)
        {
            UnitPower unitPower = array[i];
            if (unitPower.Power != 0)
            {
                xml.WriteStartElement("u:unitReference");
                xml.WriteAttributeString("unit", unitPower.Name);
                if (unitPower.Power.Numerator != 1)
                {
                    xml.WriteAttributeString("power-numerator", unitPower.Power.Numerator.ToString(NumberFormatInfo.InvariantInfo));
                }
                if (unitPower.Power.Denominator != 1)
                {
                    xml.WriteAttributeString("power-denominator", unitPower.Power.Denominator.ToString(NumberFormatInfo.InvariantInfo));
                }
                xml.WriteEndElement();
            }
        }
        xml.WriteEndElement();
    }

    internal static IValueProxy MakeProxy(object o, bool bSimplify, RuntimeOptions options)
    {
        if (o == null)
        {
            return null;
        }
        NumVal numVal = o as NumVal;
        if (numVal != null)
        {
            return new ScalarValueProxy(numVal, bSimplify, options);
        }
        MatVal matVal = o as MatVal;
        if (matVal != null)
        {
            matVal.Immutable = true;
            return new MatrixValueProxy(matVal, bSimplify, options);
        }
        RngVal rngVal = o as RngVal;
        if (rngVal != null)
        {
            matVal = ValueFactory.RangeToVector(rngVal);
            return new MatrixValueProxy(matVal, bSimplify, options);
        }
        StrVal strVal = o as StrVal;
        if (strVal != null)
        {
            return new StringValueProxy(strVal, bSimplify, options);
        }
        string text = o as string;
        if (text != null)
        {
            return new StringValueProxy(text, bSimplify, options);
        }
        Function function = o as Function;
        if (function != null)
        {
            return new FunctionValueProxy(function, bSimplify, options);
        }
        return new SymbolicValueProxy(o, bSimplify, options);
    }
}
