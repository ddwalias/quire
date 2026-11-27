using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Xml;
using Quire.AutomationPrivate;
using Quire.Mcd;

namespace Quire.Automation;

public abstract partial class Value : MarshalByRefObject, IValuePrivate, IValue, IAutomationValue
{
    protected object core;

    internal RuntimeOptions m_options;

    internal static string s_MathPrefix = "ml";

    internal static string s_UnitPrefix = "u";

    internal static string s_XmlPrefix = "xml";

    internal static int s_MathNamespaceUriVersionForClassic = 40;

    internal static int s_MathNamespaceUriVersion = s_MathNamespaceUriVersionForClassic;

    internal static string s_MathNamespaceUriPrefix = "http://schemas.mathsoft.com/math";

    internal static string s_MathNamespaceUriForClassic = "http://schemas.mathsoft.com/math40";

    internal static string s_MathNamespaceUri = s_MathNamespaceUriForClassic;

    internal static string s_UnitNamespaceUri = "http://schemas.mathsoft.com/units10";

    internal static string s_XmlNamespaceUri = "http://www.w3.org/XML/1998/namespace";

    internal static bool s_WriteXmlNamespaces = true;

    public virtual int Size => 1;

    public object Core
    {
        get => core;
        set => core = value;
    }

    public Dim Dim
    {
        get
        {
            if (core is DimVal && ((DimVal)core).m_dim != null)
            {
                return ((DimVal)core).m_dim;
            }
            return Dim.identity;
        }
    }

    public IValueProxy Raw => GetProxy(false);

    public IValueProxy Simplified => GetProxy(true);

    public static string MathPrefix
    {
        get => s_MathPrefix;
        set => s_MathPrefix = value;
    }

    public static string UnitPrefix
    {
        get => s_UnitPrefix;
        set => s_UnitPrefix = value;
    }

    public static string XmlPrefix
    {
        get => s_XmlPrefix;
        set => s_XmlPrefix = value;
    }

    public static int MathNamespaceUriVersion
    {
        get => s_MathNamespaceUriVersion;
        set => s_MathNamespaceUriVersion = value;
    }

    public static string MathNamespaceUri
    {
        get => s_MathNamespaceUri;
        set
        {
            s_MathNamespaceUri = value;
            int result = s_MathNamespaceUriVersionForClassic;
            if (value.StartsWith(s_MathNamespaceUriPrefix) && !int.TryParse(value.Substring(s_MathNamespaceUriPrefix.Length), out result))
            {
                result = s_MathNamespaceUriVersionForClassic;
            }
            s_MathNamespaceUriVersion = result;
        }
    }

    public static string UnitNamespaceUri
    {
        get => s_UnitNamespaceUri;
        set => s_UnitNamespaceUri = value;
    }

    public static string XmlNamespaceUri
    {
        get => s_XmlNamespaceUri;
        set => s_XmlNamespaceUri = value;
    }

    public static bool WriteXmlNamespaces
    {
        get => s_WriteXmlNamespaces;
        set => s_WriteXmlNamespaces = value;
    }

    public abstract string AsString { get; }

    public abstract string Type { get; }

    public string RL
    {
        get
        {
            StringWriter stringWriter = new StringWriter();
            WriteRL(stringWriter, UnitAction.Normalize);
            return stringWriter.ToString();
        }
    }

    internal static NumberFormatInfo NumFmt => NumberFormatInfo.InvariantInfo;

    public Value()
        : this(new NumVal(), RuntimeOptions.SaveThreadOptions())
    {
    }

    public Value(object val)
        : this(val, RuntimeOptions.SaveThreadOptions())
    {
    }

    public Value(object val, RuntimeOptions o)
    {
        m_options = o;
        core = val;
    }

    public static bool NearlyEqual(object x1, object x2, double fuzz)
    {
        Value value = x1 as Value;
        Value otherValue = x2 as Value;
        if (value == null || otherValue == null)
        {
            return x1.Equals(x2);
        }
        return DimVal.NearlyEqual(value.core, otherValue.core, fuzz);
    }

    public override bool Equals(object other)
    {
        Value value = other as Value;
        if (value == null)
        {
            return false;
        }
        return core.Equals(value.core);
    }

    public override int GetHashCode()
    {
        return core.GetHashCode();
    }

    public override string ToString()
    {
        return core.ToString();
    }

    [Obsolete("This method is obsolete. MarshalByRefObject.InitializeLifetimeService is obsolete in .net 8.", true)]
    public override object InitializeLifetimeService()
    {
        return null;
    }

    public IValueProxy GetProxy(bool bSimplify)
    {
        return ValueProxy.MakeProxy(Core, bSimplify, m_options);
    }

    internal Units GetUnits(bool simplify)
    {
        if (!simplify)
        {
            return m_options.m_unit_system.Normalize(Dim.Coordinates);
        }
        return m_options.m_unit_system.Simplify(Dim.Coordinates);
    }

    internal abstract void WriteValueXml(XmlWriter xml, UnitAction action, double scaleFactor);

    internal void WriteXml(XmlWriter xml, UnitAction action, double scaleFactor)
    {
        if (action == UnitAction.None || Dim.Id_b(Dim) || Dim.NonScalar_b(Dim) || !DimVal.IsDimOf(Core, Dim))
        {
            WriteValueXml(xml, action, scaleFactor);
            return;
        }
        WriteMathElementStart(xml, "unitedValue");
        Units units = GetUnits(action == UnitAction.Simplify);
        Trace.Assert(units != null, "units != null");
        Trace.Assert(scaleFactor == 1.0, "scaleFactor == 1");
        WriteValueXml(xml, UnitAction.None, units.Magnitude);
        WriteUnitsXml(xml, units);
        xml.WriteEndElement();
    }

    internal void WriteUnitsXml(XmlWriter xml, Units units)
    {
        WriteUnitElementStart(xml, "unitMonomial");
        xml.WriteAttributeString("system", m_options.m_unit_system.Name);
        Units.Power[] powers = units.Powers;
        for (int i = 0; i < powers.Length; i++)
        {
            Units.Power power = powers[i];
            if (!(power.Exponent == 0))
            {
                WriteUnitElementStart(xml, "unitReference");
                xml.WriteAttributeString("unit", power.Base.Name);
                if (power.Exponent.Numerator != 1)
                {
                    xml.WriteAttributeString("power-numerator", power.Exponent.Numerator.ToString(NumFmt));
                }
                if (power.Exponent.Denominator != 1)
                {
                    xml.WriteAttributeString("power-denominator", power.Exponent.Denominator.ToString(NumFmt));
                }
                xml.WriteEndElement();
            }
        }
        xml.WriteEndElement();
    }

    internal abstract void WriteValueRL(TextWriter rl, UnitAction action);

    internal void WriteRL(TextWriter rl, UnitAction action)
    {
        if (action == UnitAction.None || Dim.Id_b(Dim) || !DimVal.IsDimOf(Core, Dim))
        {
            WriteValueRL(rl, action);
            return;
        }
        rl.Write("(cast (numeric ");
        WriteValueRL(rl, UnitAction.None);
        rl.Write(") \"V");
        rl.Write(Dim.Product);
        rl.Write("\"");
        rl.Write(")");
    }

    internal static void WriteMathElementString(XmlWriter xml, string tag, string val)
    {
        WriteMathElementStart(xml, tag);
        xml.WriteString(val);
        xml.WriteEndElement();
    }

    internal static void WriteUnitElementString(XmlWriter xml, string tag, string val)
    {
        WriteUnitElementStart(xml, tag);
        xml.WriteString(val);
        xml.WriteEndElement();
    }

    internal static void WriteMathElementStart(XmlWriter xml, string localName)
    {
        if (s_WriteXmlNamespaces)
        {
            xml.WriteStartElement(s_MathPrefix, localName, s_MathNamespaceUri);
        }
        else
        {
            xml.WriteStartElement(string.Format("{0}:{1}", s_MathPrefix, localName));
        }
    }

    internal static void WriteUnitElementStart(XmlWriter xml, string localName)
    {
        if (s_WriteXmlNamespaces)
        {
            xml.WriteStartElement(s_UnitPrefix, localName, s_UnitNamespaceUri);
        }
        else
        {
            xml.WriteStartElement(string.Format("{0}:{1}", s_UnitPrefix, localName));
        }
    }

    internal static void WriteXmlAttribute(XmlWriter xml, string localName, string attrVal)
    {
        if (s_WriteXmlNamespaces)
        {
            xml.WriteAttributeString(s_XmlPrefix, localName, s_XmlNamespaceUri, attrVal);
        }
        else
        {
            xml.WriteAttributeString(string.Format("{0}:{1}", s_XmlPrefix, localName), attrVal);
        }
    }

    public void WriteXml(TextWriter writer, bool simplify, double scaleFactor)
    {
        XmlTextWriter xml = new XmlTextWriter(writer);
        WriteXml(xml, simplify ? UnitAction.Simplify : UnitAction.Normalize, scaleFactor);
    }

    public void WriteXml(TextWriter writer, bool simplify)
    {
        XmlTextWriter xml = new XmlTextWriter(writer);
        WriteXml(xml, simplify ? UnitAction.Simplify : UnitAction.Normalize, 1.0);
    }

    public void WriteXml(XmlWriter xml, bool simplify, double scaleFactor)
    {
        WriteXml(xml, simplify ? UnitAction.Simplify : UnitAction.Normalize, scaleFactor);
    }

    public void WriteXml(XmlWriter xml, bool simplify)
    {
        WriteXml(xml, simplify ? UnitAction.Simplify : UnitAction.Normalize, 1.0);
    }

    public virtual string XML(bool simplify)
    {
        StringWriter stringWriter = new StringWriter();
        XmlTextWriter xml = new XmlTextWriter(stringWriter);
        WriteXml(xml, simplify ? UnitAction.Simplify : UnitAction.Normalize, 1.0);
        return stringWriter.ToString();
    }

    public virtual void Save(IStream stream)
    {
        throw new Exception("Not supported in .net 8");
    }

    public virtual void Load(IStream stream)
    {
        throw new Exception("Not supported in .net 8");
    }
}
