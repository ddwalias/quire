using System;
using System.Windows;
using Quire.Controls.ExcelComponent;
using Quire.Core;
using Quire.PersistentData;

namespace Quire.Controls.Worksheet;

public partial class OleObjectPersistentData : RegionPersistentData
{
    public OleObjectPersistentData(OleObjectControl oleObject)
        : base(oleObject)
    {
    }

    protected override void Deserialize()
    {
        oleObjectRegionType oleObjectRegionType2 = RegionData.Item as oleObjectRegionType;
        if (DebugTools.AssertNotNull(oleObjectRegionType2, null).Passed && (oleObjectRegionType2.OleViewportSize == null || (oleObjectRegionType2.OleViewportSize.width == 0.0 && oleObjectRegionType2.OleViewportSize.height == 0.0)))
        {
            DebugTools.DevAssert(oleObjectRegionType2.OleViewportSize != null || RegionData.ConvertedFromMC14, null);
            if (RegionData.widthSpecified && RegionData.heightSpecified)
            {
                oleObjectRegionType2.FakeOleViewportSize = new Size
                {
                    Width = RegionData.width,
                    Height = RegionData.height
                };
            }
            else
            {
                DebugTools.FailAssertion(null, Array.Empty<object>());
            }
        }

        base.Deserialize();
        DeserializeDimensions();
    }
}
