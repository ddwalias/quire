#nullable disable
#pragma warning disable

namespace Quire.Xml;

static partial class SchemasManager
{

    partial class SchemaData
    {
        public static implicit operator string(SchemaData data)
        {
            if (data == null)
            {
                return null;
            }

            return data.SchemaName;
        }

        public global::System.Xml.Schema.XmlSchema Schema
        {
            get
            {
                if (_schema == null)
                {
                    global::System.IO.Stream stream = global::Quire.Serialization.SpiritResolver.Instance.GetStream(SchemaName);
                    _schema = global::System.Xml.Schema.XmlSchema.Read(stream, null);
                }
                return _schema;
            }
        }
        public global::System.Xml.Schema.XmlSchemaSet CompiledSchemaSet
        {
            get
            {
                if (_schemaSet == null)
                {
                    _schemaSet = new global::System.Xml.Schema.XmlSchemaSet { XmlResolver = global::Quire.Serialization.SpiritResolver.Instance };
                    _schemaSet.Add(Schema);
                    _schemaSet.Compile();
                }
                return _schemaSet;
            }
        }
    }
}
