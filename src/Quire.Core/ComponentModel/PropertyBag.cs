#nullable disable
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing.Design;
using Quire.Mirror;

namespace Quire.ComponentModel;

[MirrorOf("Flobbster.Windows.Forms.IInspectable, Ptc.Core")]
public interface IInspectable
{
    PropertyBag Properties { get; }
}

[MirrorOf("Flobbster.Windows.Forms.PropertySpec, Ptc.Core")]
public class PropertySpec
{
    private Attribute[] attributes;

    private string category;

    private object defaultValue;

    private string description;

    private string editor;

    private string name;

    private string type;

    private string typeConverter;

    public Attribute[] Attributes
    {
        get
        {
            return attributes;
        }
        set
        {
            attributes = value;
        }
    }

    public string Category
    {
        get
        {
            return category;
        }
        set
        {
            category = value;
        }
    }

    public string ConverterTypeName
    {
        get
        {
            return typeConverter;
        }
        set
        {
            typeConverter = value;
        }
    }

    public object DefaultValue
    {
        get
        {
            return defaultValue;
        }
        set
        {
            defaultValue = value;
        }
    }

    public string Description
    {
        get
        {
            return description;
        }
        set
        {
            description = value;
        }
    }

    public string EditorTypeName
    {
        get
        {
            return editor;
        }
        set
        {
            editor = value;
        }
    }

    public string Name
    {
        get
        {
            return name;
        }
    }

    public string TypeName
    {
        get
        {
            return type;
        }
        set
        {
            type = value;
        }
    }

    public PropertySpec(string name, Type type)
        : this(name, type.AssemblyQualifiedName, null, null, null)
    {
    }

    public PropertySpec(string name, Type type, string category, string description, object defaultValue)
        : this(name, type.AssemblyQualifiedName, category, description, defaultValue)
    {
    }

    public PropertySpec(string name, string type, string category, string description, object defaultValue)
    {
        this.name = name;
        this.type = type;
        this.category = category;
        this.description = description;
        this.defaultValue = defaultValue;
        attributes = null;
    }
}

[MirrorOf("Flobbster.Windows.Forms.PropertySpecEventArgs, Ptc.Core")]
public class PropertySpecEventArgs : EventArgs
{
    private PropertySpec property;

    private object val;

    public PropertySpec Property => property;

    public object Value
    {
        get
        {
            return val;
        }
        set
        {
            val = value;
        }
    }

    public PropertySpecEventArgs(PropertySpec property, object val)
    {
        this.property = property;
        this.val = val;
    }
}

[MirrorOf("Flobbster.Windows.Forms.PropertySpecEventHandler, Ptc.Core")]
public delegate void PropertySpecEventHandler(object sender, PropertySpecEventArgs e);

[MirrorOf("Flobbster.Windows.Forms.PropertyBag, Ptc.Core")]
public class PropertyBag : ICustomTypeDescriptor
{
    [Serializable]
    [MirrorOf("Flobbster.Windows.Forms.PropertyBag+PropertySpecCollection, Ptc.Core")]
    public class PropertySpecCollection : IList, ICollection, IEnumerable
    {
        private ArrayList innerArray;

        public int Count => innerArray.Count;

        public bool IsFixedSize => false;

        public bool IsReadOnly => false;

        public bool IsSynchronized => false;

        object ICollection.SyncRoot => null;

        public virtual PropertySpec this[int index]
        {
            get
            {
                return (PropertySpec)innerArray[index];
            }
            set
            {
                innerArray[index] = value;
            }
        }

        object IList.this[int index]
        {
            get => this[index];
            set => this[index] = (PropertySpec)value;
        }

        public PropertySpecCollection()
        {
            innerArray = new ArrayList();
        }

        public int Add(PropertySpec value)
        {
            return innerArray.Add(value);
        }

        public void AddRange(PropertySpec[] array) => innerArray.AddRange(array);

        public void Clear() => innerArray.Clear();

        public bool Contains(PropertySpec item) => innerArray.Contains(item);

        public void CopyTo(PropertySpec[] array) => innerArray.CopyTo(array);

        public void CopyTo(PropertySpec[] array, int index) => innerArray.CopyTo(array, index);

        public int IndexOf(PropertySpec value) => innerArray.IndexOf(value);

        public void Insert(int index, PropertySpec value) => innerArray.Insert(index, value);

        public void Remove(PropertySpec value) => innerArray.Remove(value);

        public void Remove(string name)
        {
            int index = IndexOf(name);
            RemoveAt(index);
        }

        public void RemoveAt(int index) => innerArray.RemoveAt(index);

        public PropertySpec[] ToArray() => (PropertySpec[])innerArray.ToArray(typeof(PropertySpec));

        void ICollection.CopyTo(Array array, int index) => CopyTo((PropertySpec[])array, index);

        int IList.Add(object value) => Add((PropertySpec)value);

        bool IList.Contains(object value) => Contains((PropertySpec)value);

        int IList.IndexOf(object value) => IndexOf((PropertySpec)value);

        void IList.Insert(int index, object value) => Insert(index, (PropertySpec)value);

        void IList.Remove(object value) => Remove((PropertySpec)value);

        public int IndexOf(string name)
        {
            int num = 0;
            foreach (PropertySpec item in innerArray)
            {
                if (item.Name == name)
                {
                    return num;
                }
                num++;
            }
            return -1;
        }

        public bool Contains(string name)
        {
            foreach (PropertySpec item in innerArray)
            {
                if (item.Name == name)
                {
                    return true;
                }
            }
            return false;
        }

        public IEnumerator GetEnumerator()
        {
            return innerArray.GetEnumerator();
        }
    }

    [MirrorOf("Flobbster.Windows.Forms.PropertyBag+PropertySpecDescriptor, Ptc.Core")]
    private sealed class SpecDescriptor : PropertyDescriptor
    {
        private PropertyBag bag;

        private PropertySpec item;

        public override Type ComponentType => item.GetType();

        public override bool IsReadOnly => Attributes.Matches(ReadOnlyAttribute.Yes);

        public override Type PropertyType => Type.GetType(item.TypeName);

        public SpecDescriptor(PropertySpec item, PropertyBag bag, string name, Attribute[] attrs)
            : base(name, attrs)
        {
            this.bag = bag;
            this.item = item;
        }

        public override bool CanResetValue(object component)
        {
            if (item.DefaultValue == null)
            {
                return false;
            }
            return !GetValue(component).Equals(item.DefaultValue);
        }

        public override object GetValue(object component)
        {
            PropertySpecEventArgs e = new PropertySpecEventArgs(item, null);
            bag.OnGetValue(e);
            return e.Value;
        }

        public override void ResetValue(object component)
        {
            SetValue(component, item.DefaultValue);
        }

        public override void SetValue(object component, object value)
        {
            PropertySpecEventArgs e = new PropertySpecEventArgs(item, value);
            bag.OnSetValue(e);
        }

        public override bool ShouldSerializeValue(object component)
        {
            object value = GetValue(component);
            if (item.DefaultValue == null && value == null)
            {
                return false;
            }
            return !value.Equals(item.DefaultValue);
        }
    }

    private string defaultProperty;

    private PropertySpecCollection properties;

    public string DefaultProperty
    {
        get
        {
            return defaultProperty;
        }
        set
        {
            defaultProperty = value;
        }
    }

    public PropertySpecCollection Properties => properties;

    public event PropertySpecEventHandler GetValue;

    public event PropertySpecEventHandler SetValue;

    public PropertyBag()
    {
        defaultProperty = null;
        properties = new PropertySpecCollection();
    }

    protected virtual void OnGetValue(PropertySpecEventArgs e)
    {
        if (GetValue != null)
        {
            GetValue(this, e);
        }
    }

    protected virtual void OnSetValue(PropertySpecEventArgs e)
    {
        if (SetValue != null)
        {
            SetValue(this, e);
        }
    }

    AttributeCollection ICustomTypeDescriptor.GetAttributes()
    {
        return TypeDescriptor.GetAttributes(this, noCustomTypeDesc: true);
    }

    string ICustomTypeDescriptor.GetClassName()
    {
        return TypeDescriptor.GetClassName(this, noCustomTypeDesc: true);
    }

    string ICustomTypeDescriptor.GetComponentName()
    {
        return TypeDescriptor.GetComponentName(this, noCustomTypeDesc: true);
    }

    TypeConverter ICustomTypeDescriptor.GetConverter()
    {
        return TypeDescriptor.GetConverter(this, noCustomTypeDesc: true);
    }

    EventDescriptor ICustomTypeDescriptor.GetDefaultEvent()
    {
        return TypeDescriptor.GetDefaultEvent(this, noCustomTypeDesc: true);
    }

    PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty()
    {
        PropertySpec propertySpec = null;
        if (defaultProperty != null)
        {
            int index = properties.IndexOf(defaultProperty);
            propertySpec = properties[index];
        }
        if (propertySpec != null)
        {
            return new SpecDescriptor(propertySpec, this, propertySpec.Name, null);
        }
        return null;
    }

    object ICustomTypeDescriptor.GetEditor(Type editorBaseType)
    {
        return TypeDescriptor.GetEditor(this, editorBaseType, noCustomTypeDesc: true);
    }

    EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
    {
        return TypeDescriptor.GetEvents(this, noCustomTypeDesc: true);
    }

    EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes)
    {
        return TypeDescriptor.GetEvents(this, attributes, noCustomTypeDesc: true);
    }

    PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
    {
        return ((ICustomTypeDescriptor)this).GetProperties(new Attribute[0]);
    }

    PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes)
    {
        ArrayList arrayList = new ArrayList();
        foreach (PropertySpec property in properties)
        {
            ArrayList arrayList2 = new ArrayList();
            if (property.Category != null)
            {
                arrayList2.Add(new CategoryAttribute(property.Category));
            }
            if (property.Description != null)
            {
                arrayList2.Add(new DescriptionAttribute(property.Description));
            }
            if (property.EditorTypeName != null)
            {
                arrayList2.Add(new EditorAttribute(property.EditorTypeName, typeof(UITypeEditor)));
            }
            if (property.ConverterTypeName != null)
            {
                arrayList2.Add(new TypeConverterAttribute(property.ConverterTypeName));
            }
            if (property.Attributes != null)
            {
                arrayList2.AddRange(property.Attributes);
            }
            Attribute[] attrs = (Attribute[])arrayList2.ToArray(typeof(Attribute));
            SpecDescriptor value = new SpecDescriptor(property, this, property.Name, attrs);
            arrayList.Add(value);
        }
        return new PropertyDescriptorCollection((PropertyDescriptor[])arrayList.ToArray(typeof(PropertyDescriptor)));
    }

    object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd)
    {
        return this;
    }

}

[MirrorOf("Flobbster.Windows.Forms.PropertyTable, Ptc.Core")]
public class PropertyTable : PropertyBag
{
    private Hashtable propValues;

    public object this[string key]
    {
        get
        {
            return propValues[key];
        }
        set
        {
            propValues[key] = value;
        }
    }

    public PropertyTable()
    {
        propValues = new Hashtable();
    }

    protected override void OnGetValue(PropertySpecEventArgs e)
    {
        e.Value = propValues[e.Property.Name];
        base.OnGetValue(e);
    }

    protected override void OnSetValue(PropertySpecEventArgs e)
    {
        propValues[e.Property.Name] = e.Value;
        base.OnSetValue(e);
    }
}
