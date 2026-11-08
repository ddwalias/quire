#nullable disable
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using Quire.FunctionalitiesLimitation;
using Quire.Services;
using Quire.Mirror;
using Quire.Wpf;

namespace Quire.Controls.Core;

public partial class WorksheetPageLayout
{
    [MirrorVerified("PtcControlsCoreWorksheetPageLayoutMechanicalDifferentialTests")]
    private static Thickness WideMargin(Thickness startingMargin) => ScaledMargin(startingMargin, 2.0, 2.0);

    [MirrorVerified("PtcControlsCoreWorksheetPageLayoutMechanicalDifferentialTests")]
    private static Thickness NarrowMargin(Thickness startingMargin) => ScaledMargin(startingMargin, 0.5, 0.5);

    [MirrorVerified("PtcControlsCoreWorksheetPageLayoutMechanicalDifferentialTests")]
    private static double MetricMajorGridLines => WpfUtils.CmToDips(2.0);

    [MirrorVerified("PtcControlsCoreWorksheetPageLayoutMechanicalDifferentialTests")]
    private static double USMajorGridLines => WpfUtils.InchesToDips(1.0);

    [MirrorVerified("PtcControlsCoreWorksheetPageLayoutMechanicalDifferentialTests")]
    private static Thickness USFooterMargin => USHeaderMargin;

    [MirrorVerified("PtcControlsCoreWorksheetPageLayoutMechanicalDifferentialTests")]
    private static Thickness MetricFooterMargin => MetricHeaderMargin;

    private Color _pageBackgroundColor = BackgroundTypeColors.Paper;

    [MirrorVerified("PtcControlsCoreWorksheetPageLayoutMechanicalDifferentialTests")]
    public event Action<Size, Size> GridCellSizeChanged = delegate { };

    [MirrorVerified("PtcControlsCoreWorksheetPageLayoutMechanicalDifferentialTests")]
    public event Action<Size, Size> HeaderFooterGridCellSizeChanged = delegate { };

    [MirrorVerified("PtcControlsCoreWorksheetPageLayoutMechanicalDifferentialTests")]
    public event Action WorksheetRecreateSuggested;

    [MirrorVerified("PtcControlsCoreWorksheetPageLayoutMechanicalDifferentialTests")]
    public event PropertyChangedEventHandler PropertyChanged;

    [MirrorVerified("PtcControlsCoreWorksheetPageLayoutMechanicalDifferentialTests")]
    private static Size MetricGridCellSize => new Size(WpfUtils.CmToDips(0.5), WpfUtils.CmToDips(0.5));

    [MirrorVerified("PtcControlsCoreWorksheetPageLayoutMechanicalDifferentialTests")]
    private static Size MetricFineGridCellSize => ScaledSize(MetricGridCellSize, 0.5, 0.5);

    [MirrorVerified("PtcControlsCoreWorksheetPageLayoutMechanicalDifferentialTests")]
    private static Size MetricHeaderFooterGridCellSize => ScaledSize(MetricGridCellSize, 0.5, 0.5);

    public WorksheetPageLayout()
    {
        PageSize defaultPageSizeForLocale = GetDefaultPageSizeForLocale();
        PageSizeInformation pageSizeInformation = BuiltInPageSizes[defaultPageSizeForLocale];
        MarginType = PageMarginType.Standard;
        PageMargin = pageSizeInformation.PageMargins[MarginType];
        HeaderMargin = pageSizeInformation.HeaderMargin;
        FooterMargin = pageSizeInformation.FooterMargin;
        PageSize = defaultPageSizeForLocale;
    }

    [MirrorVerified("PtcControlsCoreWorksheetPageLayoutMechanicalDifferentialTests")]
    private static Size USGridCellSize => new Size(WpfUtils.InchesToDips(0.2), WpfUtils.InchesToDips(0.2));

    [MirrorVerified("PtcControlsCoreWorksheetPageLayoutMechanicalDifferentialTests")]
    private static Size USFineGridCellSize => ScaledSize(USGridCellSize, 0.5, 0.5);

    [MirrorVerified("PtcControlsCoreWorksheetPageLayoutMechanicalDifferentialTests")]
    private static Size USHeaderFooterGridCellSize => ScaledSize(USGridCellSize, 0.5, 0.5);

    [MirrorVerified("PtcControlsCoreWorksheetPageLayoutMechanicalDifferentialTests")]
    private static Thickness StandardMetricMargin => new Thickness(WpfUtils.CmToDips(2.5));

    [MirrorVerified("PtcControlsCoreWorksheetPageLayoutMechanicalDifferentialTests")]
    private static Thickness StandardUSMargin => new Thickness(WpfUtils.InchesToDips(1.0));

    [MirrorVerified("PtcControlsCoreWorksheetPageLayoutMechanicalDifferentialTests")]
    private static Thickness LegacyMargin => new Thickness(WpfUtils.InchesToDips(1.2));

    [MirrorVerified("PtcControlsCoreWorksheetPageLayoutMechanicalDifferentialTests")]
    private static Thickness USHeaderMargin
    {
        get
        {
            double horizontal = WpfUtils.InchesToDips(0.25);
            double vertical = WpfUtils.InchesToDips(0.125);
            return new Thickness(horizontal, vertical, horizontal, vertical);
        }
    }

    [MirrorVerified("PtcControlsCoreWorksheetPageLayoutMechanicalDifferentialTests")]
    private static Thickness MetricHeaderMargin
    {
        get
        {
            double horizontal = WpfUtils.CmToDips(0.5);
            double vertical = WpfUtils.CmToDips(0.25);
            return new Thickness(horizontal, vertical, horizontal, vertical);
        }
    }

    [MirrorVerified("PtcControlsCoreWorksheetPageLayoutMechanicalDifferentialTests")]
    private static Point MetricInitialInsertionPoint => new Point(0.0, MetricGridCellSize.Height);

    [MirrorVerified("PtcControlsCoreWorksheetPageLayoutMechanicalDifferentialTests")]
    private static Point USInitialInsertionPoint => new Point(0.0, USGridCellSize.Height);

    public double PageBodyContentWidth
    {
        get => _pageBodyContentWidth;
        set
        {
            _pageBodyContentWidth = value;
            OnPropertyChanged("PageBodyContentWidth");
        }
    }

    public double PageBodyWidth
    {
        get => _pageBodyWidth;
        internal set
        {
            _pageBodyWidth = value;
            OnPropertyChanged("PageBodyWidth");
        }
    }

    public Color PageBackgroundColor
    {
        get => _pageBackgroundColor;
        set
        {
            _pageBackgroundColor = value;
            OnPropertyChanged("PageBackgroundColor");
        }
    }

    public double MajorGridLines
    {
        get => _majorGridLines;
        private set
        {
            if (_majorGridLines == value)
            {
                return;
            }
            _majorGridLines = value;
            OnPropertyChanged("MajorGridLines");
        }
    }

    public double HeaderBodyWidth
    {
        get => _headerBodyWidth;
        private set
        {
            if (_headerBodyWidth == value)
            {
                return;
            }
            _headerBodyWidth = value;
            OnPropertyChanged("HeaderBodyWidth");
        }
    }

    public double HeaderBodyHeight
    {
        get => _headerBodyHeight;
        private set
        {
            if (_headerBodyHeight == value)
            {
                return;
            }
            _headerBodyHeight = value;
            OnPropertyChanged("HeaderBodyHeight");
        }
    }

    public double FooterBodyWidth
    {
        get => _footerBodyWidth;
        private set
        {
            if (_footerBodyWidth == value)
            {
                return;
            }
            _footerBodyWidth = value;
            OnPropertyChanged("FooterBodyWidth");
        }
    }

    public double FooterBodyHeight
    {
        get => _footerBodyHeight;
        private set
        {
            if (_footerBodyHeight == value)
            {
                return;
            }
            _footerBodyHeight = value;
            OnPropertyChanged("FooterBodyHeight");
        }
    }

    public PageSizeUnits PageSizeUnits
    {
        get => _pageSizeUnits;
        internal set
        {
            if (value == _pageSizeUnits)
            {
                return;
            }
            _pageSizeUnits = value;
                OnPropertyChanged("PageSizeUnits");
        }
    }

    public Point InitialInsertionPoint => BuiltInPageSizes[PageSize].InitialInsertionPoint;

    public GridSize GridSize
    {
        get => _gridSize;
        set
        {
            if (_gridSize != value)
            {
                _gridSize = value;
                PageSizeInformation pageSizeInformation = BuiltInPageSizes[PageSize];
                GridCellSize = GridSize == GridSize.Fine ? pageSizeInformation.FineGridCellSize : pageSizeInformation.GridCellSize;
                WorksheetRecreateSuggested?.Invoke();
                OnPropertyChanged("GridSize");
            }
        }
    }

    public PageOrientation PageOrientation
    {
        get => _pageOrientation;
        private set
        {
            if (_pageOrientation != value)
            {
                _pageOrientation = value;
                double pageHeight = PageHeight;
                PageHeight = PageWidth;
                PageWidth = pageHeight;
                OnPropertyChanged("PageOrientation");
            }
        }
    }

    public double PageBodyHeight
    {
        get => _pageBodyHeight;
        internal set
        {
            double num = Math.Round(value / GridCellSize.Height);
            double num2 = num * GridCellSize.Height;
            if (!num2.AreSame(_pageBodyHeight))
            {
                _pageBodyHeight = num2;
                OnPropertyChanged("PageBodyHeight");
            }
        }
    }

    public double PageHeight
    {
        get => _pageHeight;
        internal set
        {
            _pageHeight = value;
            PageBodyHeight = PageHeight - (PageMargin.Top + PageMargin.Bottom);
            DisplayHeight = PageHeight + GridCellSize.Height;
            OnPropertyChanged("PageHeight");
        }
    }

    public double DisplayHeight
    {
        get => _displayHeight;
        private set
        {
            if (DisplayHeight == value)
            {
                return;
            }
            _displayHeight = value;
            OnPropertyChanged("DisplayHeight");
        }
    }

    public double PageWidth
    {
        get => _pageWidth;
        internal set
        {
            _pageWidth = value;
            PageBodyWidth = PageWidth - (PageMargin.Left + PageMargin.Right);
            HeaderWidth = PageWidth;
            FooterWidth = PageWidth;
            OnPropertyChanged("PageWidth");
        }
    }

    public Thickness PageMargin
    {
        get => _pageMargin;
        internal set
        {
            if (_pageMargin != value)
            {
                _pageMargin = value;
                PageBodyWidth = PageWidth - (PageMargin.Left + PageMargin.Right);
                PageBodyHeight = PageHeight - (PageMargin.Top + PageMargin.Bottom);
                HeaderHeight = PageMargin.Top;
                FooterHeight = PageMargin.Bottom;
                OnPropertyChanged("PageMargin");
            }
        }
    }

    public Thickness HeaderMargin
    {
        get => _headerMargin;
        private set
        {
            if (_headerMargin != value)
            {
                _headerMargin = value;
                HeaderBodyWidth = HeaderWidth - (HeaderMargin.Left + HeaderMargin.Right);
                HeaderBodyHeight = HeaderHeight - (HeaderMargin.Top + HeaderMargin.Bottom);
                OnPropertyChanged("HeaderMargin");
            }
        }
    }

    public Thickness FooterMargin
    {
        get => _footerMargin;
        private set
        {
            if (_footerMargin != value)
            {
                _footerMargin = value;
                FooterBodyWidth = FooterWidth - (FooterMargin.Left + FooterMargin.Right);
                FooterBodyHeight = FooterHeight - (FooterMargin.Top + FooterMargin.Bottom);
                OnPropertyChanged("FooterMargin");
            }
        }
    }

    public double HeaderWidth
    {
        get => _headerWidth;
        private set
        {
            if (_headerWidth == value)
            {
                return;
            }
            _headerWidth = value;
            HeaderBodyWidth = HeaderWidth - (HeaderMargin.Left + HeaderMargin.Right);
            OnPropertyChanged("HeaderWidth");
        }
    }

    public double HeaderHeight
    {
        get => _headerHeight;
        private set
        {
            if (_headerHeight == value)
            {
                return;
            }
            _headerHeight = value;
            HeaderBodyHeight = HeaderHeight - (HeaderMargin.Top + HeaderMargin.Bottom);
            OnPropertyChanged("HeaderHeight");
        }
    }

    public double FooterWidth
    {
        get => _footerWidth;
        private set
        {
            if (_footerWidth == value)
            {
                return;
            }
            _footerWidth = value;
            FooterBodyWidth = FooterWidth - (FooterMargin.Left + FooterMargin.Right);
            OnPropertyChanged("FooterWidth");
        }
    }

    public double FooterHeight
    {
        get => _footerHeight;
        private set
        {
            if (_footerHeight == value)
            {
                return;
            }
            _footerHeight = value;
            FooterBodyHeight = FooterHeight - (FooterMargin.Top + FooterMargin.Bottom);
            OnPropertyChanged("FooterHeight");
        }
    }

    public PageMarginType MarginType
    {
        get => _marginType;
        private set
        {
            if (_marginType != value)
            {
                _marginType = value;
                if (_marginType != PageMarginType.Custom)
                {
                    PageMargin = BuiltInPageSizes[PageSize].PageMargins[_marginType];
                }
                OnPropertyChanged("MarginType");
            }
        }
    }

    public PageSize PageSize
    {
        get => _pageSize;
        private set
        {
            if (_pageSize != value)
            {
                _pageSize = value;
                PageSizeInformation pageSizeInformation = BuiltInPageSizes[PageSize];
                if (MarginType != PageMarginType.Custom)
                {
                    PageMargin = pageSizeInformation.PageMargins[MarginType];
                }
                GridCellSize = GridSize == GridSize.Fine ? pageSizeInformation.FineGridCellSize : pageSizeInformation.GridCellSize;
                HeaderMargin = pageSizeInformation.HeaderMargin;
                FooterMargin = pageSizeInformation.FooterMargin;
                HeaderFooterGridCellSize = pageSizeInformation.HeaderFooterGridCellSize;
                MajorGridLines = pageSizeInformation.MajorGridLines;
                if (PageOrientation == PageOrientation.Portrait)
                {
                    PageWidth = pageSizeInformation.Size.Width;
                    PageHeight = pageSizeInformation.Size.Height;
                }
                else
                {
                    PageWidth = pageSizeInformation.Size.Height;
                    PageHeight = pageSizeInformation.Size.Width;
                }
                PageSizeUnits = PageLayoutConstants.GetPageSizeUnitsByPageSize(_pageSize);
                OnPropertyChanged("PageSize");
            }
        }
    }

    public Size GridCellSize
    {
        get => _gridCellSize;
        internal set
        {
            if (_gridCellSize == value)
            {
                return;
            }
            Size gridCellSize = GridCellSize;
            _gridCellSize = value;
            PageBodyHeight = PageHeight - (PageMargin.Top + PageMargin.Bottom);
            DisplayHeight = PageHeight + GridCellSize.Height;
            OnSizeChanged("GridCellSize", gridCellSize, _gridCellSize);
        }
    }

    public Size HeaderFooterGridCellSize
    {
        get => _headerFooterGridCellSize;
        internal set
        {
            if (_headerFooterGridCellSize == value)
            {
                return;
            }
            Size headerFooterGridCellSize = HeaderFooterGridCellSize;
            _headerFooterGridCellSize = value;
            OnSizeChanged("HeaderFooterGridCellSize", headerFooterGridCellSize, _headerFooterGridCellSize);
        }
    }

    public int LinesPerPage
    {
        get
        {
            double pageBodyHeight = PageBodyHeight;
            Size gridCellSize = GridCellSize;
            return (int)Math.Round(pageBodyHeight / gridCellSize.Height);
        }
    }

    public static PageSize GetDefaultPageSizeForLocale()
    {
        PageSize result = PageSize.A4;
        switch (Thread.CurrentThread.CurrentUICulture.LCID)
        {
            case 1033:
            case 2058:
            case 3084:
            case 4105:
            case 8202:
            case 9226:
            case 11274:
            case 13321:
            case 13322:
                result = PageSize.Letter;
                break;
        }
        return result;
    }

    public bool IsPageSizeAllowed(PageSize pageSize)
    {
        if (!FunctionalityService.Instance.IsFunctionalityEnabled(EnumFunctionalityKey.SetPageSizeCommand) && (pageSize == PageSize.A1 || pageSize == PageSize.A2))
        {
            return false;
        }
        return true;
    }

    public void SetMarginType(PageMarginType marginType)
    {
        PageMarginType marginType2 = MarginType;
        MarginType = marginType;
        if (marginType2 != MarginType)
        {
            WorksheetRecreateSuggested?.Invoke();
        }
    }

    public void SetCustomMargin(Thickness customMargins)
    {
        PageMargin = customMargins;
        MarginType = PageMarginType.Custom;
        DebugTools.DevAssert(PageMargin == customMargins, null);
    }

    public void SetPageMarginInPageSizeUnits(Thickness marginInPageSizeUnits)
    {
        if (!FunctionalityService.Instance.IsFunctionalityEnabled(EnumFunctionalityKey.SetCustomMargins))
        {
            return;
        }
        Thickness pageMargin = PageMargin;
        SetPageMarginInDipsInternal(PageSizeUnitsToDipsMargins(marginInPageSizeUnits));
        if (pageMargin != PageMargin)
        {
            WorksheetRecreateSuggested?.Invoke();
        }
    }

    private void SetPageMarginInDipsInternal(Thickness marginInDips)
    {
        PageMarginType marginTypeOfMarginByPageSize = GetMarginTypeOfMarginByPageSize(PageSize, PageOrientation, marginInDips, out var actualMargin);
        if (marginTypeOfMarginByPageSize == PageMarginType.Custom)
        {
            SetCustomMargin(actualMargin);
        }
        else
        {
            MarginType = marginTypeOfMarginByPageSize;
        }
    }

    public void SetPageMarginInDips(Thickness marginInDips)
    {
        Thickness pageMargin = PageMargin;
        SetPageMarginInDipsInternal(marginInDips);
        if (pageMargin != PageMargin)
        {
            WorksheetRecreateSuggested?.Invoke();
        }
    }

    public Thickness PageSizeUnitsToDipsMargins(Thickness marginsInPageSizeUnits)
    {
        return new Thickness(PageSizeUnitsToDips(marginsInPageSizeUnits.Left), PageSizeUnitsToDips(marginsInPageSizeUnits.Top), PageSizeUnitsToDips(marginsInPageSizeUnits.Right), PageSizeUnitsToDips(marginsInPageSizeUnits.Bottom));
    }

    private double PageSizeUnitsToDips(double numInPageSizeUnits)
    {
        if (PageSizeUnits != PageSizeUnits.Inches)
        {
            return WpfUtils.CmToDips(numInPageSizeUnits);
        }
        return WpfUtils.InchesToDips(numInPageSizeUnits);
    }

    public Size ContentAreaSize()
    {
        return new Size(PageBodyWidth, PageBodyHeight);
    }

    object ICloneable.Clone()
    {
        return Clone();
    }

    public WorksheetPageLayout Clone()
    {
        return new WorksheetPageLayout().CloneFrom(this);
    }

    private WorksheetPageLayout CloneFrom(WorksheetPageLayout from)
    {
        _pageOrientation = from._pageOrientation;
        _pageMargin = from._pageMargin;
        _headerMargin = from._headerMargin;
        _footerMargin = from._footerMargin;
        _marginType = from._marginType;
        _gridCellSize = from._gridCellSize;
        _headerFooterGridCellSize = from._headerFooterGridCellSize;
        _pageSize = from._pageSize;
        _pageWidth = from._pageWidth;
        _pageHeight = from._pageHeight;
        _pageBodyWidth = from._pageBodyWidth;
        _pageBodyHeight = from._pageBodyHeight;
        _majorGridLines = from._majorGridLines;
        _displayHeight = from._displayHeight;
        _gridSize = from._gridSize;
        _pageBodyContentWidth = from._pageBodyContentWidth;
        _headerWidth = from._headerWidth;
        _headerHeight = from._headerHeight;
        _headerBodyWidth = from._headerBodyWidth;
        _headerBodyHeight = from._headerBodyHeight;
        _footerWidth = from._footerWidth;
        _footerHeight = from._footerHeight;
        _footerBodyWidth = from._footerBodyWidth;
        _footerBodyHeight = from._footerBodyHeight;
        PageSizeUnits = from.PageSizeUnits;
        return this;
    }

    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void OnSizeChanged(string propertyName, Size oldSize, Size newSize)
    {
        switch (propertyName)
        {
            case "GridCellSize":
                GridCellSizeChanged(oldSize, newSize);
                break;
            case "HeaderFooterGridCellSize":
                HeaderFooterGridCellSizeChanged(oldSize, newSize);
                break;
            default:
                DebugTools.FailAssertion("Unexpected property name: " + propertyName);
                break;
        }
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs<Size>(propertyName, oldSize, newSize));
    }
}
