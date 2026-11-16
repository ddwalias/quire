using System;
using System.Collections.Generic;
using Quire.PersistentData;
using Quire.Serialization;

namespace Quire.Controls.Worksheet;

public partial class SavedDateControl
{
    public override object GetRegionPersistentData(IS11NProvider s11NProvider)
    {
        DebugTools.DevAssert(Enum.TryParse<dateSavedTemplate>(TemplateFormat, true, out var result), "failed to parse dateSaved template");
        return new fieldType
        {
            Item = new fieldTypeDateSaved
            {
                template = result,
                templateSpecified = true
            },
            ItemElementName = ItemChoiceType1.dateSaved,
            text = GetRegionTextPersistentData(s11NProvider)
        };
    }

    public override IList<IRegionPersistentData> SetRegionPersistentData(IDeS11NProvider d11NProvider, object data)
    {
        if (data is fieldType fieldType2)
        {
            TemplateFormat = ((fieldTypeDateSaved)fieldType2.Item).template.ToString();
            base.SetRegionPersistentData(d11NProvider, data);
            return null;
        }
        throw new ArgumentException("should be of fieldType");
    }

    public SavedDateControl() { }
}
