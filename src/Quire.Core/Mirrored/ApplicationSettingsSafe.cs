#nullable disable
#pragma warning disable

namespace Quire;

partial class ApplicationSettingsSafe
{
    public override object this[string propertyName]
    {
        get
        {
            if (FallbackValues.TryGetValue(propertyName, out var value))
            {
                return value;
            }

            try
            {
                return base[propertyName];
            }
            catch (global::System.Configuration.ConfigurationErrorsException)
            {
                global::System.Reflection.PropertyInfo property = GetType().GetProperty(propertyName);
                if (property != null)
                {
                    string text = (global::System.Reflection.CustomAttributeExtensions.GetCustomAttribute(property, typeof(global::System.Configuration.DefaultSettingValueAttribute)) as global::System.Configuration.DefaultSettingValueAttribute)?.Value;
                    if (text is not null)
                    {
                        global::System.ComponentModel.TypeConverter converter = global::System.ComponentModel.TypeDescriptor.GetConverter(property.PropertyType);
                        if (converter.CanConvertFrom(text.GetType()))
                        {
                            FallbackValues[propertyName] = value = converter.ConvertFrom(text);
                        }
                    }
                }

                return value;
            }
        }
        set
        {
            try
            {
                base[propertyName] = value;
                FallbackValues.Remove(propertyName);
            }
            catch (global::System.Configuration.ConfigurationErrorsException)
            {
                FallbackValues[propertyName] = value;
            }
        }
    }
    public override void Save()
    {
        try
        {
            base.Save();
            DebugTools.DevAssert(FallbackValues.IsNullOrEmpty(), null);
        }
        catch (global::System.Configuration.ConfigurationErrorsException)
        {
        }
    }

    public override void Upgrade()
    {
        try
        {
            base.Upgrade();
            FallbackValues.Clear();
        }
        catch (global::System.Configuration.ConfigurationErrorsException ex)
        {
            DebugTools.FailAssertion(ex);
        }
    }
}
