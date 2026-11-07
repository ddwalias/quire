#nullable disable
#pragma warning disable

using System.Collections.Generic;
using System.Xml;
using Quire.Controls.Core;
using Quire.Controls.Core.Serialization;
using Quire.PersistentDataObjects;
using Quire.Serialization;
using Quire;

namespace Quire.Controls.Text;

internal partial class RichTextSerializationStrategy
{
    private readonly IRichTextWithEmbeddedItem _richText;
    private readonly DelegateFunction0<IRegionType> _regionCreator;

    public RichTextSerializationStrategy(IRichTextWithEmbeddedItem richText, DelegateFunction0<IRegionType> regionCreator)
    {
        _richText = richText;
        _regionCreator = regionCreator;
    }

    public void SerializeRegionsEpilog(XmlDocument xmlDocument)
    {
    }

    public IList<IRegionType> GetRegions(IS11NProvider s11NProvider)
    {
        IList<IRegionType> regions = new List<IRegionType>();
        foreach (IWhiteboardItem item in _richText.WhiteboardItems)
        {
            IRegionType region = ConvertToRegionPersistantData(s11NProvider, item as IWhiteboardItemInText);
            if (region != null)
            {
                regions.Add(region);
            }
        }
        return regions.Count <= 0 ? null : regions;
    }

    protected IRegionType ConvertToRegionPersistantData(IS11NProvider s11NProvider, IWhiteboardItemInText item)
    {
        IRegionInTextType regionInText = _richText.RegionFactory.ConvertToIRegionType(_regionCreator, s11NProvider, item as System.Windows.UIElement) as IRegionInTextType;
        if (regionInText == null)
        {
            DebugTools.DevAssert(false);
            return null;
        }
        int offset = _richText.GetEmbeddedItemSymbolOffset(item);
        regionInText.textcaretposition = offset.ToString();
        return regionInText;
    }
}
