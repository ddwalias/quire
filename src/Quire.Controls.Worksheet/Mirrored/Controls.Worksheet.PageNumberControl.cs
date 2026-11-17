using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Quire.Controls.Core;
using Quire.PersistentData;
using Quire.Serialization;
using Quire.Wpf;

namespace Quire.Controls.Worksheet;

public partial class PageNumberControl
{
    public bool ClearTextForXamlSerialization
    {
        get
        {
            return true;
        }
        set
        {
            if (DebugTools.DevAssert(value, null).Passed)
            {
                ClearValue(TextBox.TextProperty);
            }
        }
    }

    public override string TemplateFormat
    {
        get
        {
            return base.TemplateFormat;
        }
        set
        {
            if (!IsValidTemplateKey(value))
            {
                value = GetTemplateKeyFromGivenFormat(value);
            }
            TemplateKey = value;
            base.TemplateFormat = GetPageNumberFormat(value);
        }
    }

    public string TemplateKey { get; private set; }

    private static void HandleTagChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
        if (args.NewValue == null || DebugTools.DevAssert(args.NewValue is string, null).Failed)
        {
            return;
        }
        string text = (string)args.NewValue;
        PageNumberControl pageNumberControl = (PageNumberControl)obj;
        if (text != pageNumberControl.Text)
        {
            pageNumberControl.InvalidateMeasure();
        }
    }

    protected override Size MeasureOverride(Size constraint)
    {
        if (Tag is string text)
        {
            Text = text;
        }
        return base.MeasureOverride(constraint);
    }

    public override IList<IRegionPersistentData> SetRegionPersistentData(IDeS11NProvider d11NProvider, object data)
    {
        if (data is fieldType fieldType2)
        {
            HeaderAndFooterResources instance = ResourceProvider<HeaderAndFooterResources>.Instance;
            string key = fieldTypePageNumber.EnumSerializationValues[((fieldTypePageNumber)fieldType2.Item).template.ToString()];
            TemplateFormat = (string)instance[key];
            base.SetRegionPersistentData(d11NProvider, data);
            return null;
        }
        throw new ArgumentException("should be of fieldType");
    }

    private static string GetPageNumberFormat(string template)
    {
        StringBuilder stringBuilder = new StringBuilder();
        int placeholderIndex = 0;
        foreach (char character in template)
        {
            if (character == '#')
            {
                stringBuilder.AppendFormat("{{{0}}}", placeholderIndex++);
            }
            else
            {
                stringBuilder.Append(character);
            }
        }
        return stringBuilder.ToString();
    }

    private string GetTemplateKeyFromGivenFormat(string format)
    {
        StringBuilder stringBuilder = new StringBuilder();
        return stringBuilder.AppendFormat(format, "#", "#").ToString();
    }
}
