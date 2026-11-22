#nullable disable
using System;
using System.Xml;
using System.Xml.Schema;
using Quire.Calculation.Core;
using Quire.Mirror;
using Quire.PersistentDataObjects;

namespace Quire.Controls.Core.Serialization;

public partial class NumberFormatSerializer
{
    private readonly NumberFormat _numberFormat;

    private readonly string _nameSpace;

    public const string roundTripDecimalPlaces = "round-trip-decimal-places";

    [MirrorVerified("PtcControlsCoreSerializationNumberFormatSerializerMechanicalDifferentialTests")]
    public bool NumberFormatWasRead { get; private set; }

    private NumberFormat.NumberStyles NumberStyle
    {
        get => _numberFormat.NumberStyle.Value;
        set => _numberFormat.NumberStyle = value;
    }

    private string ShowTrailingZeros
    {
        get => _numberFormat.ShowTrailingDecimalZeros.ToString().ToLower();
        set => _numberFormat.ShowTrailingDecimalZeros = bool.Parse(value);
    }

    private string Precision
    {
        get
        {
            switch (_numberFormat.DecimalPlaces)
            {
            case -1:
                return "round-trip-decimal-places";
            case null:
                return 3.ToString();
            default:
                return _numberFormat.DecimalPlaces.ToString();
            }
        }
        set
        {
            if (value != null)
            {
                if (value == "round-trip-decimal-places")
                {
                    _numberFormat.DecimalPlaces = -1;
                    return;
                }
                try
                {
                    _numberFormat.DecimalPlaces = int.Parse(value);
                }
                catch (ArgumentException)
                {
                    _numberFormat.DecimalPlaces = -1;
                }
            }
        }
    }

    private string ZeroThreshold
    {
        get => _numberFormat.ZeroThreshold.ToString();
        set => _numberFormat.ZeroThreshold = int.Parse(value);
    }

    private string ExponentialThreshold
    {
        get => _numberFormat.ExponentialThreshold.ToString();
        set => _numberFormat.ExponentialThreshold = int.Parse(value);
    }

    private string ComplexThreshold
    {
        get => _numberFormat.ComplexThreshold.ToString();
        set => _numberFormat.ComplexThreshold = int.Parse(value);
    }

    private string UseENotation
    {
        get => ExponentStyleIsENotation.ToString().ToLower();
        set => ExponentStyleIsENotation = value == "true";
    }

    private bool ExponentStyleIsENotation
    {
        get => _numberFormat.ExponentStyle == NumberFormat.ExponentStyles.ENotation;
        set => _numberFormat.ExponentStyle = value ? NumberFormat.ExponentStyles.ENotation : NumberFormat.ExponentStyles.MultiplyByBaseToAPower;
    }

    private string ImaginaryValue
    {
        get
        {
            switch (_numberFormat.ComplexStyle)
            {
            case NumberFormat.ComplexStyles.CartesianI:
                return "i";
            case NumberFormat.ComplexStyles.CartesianJ:
                return "j";
            case NumberFormat.ComplexStyles.PolarRadians:
                return "polarRadians";
            case NumberFormat.ComplexStyles.PolarPiRadians:
                return "polarPiRadians";
            case NumberFormat.ComplexStyles.PolarDegrees:
                return "polarDegrees";
            default:
                return "i";
            }
        }
        set
        {
            switch (value)
            {
            case "i":
                _numberFormat.ComplexStyle = NumberFormat.ComplexStyles.CartesianI;
                break;
            case "j":
                _numberFormat.ComplexStyle = NumberFormat.ComplexStyles.CartesianJ;
                break;
            case "polarRadians":
                _numberFormat.ComplexStyle = NumberFormat.ComplexStyles.PolarRadians;
                break;
            case "polarPiRadians":
                _numberFormat.ComplexStyle = NumberFormat.ComplexStyles.PolarPiRadians;
                break;
            case "polarDegrees":
                _numberFormat.ComplexStyle = NumberFormat.ComplexStyles.PolarDegrees;
                break;
            default:
                _numberFormat.ComplexStyle = NumberFormat.ComplexStyles.CartesianI;
                break;
            }
        }
    }

    private string RadixStyle
    {
        get
        {
            switch (_numberFormat.RadixStyle)
            {
            case NumberFormat.RadixStyles.Decimal:
                return "dec";
            case NumberFormat.RadixStyles.Binary:
                return "bin";
            case NumberFormat.RadixStyles.Octal:
                return "oct";
            case NumberFormat.RadixStyles.Hexadecimal:
                return "hex";
            default:
                DebugTools.FailAssertionDoNotThrow("NumberForamt.RadixStyle is not recognized");
                return null;
            }
        }
        set
        {
            switch (value)
            {
            case "dec":
                _numberFormat.RadixStyle = NumberFormat.RadixStyles.Decimal;
                break;
            case "bin":
                _numberFormat.RadixStyle = NumberFormat.RadixStyles.Binary;
                break;
            case "oct":
                _numberFormat.RadixStyle = NumberFormat.RadixStyles.Octal;
                break;
            case "hex":
                _numberFormat.RadixStyle = NumberFormat.RadixStyles.Hexadecimal;
                break;
            default:
                DebugTools.FailAssertionDoNotThrow("NumberForamt.RadixStyle is not recognized");
                break;
            }
        }
    }

    private resultFormatGeneralTypeImaginaryvalue ResultFormatImaginaryValue
    {
        get
        {
            switch (_numberFormat.ComplexStyle)
            {
            case NumberFormat.ComplexStyles.CartesianI:
                return resultFormatGeneralTypeImaginaryvalue.i;
            case NumberFormat.ComplexStyles.CartesianJ:
                return resultFormatGeneralTypeImaginaryvalue.j;
            case NumberFormat.ComplexStyles.PolarRadians:
                return resultFormatGeneralTypeImaginaryvalue.polarRadians;
            case NumberFormat.ComplexStyles.PolarPiRadians:
                return resultFormatGeneralTypeImaginaryvalue.polarPiRadians;
            case NumberFormat.ComplexStyles.PolarDegrees:
                return resultFormatGeneralTypeImaginaryvalue.polarDegrees;
            default:
                return resultFormatGeneralTypeImaginaryvalue.i;
            }
        }
        set
        {
            switch (value)
            {
            case resultFormatGeneralTypeImaginaryvalue.i:
                _numberFormat.ComplexStyle = NumberFormat.ComplexStyles.CartesianI;
                break;
            case resultFormatGeneralTypeImaginaryvalue.j:
                _numberFormat.ComplexStyle = NumberFormat.ComplexStyles.CartesianJ;
                break;
            case resultFormatGeneralTypeImaginaryvalue.polarRadians:
                _numberFormat.ComplexStyle = NumberFormat.ComplexStyles.PolarRadians;
                break;
            case resultFormatGeneralTypeImaginaryvalue.polarPiRadians:
                _numberFormat.ComplexStyle = NumberFormat.ComplexStyles.PolarPiRadians;
                break;
            case resultFormatGeneralTypeImaginaryvalue.polarDegrees:
                _numberFormat.ComplexStyle = NumberFormat.ComplexStyles.PolarDegrees;
                break;
            default:
                _numberFormat.ComplexStyle = NumberFormat.ComplexStyles.CartesianI;
                break;
            }
        }
    }

    public NumberFormatSerializer(NumberFormat numberFormat, string nameSpace)
    {
        _numberFormat = numberFormat;
        _nameSpace = nameSpace;
    }

    [MirrorVerified("PtcControlsCoreSerializationNumberFormatSerializerMechanicalDifferentialTests")]
    public XmlSchema GetSchema() => throw new NotImplementedException();

    public void ReadXml(XmlReader reader)
    {
        if (_numberFormat == null)
        {
            return;
        }
        NumberFormatWasRead = true;
        switch (reader.LocalName)
        {
        case "engineering":
            ReadEngineeringXml(reader);
            break;
        case "scientific":
            ReadScientificXml(reader);
            break;
        case "decimal":
            NumberStyle = NumberFormat.NumberStyles.Decimal;
            ReadDecimalXml(reader);
            break;
        case "fraction":
            ReadFractionalXml(reader);
            break;
        case "percent":
            ReadPercentXml(reader);
            break;
        case "general":
            ReadGeneralXml(reader);
            break;
        default:
            NumberFormatWasRead = false;
            return;
        }
        reader.Read();
    }

    private void ReadGeneralXml(XmlReader reader)
    {
        ReadPrecision(reader);
        ReadShowTrailingZeros(reader);
        ReadRadix(reader);
        ReadZeroThreshold(reader);
        ImaginaryValue = reader.GetAttribute("imaginary-value");
        ReadExponentialThreshold(reader);
        ReadComplexThreshold(reader);
    }

    private void ReadScientificXml(XmlReader reader)
    {
        NumberStyle = NumberFormat.NumberStyles.Scientific;
        ReadExpXml(reader);
    }

    private void ReadEngineeringXml(XmlReader reader)
    {
        NumberStyle = NumberFormat.NumberStyles.Engineering;
        ReadExpXml(reader);
    }

    private void ReadPercentXml(XmlReader reader)
    {
        NumberStyle = NumberFormat.NumberStyles.Percent;
        ReadPrecision(reader);
        ReadShowTrailingZeros(reader);
        reader.GetAttribute("radix");
        ImaginaryValue = reader.GetAttribute("imaginary-value");
        ReadZeroThreshold(reader);
        ReadComplexThreshold(reader);
    }

    private void ReadShowTrailingZeros(XmlReader reader)
    {
        ShowTrailingZeros = reader.GetAttribute("show-trailing-zeros");
    }

    private void ReadPrecision(XmlReader reader)
    {
        Precision = reader.GetAttribute("precision");
    }

    private void ReadZeroThreshold(XmlReader reader)
    {
        ZeroThreshold = reader.GetAttribute("zero-threshold") ?? 15.ToString();
    }

    private void ReadExponentialThreshold(XmlReader reader)
    {
        ExponentialThreshold = reader.GetAttribute("exponential-threshold");
    }

    private void ReadComplexThreshold(XmlReader reader)
    {
        ComplexThreshold = reader.GetAttribute("complex-threshold") ?? 10.ToString();
    }

    private void ReadFractionalXml(XmlReader reader)
    {
        ReadPrecision(reader);
        ReadShowTrailingZeros(reader);
        switch (reader.GetAttribute("radix"))
        {
        case "bin":
            NumberStyle = NumberFormat.NumberStyles.Binary;
            break;
        case "oct":
            NumberStyle = NumberFormat.NumberStyles.Octal;
            break;
        case "dec":
            NumberStyle = NumberFormat.NumberStyles.Decimal;
            break;
        case "hex":
            NumberStyle = NumberFormat.NumberStyles.Hexadecimal;
            break;
        }
        ImaginaryValue = reader.GetAttribute("imaginary-value");
        ReadZeroThreshold(reader);
        ReadComplexThreshold(reader);
    }

    private void ReadDecimalXml(XmlReader reader)
    {
        ReadPrecision(reader);
        ReadShowTrailingZeros(reader);
        ReadRadix(reader);
        ImaginaryValue = reader.GetAttribute("imaginary-value");
        ReadZeroThreshold(reader);
        ReadComplexThreshold(reader);
    }

    private void ReadExpXml(XmlReader reader)
    {
        UseENotation = reader.GetAttribute("use-e-notation");
        if (reader.MoveToAttribute("precision"))
        {
            ReadPrecision(reader);
        }
        ReadShowTrailingZeros(reader);
        reader.GetAttribute("radix");
        ImaginaryValue = reader.GetAttribute("imaginary-value");
        ReadZeroThreshold(reader);
        ReadComplexThreshold(reader);
    }

    private void ReadRadix(XmlReader reader)
    {
        RadixStyle = reader.GetAttribute("radix");
    }

    public void WriteXml(XmlWriter writer)
    {
        if (_numberFormat == null)
        {
            return;
        }
        switch (NumberStyle)
        {
        case NumberFormat.NumberStyles.Engineering:
            WriteEngineering(writer);
            break;
        case NumberFormat.NumberStyles.Scientific:
            WriteScientific(writer);
            break;
        case NumberFormat.NumberStyles.Decimal:
            WriteDecimal(writer);
            break;
        case NumberFormat.NumberStyles.Percent:
            WritePercent(writer);
            break;
        case NumberFormat.NumberStyles.General:
            WriteGeneral(writer);
            break;
        case NumberFormat.NumberStyles.Binary:
        case NumberFormat.NumberStyles.Octal:
        case NumberFormat.NumberStyles.Hexadecimal:
            break;
        }
    }

    private void WriteGeneral(XmlWriter writer)
    {
        writer.WriteStartElement("general", _nameSpace);
        writer.WriteAttributeString("precision", Precision);
        writer.WriteAttributeString("show-trailing-zeros", ShowTrailingZeros);
        writer.WriteAttributeString("radix", RadixStyle);
        writer.WriteAttributeString("zero-threshold", ZeroThreshold);
        writer.WriteAttributeString("imaginary-value", ImaginaryValue);
        writer.WriteAttributeString("exponential-threshold", ExponentialThreshold);
        writer.WriteAttributeString("complex-threshold", ComplexThreshold);
        writer.WriteEndElement();
    }

    private void WritePercent(XmlWriter writer)
    {
        writer.WriteStartElement("percent", _nameSpace);
        writer.WriteAttributeString("precision", Precision);
        writer.WriteAttributeString("show-trailing-zeros", ShowTrailingZeros);
        writer.WriteAttributeString("radix", "dec");
        writer.WriteAttributeString("imaginary-value", ImaginaryValue);
        writer.WriteEndElement();
    }

    private void WriteDecimal(XmlWriter writer)
    {
        writer.WriteStartElement("decimal", _nameSpace);
        writer.WriteAttributeString("precision", Precision);
        writer.WriteAttributeString("show-trailing-zeros", ShowTrailingZeros);
        writer.WriteAttributeString("radix", RadixStyle);
        writer.WriteAttributeString("imaginary-value", ImaginaryValue);
        writer.WriteEndElement();
    }

    private void WriteScientific(XmlWriter writer)
    {
        writer.WriteStartElement("scientific", _nameSpace);
        writer.WriteAttributeString("use-e-notation", UseENotation);
        writer.WriteAttributeString("precision", Precision);
        writer.WriteAttributeString("show-trailing-zeros", ShowTrailingZeros);
        writer.WriteAttributeString("radix", "dec");
        writer.WriteAttributeString("imaginary-value", ImaginaryValue);
        writer.WriteEndElement();
    }

    private void WriteEngineering(XmlWriter writer)
    {
        writer.WriteStartElement("engineering", _nameSpace);
        writer.WriteAttributeString("use-e-notation", UseENotation);
        writer.WriteAttributeString("precision", Precision);
        writer.WriteAttributeString("show-trailing-zeros", ShowTrailingZeros);
        writer.WriteAttributeString("radix", "dec");
        writer.WriteAttributeString("imaginary-value", ImaginaryValue);
        writer.WriteEndElement();
    }

    public resultFormatType GetDataObject(IResultManager resultManager)
    {
        resultFormatType resultFormatType2 = new resultFormatType();
        switch (NumberStyle)
        {
        case NumberFormat.NumberStyles.Engineering:
            resultFormatType2.Item = new resultFormatEngineeringType
            {
                useenotation = ExponentStyleIsENotation,
                precision = Precision,
                showtrailingzeros = _numberFormat.ShowTrailingDecimalZeros,
                radix = radixType.dec,
                imaginaryvalue = ResultFormatImaginaryValue
            };
            break;
        case NumberFormat.NumberStyles.Scientific:
            resultFormatType2.Item = new resultFormatScientificType
            {
                useenotation = ExponentStyleIsENotation,
                precision = Precision,
                showtrailingzeros = _numberFormat.ShowTrailingDecimalZeros,
                radix = radixType.dec,
                imaginaryvalue = ResultFormatImaginaryValue
            };
            break;
        case NumberFormat.NumberStyles.Decimal:
            resultFormatType2.Item = new resultFormatDecimalType
            {
                precision = Precision,
                showtrailingzeros = _numberFormat.ShowTrailingDecimalZeros,
                radix = (radixType)(_numberFormat.RadixStyle.HasValue ? _numberFormat.RadixStyle.Value : NumberFormat.RadixStyles.Decimal),
                imaginaryvalue = ResultFormatImaginaryValue
            };
            break;
        case NumberFormat.NumberStyles.Percent:
            resultFormatType2.Item = new resultFormatPercentType
            {
                precision = Precision,
                showtrailingzeros = _numberFormat.ShowTrailingDecimalZeros,
                radix = radixType.dec,
                imaginaryvalue = ResultFormatImaginaryValue
            };
            break;
        case NumberFormat.NumberStyles.General:
            resultFormatType2.Item = new resultFormatTypeGeneral
            {
                precision = Precision,
                showtrailingzeros = _numberFormat.ShowTrailingDecimalZeros,
                radix = (radixType)(_numberFormat.RadixStyle.HasValue ? _numberFormat.RadixStyle.Value : NumberFormat.RadixStyles.Decimal),
                zerothreshold = ZeroThreshold,
                imaginaryvalue = ResultFormatImaginaryValue,
                exponentialthreshold = ExponentialThreshold
            };
            break;
        default:
            resultFormatType2.Item = null;
            break;
        }
        return resultFormatType2;
    }

    public void SetDataObject(resultFormatType resultFormat, IResultManager resultManager)
    {
        if (_numberFormat == null || resultFormat == null || resultFormat.Item == null)
        {
            return;
        }
        NumberFormatWasRead = true;
        if (resultFormat.Item is resultFormatEngineeringType)
        {
            NumberStyle = NumberFormat.NumberStyles.Engineering;
            resultFormatEngineeringType engineering = resultFormat.Item as resultFormatEngineeringType;
            ExponentStyleIsENotation = engineering.useenotation;
            Precision = engineering.precision;
            _numberFormat.ShowTrailingDecimalZeros = engineering.showtrailingzeros;
            ResultFormatImaginaryValue = engineering.imaginaryvalue;
        }
        else if (resultFormat.Item is resultFormatScientificType)
        {
            NumberStyle = NumberFormat.NumberStyles.Scientific;
            resultFormatScientificType scientific = resultFormat.Item as resultFormatScientificType;
            ExponentStyleIsENotation = scientific.useenotation;
            Precision = scientific.precision;
            _numberFormat.ShowTrailingDecimalZeros = scientific.showtrailingzeros;
            ResultFormatImaginaryValue = scientific.imaginaryvalue;
        }
        else if (resultFormat.Item is resultFormatDecimalType)
        {
            NumberStyle = NumberFormat.NumberStyles.Decimal;
            resultFormatDecimalType decimalFormat = resultFormat.Item as resultFormatDecimalType;
            Precision = decimalFormat.precision;
            _numberFormat.ShowTrailingDecimalZeros = decimalFormat.showtrailingzeros;
            ResultFormatImaginaryValue = decimalFormat.imaginaryvalue;
            RadixStyle = decimalFormat.radix.ToString();
        }
        else if (resultFormat.Item is resultFormatPercentType)
        {
            NumberStyle = NumberFormat.NumberStyles.Percent;
            resultFormatPercentType percent = resultFormat.Item as resultFormatPercentType;
            Precision = percent.precision;
            _numberFormat.ShowTrailingDecimalZeros = percent.showtrailingzeros;
            ResultFormatImaginaryValue = percent.imaginaryvalue;
        }
        else if (resultFormat.Item is resultFormatGeneralType)
        {
            NumberStyle = NumberFormat.NumberStyles.General;
            resultFormatGeneralType general = resultFormat.Item as resultFormatGeneralType;
            Precision = general.precision;
            _numberFormat.ShowTrailingDecimalZeros = general.showtrailingzeros;
            ZeroThreshold = general.zerothreshold;
            ResultFormatImaginaryValue = general.imaginaryvalue;
            ExponentialThreshold = general.exponentialthreshold;
            RadixStyle = general.radix.ToString();
        }
        else
        {
            DebugTools.DevAssert(false, "resultFormat.Item is of unexpected type.");
            NumberFormatWasRead = false;
        }
    }
}
