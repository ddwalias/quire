#nullable disable
using Quire.Mirror;

namespace Quire.PersistentData;

public partial class cachedWorksheetRegionType
{
    [MirrorVerified("PtcPersistentDatacachedWorksheetRegionTypeMechanicalDifferentialTests")]
    public cachedWorksheetRegionType() { }

    [MirrorVerified("PtcPersistentDatacachedWorksheetRegionTypeMechanicalDifferentialTests")]
    private object itemField;

    [MirrorVerified("PtcPersistentDatacachedWorksheetRegionTypeMechanicalDifferentialTests")]
    public override object Item { get => itemField; set => itemField = value; }

    [MirrorVerified("PtcPersistentDatacachedWorksheetRegionTypeMechanicalDifferentialTests")]
    private string[] mc14conversionlossannotationsField;

    [MirrorVerified("PtcPersistentDatacachedWorksheetRegionTypeMechanicalDifferentialTests")]
    public override string[] mc14conversionlossannotations { get => mc14conversionlossannotationsField; set => mc14conversionlossannotationsField = value; }

    partial class headersContainer
    {
        [MirrorVerified("PtcPersistentDatacachedWorksheetRegionTypeMechanicalDifferentialTests")]
        public headersContainer() { }

        [MirrorVerified("PtcPersistentDatacachedWorksheetRegionTypeMechanicalDifferentialTests")]
        private headerFooterRegions headerField;

        [MirrorVerified("PtcPersistentDatacachedWorksheetRegionTypeMechanicalDifferentialTests")]
        public headerFooterRegions header { get => headerField; set => headerField = value; }

        [MirrorVerified("PtcPersistentDatacachedWorksheetRegionTypeMechanicalDifferentialTests")]
        private headerFooterRegions firstPageHeaderField;

        [MirrorVerified("PtcPersistentDatacachedWorksheetRegionTypeMechanicalDifferentialTests")]
        public headerFooterRegions firstPageHeader { get => firstPageHeaderField; set => firstPageHeaderField = value; }
    }

    partial class footersContainer
    {
        [MirrorVerified("PtcPersistentDatacachedWorksheetRegionTypeMechanicalDifferentialTests")]
        public footersContainer() { }

        [MirrorVerified("PtcPersistentDatacachedWorksheetRegionTypeMechanicalDifferentialTests")]
        private headerFooterRegions footerField;

        [MirrorVerified("PtcPersistentDatacachedWorksheetRegionTypeMechanicalDifferentialTests")]
        public headerFooterRegions footer { get => footerField; set => footerField = value; }

        [MirrorVerified("PtcPersistentDatacachedWorksheetRegionTypeMechanicalDifferentialTests")]
        private headerFooterRegions firstPageFooterField;

        [MirrorVerified("PtcPersistentDatacachedWorksheetRegionTypeMechanicalDifferentialTests")]
        public headerFooterRegions firstPageFooter { get => firstPageFooterField; set => firstPageFooterField = value; }
    }
}
