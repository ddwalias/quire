#nullable disable
#pragma warning disable

namespace Quire.PersistentDataObjects;

partial interface IRegionType
{
    string regionid { get; set; }
    double height { get; }
    bool heightSpecified { get; }
    double width { get; }
    bool widthSpecified { get; }
    double actualWidth { get; set; }
    bool actualWidthSpecified { get; set; }
    double actualHeight { get; set; }
    bool actualHeightSpecified { get; set; }
    string[] mc14conversionlossannotations { get; set; }
    object Item { get; set; }
    bool mc14conversionloss { get; }
    bool ConvertedFromMC14 { get; }
    string Tag { get; set; }
    bool ShowBorder { get; set; }
    void SetHeight(double newHeight) ;
    void SetWidth(double newWidth) ;
}
