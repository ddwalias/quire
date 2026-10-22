#nullable disable
#pragma warning disable

namespace Quire.PersistentData;

partial class footersContainerWrapper
{
    public global::Quire.PersistentData.footer Footer
    {
        get
        {
            global::Quire.PersistentData.footer footer = new global::Quire.PersistentData.footer();
            if (_container.footer?.regions != null)
            {
                footer.regions = _container.footer.regions;
            }
            return footer;
        }
        set
        {
            if (_container.footer == null)
            {
                _container.footer = new global::Quire.PersistentData.headerFooterRegions();
            }
            _container.footer.regions = value?.regions;
        }
    }
    public global::Quire.PersistentData.footer FirstPageFooter
    {
        get
        {
            global::Quire.PersistentData.footer footer = new global::Quire.PersistentData.footer();
            if (_container.firstPageFooter?.regions != null)
            {
                footer.regions = _container.firstPageFooter.regions;
            }
            return footer;
        }
        set
        {
            if (_container.firstPageFooter == null)
            {
                _container.firstPageFooter = new global::Quire.PersistentData.headerFooterRegions();
            }
            _container.firstPageFooter.regions = value?.regions;
        }
    }
    public bool FooterConvertedFromMC14
    {
        get
        {
            if (_container.footer != null && _container.footer.convertedfromSpecified)
            {
                return _container.footer.convertedfrom == (global::Quire.PersistentDataObjects.applicationVersionType)0;
            }
            return false;
        }
    }
    public bool FirstPageFooterConvertedFromMC14
    {
        get
        {
            if (_container.firstPageFooter != null && _container.firstPageFooter.convertedfromSpecified)
            {
                return _container.firstPageFooter.convertedfrom == (global::Quire.PersistentDataObjects.applicationVersionType)0;
            }
            return false;
        }
    }
}
