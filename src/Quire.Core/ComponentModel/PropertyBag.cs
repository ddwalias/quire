using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Quire.ComponentModel;

/// <summary>An object whose properties are defined at run time and shown through a <see cref="PropertyBag"/>.</summary>
public interface IInspectable
{
    PropertyBag Properties { get; }
}

/// <summary>One run-time property: its name, type and the metadata a property grid shows.</summary>
public class PropertySpec(string name, Type type, string? category = null, string? description = null, object? defaultValue = null)
{
    public string Name { get; } = name;
    public Type Type { get; } = type;
    public string? Category { get; set; } = category;
    public string? Description { get; set; } = description;
    public object? DefaultValue { get; set; } = defaultValue;

    /// <summary>Assembly-qualified name of a UITypeEditor, if the property has a custom editor.</summary>
    public string? EditorTypeName { get; set; }

    /// <summary>Assembly-qualified name of a TypeConverter, if the property needs one.</summary>
    public string? ConverterTypeName { get; set; }

    public Attribute[] Attributes { get; set; } = [];
}

public class PropertySpecEventArgs(PropertySpec property, object? value) : EventArgs
{
    public PropertySpec Property { get; } = property;
    public object? Value { get; set; } = value;
}

public delegate void PropertySpecEventHandler(object sender, PropertySpecEventArgs e);

/// <summary>
/// A type descriptor whose properties are <see cref="PropertySpec"/>s. Reads and writes are routed through the
/// <see cref="GetValue"/> and <see cref="SetValue"/> events, so the owner decides where values live.
/// </summary>
public class PropertyBag : CustomTypeDescriptor
{
    public sealed class PropertySpecCollection : Collection<PropertySpec>
    {
        public PropertySpec? this[string name] => this.FirstOrDefault(p => p.Name == name);
        public bool Contains(string name) => this[name] != null;
    }

    private readonly Dictionary<PropertySpec, PropertyDescriptor> _descriptors = [];

    public PropertySpecCollection Properties { get; } = [];
    public string? DefaultProperty { get; set; }

    public event PropertySpecEventHandler? GetValue;
    public event PropertySpecEventHandler? SetValue;

    protected virtual void OnGetValue(PropertySpecEventArgs e) => GetValue?.Invoke(this, e);
    protected virtual void OnSetValue(PropertySpecEventArgs e) => SetValue?.Invoke(this, e);

    public override PropertyDescriptorCollection GetProperties() => GetProperties(null);

    public override PropertyDescriptorCollection GetProperties(Attribute[]? attributes)
    {
        var descriptors = Properties.Select(Descriptor);
        if (attributes is { Length: > 0 }) descriptors = descriptors.Where(d => d.Attributes.Contains(attributes));
        return new([.. descriptors]);
    }

    /// <summary>One descriptor per spec, kept so value-changed subscriptions (e.g. from bindings) stay attached.</summary>
    private PropertyDescriptor Descriptor(PropertySpec spec)
    {
        if (!_descriptors.TryGetValue(spec, out var d)) _descriptors[spec] = d = new SpecDescriptor(this, spec);
        return d;
    }

    public override PropertyDescriptor? GetDefaultProperty() =>
        DefaultProperty == null ? null : GetProperties()[DefaultProperty];

    public override object? GetPropertyOwner(PropertyDescriptor? pd) => this;

    private sealed class SpecDescriptor(PropertyBag bag, PropertySpec spec) : PropertyDescriptor(spec.Name, Metadata(spec))
    {
        public override Type ComponentType => bag.GetType();
        public override Type PropertyType => spec.Type;
        public override bool IsReadOnly => Attributes.Matches(ReadOnlyAttribute.Yes);

        public override object? GetValue(object? component)
        {
            var e = new PropertySpecEventArgs(spec, spec.DefaultValue);
            bag.OnGetValue(e);
            return e.Value;
        }

        public override void SetValue(object? component, object? value)
        {
            bag.OnSetValue(new PropertySpecEventArgs(spec, value));
            if (component != null) OnValueChanged(component, EventArgs.Empty);
        }
        public override bool CanResetValue(object component) => spec.DefaultValue != null;
        public override void ResetValue(object component) => SetValue(component, spec.DefaultValue);
        public override bool ShouldSerializeValue(object component) => !Equals(GetValue(component), spec.DefaultValue);

        private static Attribute[] Metadata(PropertySpec spec)
        {
            var list = new List<Attribute>(spec.Attributes);
            if (spec.Category != null) list.Add(new CategoryAttribute(spec.Category));
            if (spec.Description != null) list.Add(new DescriptionAttribute(spec.Description));
            if (spec.EditorTypeName != null)
                list.Add(new EditorAttribute(spec.EditorTypeName, "System.Drawing.Design.UITypeEditor, System.Drawing.Design"));
            if (spec.ConverterTypeName != null) list.Add(new TypeConverterAttribute(spec.ConverterTypeName));
            return [.. list];
        }
    }
}

/// <summary>A <see cref="PropertyBag"/> that stores the values itself.</summary>
public class PropertyTable : PropertyBag
{
    private readonly Dictionary<string, object?> _values = [];

    public object? this[string name]
    {
        get => _values.TryGetValue(name, out var v) ? v : Properties[name]?.DefaultValue;
        set => _values[name] = value;
    }

    protected override void OnGetValue(PropertySpecEventArgs e)
    {
        e.Value = this[e.Property.Name];
        base.OnGetValue(e);
    }

    protected override void OnSetValue(PropertySpecEventArgs e)
    {
        this[e.Property.Name] = e.Value;
        base.OnSetValue(e);
    }
}
