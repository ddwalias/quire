#nullable disable
#pragma warning disable

namespace Quire.PersistentData;

partial class headersContainerWrapper
{
    public global::Quire.PersistentData.header Header
    {
        get
        {
            global::Quire.PersistentData.header header = new global::Quire.PersistentData.header();
            if (_container.header?.regions != null)
            {
                header.regions = _container.header.regions;
            }
            return header;
        }
        set
        {
            if (_container.header == null)
            {
                _container.header = new global::Quire.PersistentData.headerFooterRegions();
            }
            _container.header.regions = value?.regions;
        }
    }
    public global::Quire.PersistentData.header FirstPageHeader
    {
        get
        {
            global::Quire.PersistentData.header header = new global::Quire.PersistentData.header();
            if (_container.firstPageHeader?.regions != null)
            {
                header.regions = _container.firstPageHeader.regions;
            }
            return header;
        }
        set
        {
            if (_container.firstPageHeader == null)
            {
                _container.firstPageHeader = new global::Quire.PersistentData.headerFooterRegions();
            }
            _container.firstPageHeader.regions = value?.regions;
        }
    }
    public bool HeaderConvertedFromMC14
    {
        get
        {
            if (_container.header != null && _container.header.convertedfromSpecified)
            {
                return _container.header.convertedfrom == (global::Quire.PersistentDataObjects.applicationVersionType)0;
            }
            return false;
        }
    }
    public bool FirstPageHeaderConvertedFromMC14
    {
        get
        {
            if (_container.firstPageHeader != null && _container.firstPageHeader.convertedfromSpecified)
            {
                return _container.firstPageHeader.convertedfrom == (global::Quire.PersistentDataObjects.applicationVersionType)0;
            }
            return false;
        }
    }
}
