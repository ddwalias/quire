using System;
using System.Diagnostics;
using System.IO;
using System.Xml;
using Quire.EfiSymbolicsEmitter;
using Quire.Mcd;
using Quire.Mpl.Run;

namespace Quire.Automation;

public partial class SymbolicValue : Value, Quire.AutomationPrivate.ISymbolicValue
{
    public override int Size => ExpressionSize(core);

    public override string Type => "Symbolic";

    public override string AsString => core.ToString();

    public SymbolicValue()
        : base(null)
    {
    }

    public SymbolicValue(object sym)
        : base(sym, RuntimeOptions.SaveThreadOptions())
    {
    }

    public SymbolicValue(object sym, RuntimeOptions o)
        : base(sym, o)
    {
    }

    internal override void WriteValueXml(XmlWriter xml, UnitAction action, double scaleFactor)
    {
        try
        {
            xml.WriteRaw(EmitToXMLString("ml", core));
        }
        catch (Exception ex)
        {
            xml.WriteStartElement("SymbolicValue", "MathcadAutomation");
            try
            {
                if (core != null)
                {
                    xml.WriteElementString("core", core.ToString());
                }
                xml.WriteElementString("Exception", "System", ex.ToString());
            }
            finally
            {
                xml.WriteEndElement();
            }
        }
    }

    internal override void WriteValueRL(TextWriter rl, UnitAction action)
    {
        try
        {
            rl.Write(EmitToRLString(core));
        }
        catch (Exception ex)
        {
            if (!Debugger.IsAttached)
            {
                if (ex.Message.Equals("Stack overflow"))
                {
                    Console.WriteLine(ex);
                }
                else
                {
                    Console.WriteLine(core);
                    Console.WriteLine(ex);
                }
            }
        }
    }

    private string EmitToXMLString(string prefix, object sym)
    {
        RuntimeOptions runtimeOptions = RuntimeOptions.SaveThreadOptions();
        try
        {
            m_options.SyncToThread();
            return (string)values.emitToXMLForAny(MathNamespaceUriVersion, sym);
        }
        finally
        {
            runtimeOptions.SyncToThread();
        }
    }

    private string EmitToRLString(object sym)
    {
        RuntimeOptions runtimeOptions = RuntimeOptions.SaveThreadOptions();
        try
        {
            m_options.SyncToThread();
            if (values.appDepth(490, sym) != null)
            {
                AppException.Raise("Stack overflow");
            }
            return (string)values.emitToRL(sym);
        }
        finally
        {
            runtimeOptions.SyncToThread();
        }
    }

    private int ExpressionSize(object sym)
    {
        RuntimeOptions runtimeOptions = RuntimeOptions.SaveThreadOptions();
        try
        {
            m_options.SyncToThread();
            return (int)values.exprSizeForAny(MathNamespaceUriVersion, sym);
        }
        finally
        {
            runtimeOptions.SyncToThread();
        }
    }
}
