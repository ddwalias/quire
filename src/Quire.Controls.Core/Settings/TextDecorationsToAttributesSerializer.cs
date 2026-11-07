#nullable disable
using System;
using System.Collections.Generic;
using System.Windows;
using System.Xml.Linq;
using Quire.Controls.Core.Serialization;
using Quire.Mirror;

namespace Quire.Controls.Core.Settings;

public partial class TextDecorationsToAttributesSerializer : XElementSerializable
{
    private partial class TextDecorationsExistence
    {
        private TextDecorationCollection _collection;

        public bool Exists;

        public TextDecorationCollection Collection
        {
            get
            {
                return _collection;
            }
            private set
            {
                DebugTools.DevAssert(value != null && value.Count == 1, null);
                _collection = value;
            }
        }

        public TextDecorationsExistence(TextDecorationCollection collection, bool existence)
        {
            Collection = collection;
            Exists = existence;
        }

        public bool IsSameDecoration(TextDecoration decoration)
        {
            if (decoration != null && Collection[0] != null)
            {
                return object.Equals(decoration.Location, Collection[0].Location);
            }
            return false;
        }
    }

    private const string LocalName = "textDecorations";

    private const string Underline = "underline";

    private const string Overline = "overline";

    private const string Strikethrough = "strikethrough";

    private const string Baseline = "baseline";

    private readonly Dictionary<string, TextDecorationsExistence> _existingDecorations = new Dictionary<string, TextDecorationsExistence>
    {
        {
            "underline",
            new TextDecorationsExistence(TextDecorations.Underline, existence: false)
        },
        {
            "overline",
            new TextDecorationsExistence(TextDecorations.OverLine, existence: false)
        },
        {
            "strikethrough",
            new TextDecorationsExistence(TextDecorations.Strikethrough, existence: false)
        },
        {
            "baseline",
            new TextDecorationsExistence(TextDecorations.Baseline, existence: false)
        }
    };

    [MirrorVerified("PtcControlsCoreSettingsTextDecorationsToAttributesSerializerMechanicalDifferentialTests")]
    public TextDecorationCollection TextDecorationsCollection { get; set; }

    public bool HasUnderline
    {
        get
        {
            return _existingDecorations["underline"].Exists;
        }
        set
        {
            _existingDecorations["underline"].Exists = value;
        }
    }

    public bool HasOverLine
    {
        get
        {
            return _existingDecorations["overline"].Exists;
        }
        set
        {
            _existingDecorations["overline"].Exists = value;
        }
    }

    public bool HasStrikethrough
    {
        get
        {
            return _existingDecorations["strikethrough"].Exists;
        }
        set
        {
            _existingDecorations["strikethrough"].Exists = value;
        }
    }

    public bool HasBaseline
    {
        get
        {
            return _existingDecorations["baseline"].Exists;
        }
        set
        {
            _existingDecorations["baseline"].Exists = value;
        }
    }

    public TextDecorationsToAttributesSerializer(TextDecorationCollection textDecorations, XNamespace ns)
        : base(ns + "textDecorations")
    {
        TextDecorationsCollection = textDecorations;
    }

    public override void ReadElementContents(XElement xElement)
    {
        base.ReadElementContents(xElement);
        TextDecorationCollection textDecorationCollection = new TextDecorationCollection();
        foreach (KeyValuePair<string, TextDecorationsExistence> existingDecoration in _existingDecorations)
        {
            if (existingDecoration.Value.Exists)
            {
                textDecorationCollection.Add(existingDecoration.Value.Collection);
            }
        }
        TextDecorationsCollection = textDecorationCollection;
    }

    public override void WriteElementContents(XElement xElement)
    {
        if (TextDecorationsCollection != null)
        {
            foreach (TextDecoration item in TextDecorationsCollection)
            {
                bool condition = false;
                foreach (KeyValuePair<string, TextDecorationsExistence> existingDecoration in _existingDecorations)
                {
                    if (existingDecoration.Value.IsSameDecoration(item))
                    {
                        existingDecoration.Value.Exists = condition = true;
                        break;
                    }
                }
                DebugTools.DevAssert(condition, null);
            }
            base.WriteElementContents(xElement);
        }
    }

    protected override void CreateChildren(XElement thisElement = null)
    {
        SerializationInfo serializationInfo = new SerializationInfo
        {
            Flags = SerializationFlags.DONT_SERIALIZE_DEFAULT_VALUE,
            DefaultValue = false
        };
        XmlBooleanConverter typeConverter = new XmlBooleanConverter();
        Children = new object[4]
        {
            new PropertySerializableToXmlAttribute(this, "HasUnderline", "underline", typeConverter, serializationInfo),
            new PropertySerializableToXmlAttribute(this, "HasOverLine", "overline", typeConverter, serializationInfo),
            new PropertySerializableToXmlAttribute(this, "HasStrikethrough", "strikethrough", typeConverter, serializationInfo),
            new PropertySerializableToXmlAttribute(this, "HasBaseline", "baseline", typeConverter, serializationInfo)
        };
    }
}
