#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using Quire.FunctionalitiesLimitation;
using Quire.Mirror;
using Quire.Services;
using Quire.Wpf;

namespace Quire.Controls.Core;

public partial class WorksheetPageLayout
{
    public static partial class PropertyNames
    {
        public const string DisplayHeight = "DisplayHeight";
        public const string HeaderWidth = "HeaderWidth";
        public const string HeaderHeight = "HeaderHeight";
        public const string HeaderBodyWidth = "HeaderBodyWidth";
        public const string HeaderBodyHeight = "HeaderBodyHeight";
        public const string HeaderMargin = "HeaderMargin";
        public const string FooterWidth = "FooterWidth";
        public const string FooterHeight = "FooterHeight";
        public const string FooterBodyWidth = "FooterBodyWidth";
        public const string FooterBodyHeight = "FooterBodyHeight";
        public const string FooterMargin = "FooterMargin";
        public const string PageBodyWidth = "PageBodyWidth";
        public const string PageBodyHeight = "PageBodyHeight";
        public const string PageBodyContentWidth = "PageBodyContentWidth";
        public const string PageWidth = "PageWidth";
        public const string PageHeight = "PageHeight";
        public const string PageBackgroundColor = "PageBackgroundColor";
        public const string PageOrientation = "PageOrientation";
        public const string GridSize = "GridSize";
        public const string PageSize = "PageSize";
        public const string PageSizeUnits = "PageSizeUnits";
        public const string MarginType = "MarginType";
        public const string HeaderFooterGridCellSize = "HeaderFooterGridCellSize";
        public const string MajorGridLines = "MajorGridLines";
        public const string PageMargin = "PageMargin";
        public const string GridCellSize = "GridCellSize";
    }

    private PageOrientation _pageOrientation;
    private Thickness _pageMargin;
    private Thickness _headerMargin;
    private Thickness _footerMargin;
    private PageMarginType _marginType;
    private Size _gridCellSize;
    private Size _headerFooterGridCellSize;
    private PageSize _pageSize;
    private PageSizeUnits _pageSizeUnits;
    private double _pageWidth;
    private double _pageHeight;
    private double _pageBodyWidth;
    private double _pageBodyHeight;
    private Color _pageBackgroundColor = BackgroundTypeColors.Paper;
    private double _majorGridLines;
    private double _displayHeight;
    private GridSize _gridSize;
    private double _pageBodyContentWidth;
    private double _headerWidth;
    private double _headerHeight;
    private double _headerBodyWidth;
    private double _headerBodyHeight;
    private double _footerWidth;
    private double _footerHeight;
    private double _footerBodyWidth;
    private double _footerBodyHeight;

    static WorksheetPageLayout()
    {
        DraftModeGutterWidthInDips = 19.2;
        BuiltInPageSizes = new Dictionary<PageSize, PageSizeInformation>();
        MarginEpsilon = 0.96;
        SetupDefaultPageSizeInformation();
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
    public event Action<Size, Size> GridCellSizeChanged = delegate { };

    [MirrorVerified("PtcControlsCoreWorksheetPageLayoutMechanicalDifferentialTests")]
    public event Action<Size, Size> HeaderFooterGridCellSizeChanged = delegate { };

    [MirrorVerified("PtcControlsCoreWorksheetPageLayoutMechanicalDifferentialTests")]
    public event Action WorksheetRecreateSuggested;

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

    public double PageBodyHeight
    {
        get => _pageBodyHeight;
        internal set
        {
            double num = Math.Round(value / GridCellSize.Height);
            double pageBodyHeight = num * GridCellSize.Height;
            if (!pageBodyHeight.AreSame(_pageBodyHeight))
            {
                _pageBodyHeight = pageBodyHeight;
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

    public Color PageBackgroundColor
    {
        get => _pageBackgroundColor;
        set
        {
            _pageBackgroundColor = value;
            OnPropertyChanged("PageBackgroundColor");
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

    public int LinesPerPage
    {
        get
        {
            double pageBodyHeight = PageBodyHeight;
            Size gridCellSize = GridCellSize;
            return (int)Math.Round(pageBodyHeight / gridCellSize.Height);
        }
    }

    // Declared between .ctor and UpdatePropertyAndDelayNotification to preserve closure ordinals 29_x, 122_0, 124_0, and 125_0.
    public static readonly double DraftModeGutterWidthInDips;

    private void UpdatePropertyAndDelayNotification(Action<WorksheetPageLayout> updateAction)
    {
        WorksheetPageLayout worksheetPageLayout = Clone();
        using (new PropertyChangeUpdateMonitor(worksheetPageLayout, FirePropertyChanged))
        {
            updateAction(worksheetPageLayout);
            CloneFrom(worksheetPageLayout);
        }
        void FirePropertyChanged(IDictionary<string, (object oldValue, object newValue)> props)
        {
            foreach (KeyValuePair<string, (object, object)> prop in props)
            {
                if (prop.Value.Item1 == null && prop.Value.Item2 == null)
                {
                    OnPropertyChanged(prop.Key);
                }
                else if (prop.Value.Item1 is Size oldSize && prop.Value.Item2 is Size newSize)
                {
                    OnSizeChanged(prop.Key, oldSize, newSize);
                }
                else
                {
                    string text = prop.Value.Item1 == null ? "None" : prop.Value.Item1.GetType().Name;
                    DebugTools.FailAssertion("Value of property " + prop.Key + " is of unexpected type: " + text);
                    OnPropertyChanged(prop.Key);
                }
            }
        }
    }

    public bool IsPageSizeAllowed(PageSize pageSize)
    {
        if (!FunctionalityService.Instance.IsFunctionalityEnabled(EnumFunctionalityKey.SetPageSizeCommand) && (pageSize == PageSize.A1 || pageSize == PageSize.A2))
        {
            return false;
        }
        return true;
    }

    public void SetPageSize(PageSize pageSize, bool updateCustomMargins)
    {
        if (!IsPageSizeAllowed(pageSize))
        {
            pageSize = PageSize.A3;
        }
        PageSize pageSize2 = PageSize;
        if (updateCustomMargins)
        {
            UpdatePropertyAndDelayNotification(UpdateAction);
        }
        else
        {
            PageSize = pageSize;
        }
        if (pageSize2 != PageSize)
        {
            WorksheetRecreateSuggested?.Invoke();
        }
        void UpdateAction(WorksheetPageLayout objectToUpdate)
        {
            objectToUpdate.PageSize = pageSize;
            if (objectToUpdate.MarginType == PageMarginType.Custom)
            {
                objectToUpdate.SetPageMarginInDipsInternal(objectToUpdate.PageMargin);
            }
        }
    }

    public void SetPageOrientation(PageOrientation pageOrientation, bool updateCustomMargins)
    {
        PageOrientation pageOrientation2 = PageOrientation;
        if (updateCustomMargins)
        {
            UpdatePropertyAndDelayNotification(UpdateAction);
        }
        else
        {
            PageOrientation = pageOrientation;
        }
        if (pageOrientation2 != PageOrientation)
        {
            WorksheetRecreateSuggested?.Invoke();
        }
        void UpdateAction(WorksheetPageLayout objectToUpdate)
        {
            objectToUpdate.PageOrientation = pageOrientation;
            if (objectToUpdate.MarginType == PageMarginType.Custom)
            {
                Thickness pageMarginInDipsInternal = pageOrientation == PageOrientation.Landscape ? new Thickness(PageMargin.Bottom, PageMargin.Left, PageMargin.Top, PageMargin.Right) : new Thickness(PageMargin.Top, PageMargin.Right, PageMargin.Bottom, PageMargin.Left);
                objectToUpdate.SetPageMarginInDipsInternal(pageMarginInDipsInternal);
            }
        }
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

    public Size ContentAreaSize() => new Size(PageBodyWidth, PageBodyHeight);

    object ICloneable.Clone() => Clone();

    public WorksheetPageLayout Clone() => new WorksheetPageLayout().CloneFrom(this);

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

    [MirrorVerified("PtcControlsCoreWorksheetPageLayoutMechanicalDifferentialTests")]
    public event PropertyChangedEventHandler PropertyChanged;

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

    // Declared after SetPageOrientation to preserve closure ordinals 29_x, 122_0, 124_0, and 125_0.
    public static Dictionary<PageSize, PageSizeInformation> BuiltInPageSizes;
    public static double MarginEpsilon;

    private static void SetupDefaultPageSizeInformation()
    {
        BuiltInPageSizes.Add(PageSize.A1, new PageSizeInformation
        {
            Size = new Size(WpfUtils.CmToDips(59.4), WpfUtils.CmToDips(84.1)),
            PageMargins = SetupMarginMap(NarrowMargin(StandardMetricMargin), StandardMetricMargin, WideMargin(StandardMetricMargin), LegacyMargin),
            HeaderMargin = MetricHeaderMargin,
            FooterMargin = MetricFooterMargin,
            GridCellSize = MetricGridCellSize,
            FineGridCellSize = MetricFineGridCellSize,
            HeaderFooterGridCellSize = MetricHeaderFooterGridCellSize,
            MajorGridLines = MetricMajorGridLines,
            InitialInsertionPoint = MetricInitialInsertionPoint
        });
        BuiltInPageSizes.Add(PageSize.A2, new PageSizeInformation
        {
            Size = new Size(WpfUtils.CmToDips(42.0), WpfUtils.CmToDips(59.4)),
            PageMargins = SetupMarginMap(NarrowMargin(StandardMetricMargin), StandardMetricMargin, WideMargin(StandardMetricMargin), LegacyMargin),
            HeaderMargin = MetricHeaderMargin,
            FooterMargin = MetricFooterMargin,
            GridCellSize = MetricGridCellSize,
            FineGridCellSize = MetricFineGridCellSize,
            HeaderFooterGridCellSize = MetricHeaderFooterGridCellSize,
            MajorGridLines = MetricMajorGridLines,
            InitialInsertionPoint = MetricInitialInsertionPoint
        });
        BuiltInPageSizes.Add(PageSize.A3, new PageSizeInformation
        {
            Size = new Size(WpfUtils.CmToDips(29.7), WpfUtils.CmToDips(42.0)),
            PageMargins = SetupMarginMap(NarrowMargin(StandardMetricMargin), StandardMetricMargin, WideMargin(StandardMetricMargin), LegacyMargin),
            HeaderMargin = MetricHeaderMargin,
            FooterMargin = MetricFooterMargin,
            GridCellSize = MetricGridCellSize,
            FineGridCellSize = MetricFineGridCellSize,
            HeaderFooterGridCellSize = MetricHeaderFooterGridCellSize,
            MajorGridLines = MetricMajorGridLines,
            InitialInsertionPoint = MetricInitialInsertionPoint
        });
        BuiltInPageSizes.Add(PageSize.A4, new PageSizeInformation
        {
            Size = new Size(WpfUtils.CmToDips(21.0), WpfUtils.CmToDips(29.7)),
            PageMargins = SetupMarginMap(NarrowMargin(StandardMetricMargin), StandardMetricMargin, WideMargin(StandardMetricMargin), LegacyMargin),
            HeaderMargin = MetricHeaderMargin,
            FooterMargin = MetricFooterMargin,
            GridCellSize = MetricGridCellSize,
            FineGridCellSize = MetricFineGridCellSize,
            HeaderFooterGridCellSize = MetricHeaderFooterGridCellSize,
            MajorGridLines = MetricMajorGridLines,
            InitialInsertionPoint = MetricInitialInsertionPoint
        });
        BuiltInPageSizes.Add(PageSize.A5, new PageSizeInformation
        {
            Size = new Size(WpfUtils.CmToDips(14.8), WpfUtils.CmToDips(21.0)),
            PageMargins = SetupMarginMap(NarrowMargin(StandardMetricMargin), StandardMetricMargin, WideMargin(StandardMetricMargin), LegacyMargin),
            HeaderMargin = MetricHeaderMargin,
            FooterMargin = MetricFooterMargin,
            GridCellSize = MetricGridCellSize,
            FineGridCellSize = MetricFineGridCellSize,
            HeaderFooterGridCellSize = MetricHeaderFooterGridCellSize,
            MajorGridLines = MetricMajorGridLines,
            InitialInsertionPoint = MetricInitialInsertionPoint
        });
        BuiltInPageSizes.Add(PageSize.Letter, new PageSizeInformation
        {
            Size = new Size(WpfUtils.InchesToDips(8.5), WpfUtils.InchesToDips(11.0)),
            PageMargins = SetupMarginMap(NarrowMargin(StandardUSMargin), StandardUSMargin, WideMargin(StandardUSMargin), LegacyMargin),
            HeaderMargin = USHeaderMargin,
            FooterMargin = USFooterMargin,
            GridCellSize = USGridCellSize,
            FineGridCellSize = USFineGridCellSize,
            HeaderFooterGridCellSize = USHeaderFooterGridCellSize,
            MajorGridLines = USMajorGridLines,
            InitialInsertionPoint = USInitialInsertionPoint
        });
        BuiltInPageSizes.Add(PageSize.Legal, new PageSizeInformation
        {
            Size = new Size(WpfUtils.InchesToDips(8.5), WpfUtils.InchesToDips(14.0)),
            PageMargins = SetupMarginMap(NarrowMargin(StandardUSMargin), StandardUSMargin, WideMargin(StandardUSMargin), LegacyMargin),
            HeaderMargin = USHeaderMargin,
            FooterMargin = USFooterMargin,
            GridCellSize = USGridCellSize,
            FineGridCellSize = USFineGridCellSize,
            HeaderFooterGridCellSize = USHeaderFooterGridCellSize,
            MajorGridLines = USMajorGridLines,
            InitialInsertionPoint = USInitialInsertionPoint
        });
        BuiltInPageSizes.Add(PageSize.Ledger, new PageSizeInformation
        {
            Size = new Size(WpfUtils.InchesToDips(11.0), WpfUtils.InchesToDips(17.0)),
            PageMargins = SetupMarginMap(NarrowMargin(StandardUSMargin), StandardUSMargin, WideMargin(StandardUSMargin), LegacyMargin),
            HeaderMargin = USHeaderMargin,
            FooterMargin = USFooterMargin,
            GridCellSize = USGridCellSize,
            FineGridCellSize = USFineGridCellSize,
            HeaderFooterGridCellSize = USHeaderFooterGridCellSize,
            MajorGridLines = USMajorGridLines,
            InitialInsertionPoint = USInitialInsertionPoint
        });
    }

    private static Dictionary<PageMarginType, Thickness> SetupMarginMap(Thickness narrowMargin, Thickness standardMargin, Thickness wideMargin, Thickness legacyMargin)
    {
        return new Dictionary<PageMarginType, Thickness>
        {
            [PageMarginType.Narrow] = narrowMargin,
            [PageMarginType.Standard] = standardMargin,
            [PageMarginType.Wide] = wideMargin,
            [PageMarginType.Legacy] = legacyMargin
        };
    }

    [MirrorVerified("PtcControlsCoreWorksheetPageLayoutMechanicalDifferentialTests")]
    private static Size MetricGridCellSize => new Size(WpfUtils.CmToDips(0.5), WpfUtils.CmToDips(0.5));

    [MirrorVerified("PtcControlsCoreWorksheetPageLayoutMechanicalDifferentialTests")]
    private static Size MetricFineGridCellSize => ScaledSize(MetricGridCellSize, 0.5, 0.5);

    [MirrorVerified("PtcControlsCoreWorksheetPageLayoutMechanicalDifferentialTests")]
    private static Size MetricHeaderFooterGridCellSize => ScaledSize(MetricGridCellSize, 0.5, 0.5);

    [MirrorVerified("PtcControlsCoreWorksheetPageLayoutMechanicalDifferentialTests")]
    private static Size USGridCellSize => new Size(WpfUtils.InchesToDips(0.2), WpfUtils.InchesToDips(0.2));

    [MirrorVerified("PtcControlsCoreWorksheetPageLayoutMechanicalDifferentialTests")]
    private static Size USFineGridCellSize => ScaledSize(USGridCellSize, 0.5, 0.5);

    [MirrorVerified("PtcControlsCoreWorksheetPageLayoutMechanicalDifferentialTests")]
    private static Size USHeaderFooterGridCellSize => ScaledSize(USGridCellSize, 0.5, 0.5);

    [MirrorVerified("PtcControlsCoreWorksheetPageLayoutMechanicalDifferentialTests")]
    private static double MetricMajorGridLines => WpfUtils.CmToDips(2.0);

    [MirrorVerified("PtcControlsCoreWorksheetPageLayoutMechanicalDifferentialTests")]
    private static double USMajorGridLines => WpfUtils.InchesToDips(1.0);

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
    private static Thickness USFooterMargin => USHeaderMargin;

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
    private static Thickness MetricFooterMargin => MetricHeaderMargin;

    [MirrorVerified("PtcControlsCoreWorksheetPageLayoutMechanicalDifferentialTests")]
    private static Thickness WideMargin(Thickness startingMargin) => ScaledMargin(startingMargin, 2.0, 2.0);

    [MirrorVerified("PtcControlsCoreWorksheetPageLayoutMechanicalDifferentialTests")]
    private static Thickness NarrowMargin(Thickness startingMargin) => ScaledMargin(startingMargin, 0.5, 0.5);

    private static Thickness ScaledMargin(Thickness startingMargin, double widthScale, double heightScale)
    {
        double left = startingMargin.Left * widthScale;
        double right = startingMargin.Right * widthScale;
        double top = startingMargin.Top * heightScale;
        double bottom = startingMargin.Bottom * heightScale;
        return new Thickness(left, top, right, bottom);
    }

    private static Size ScaledSize(Size size, double widthScale, double heightScale) => new Size(size.Width * widthScale, size.Height * heightScale);

    [MirrorVerified("PtcControlsCoreWorksheetPageLayoutMechanicalDifferentialTests")]
    private static Point MetricInitialInsertionPoint => new Point(0.0, MetricGridCellSize.Height);

    [MirrorVerified("PtcControlsCoreWorksheetPageLayoutMechanicalDifferentialTests")]
    private static Point USInitialInsertionPoint => new Point(0.0, USGridCellSize.Height);

    public static Size ContentAreaSize(Size pageSize, Thickness pageMargin)
    {
        return new Size(pageSize.Width - pageMargin.Left - pageMargin.Right, pageSize.Height - pageMargin.Top - pageMargin.Bottom);
    }

    public static Rect ContentArea(Size pageSize, Thickness pageMargin)
    {
        return new Rect(new Point(pageMargin.Left, pageMargin.Top), ContentAreaSize(pageSize, pageMargin));
    }

    public static PageMarginType GetMarginTypeOfMarginByPageSize(PageSize pagesize, PageOrientation orientation, Thickness margin, out Thickness actualMargin)
    {
        PageMarginType[] marginTypes = new PageMarginType[4]
        {
            PageMarginType.Standard,
            PageMarginType.Wide,
            PageMarginType.Narrow,
            PageMarginType.Legacy
        };
        foreach (PageMarginType pageMarginType in marginTypes)
        {
            Thickness thickness = BuiltInPageSizes[pagesize].PageMargins[pageMarginType];
            if (margin.IsEqualTo(thickness, MarginEpsilon))
            {
                actualMargin = thickness;
                return pageMarginType;
            }
        }
        actualMargin = CoercePageMargin(pagesize, orientation, margin);
        return PageMarginType.Custom;
    }

    public static Size GetMaxPageMarginsRestriction(WorksheetPageLayout pageLayout)
    {
        Size gridCellSize = BuiltInPageSizes[pageLayout.PageSize].GridCellSize;
        Size result = default;
        result.Width = pageLayout.PageWidth - 3.0 * gridCellSize.Width;
        result.Height = pageLayout.PageHeight - 3.0 * gridCellSize.Height;
        return result;
    }

    public static Thickness GetMinPageMarginsRestriction(WorksheetPageLayout pageLayout)
    {
        double vertical;
        double horizontal;
        if (pageLayout.PageSizeUnits == PageSizeUnits.Inches)
        {
            vertical = WpfUtils.InchesToDips(0.3);
            horizontal = WpfUtils.InchesToDips(0.25);
        }
        else
        {
            vertical = WpfUtils.CmToDips(0.6);
            horizontal = WpfUtils.CmToDips(0.5);
        }
        return new Thickness(horizontal, vertical, horizontal, vertical);
    }

    private static Thickness CoercePageMargin(PageSize pageSize, PageOrientation orientation, Thickness marginsToCoerce)
    {
        WorksheetPageLayout pageLayout = new WorksheetPageLayout
        {
            PageSize = pageSize,
            PageOrientation = orientation
        };
        Thickness minPageMarginsRestriction = GetMinPageMarginsRestriction(pageLayout);
        double left = Math.Max(marginsToCoerce.Left, minPageMarginsRestriction.Left);
        double right = Math.Max(marginsToCoerce.Right, minPageMarginsRestriction.Right);
        double top = Math.Max(marginsToCoerce.Top, minPageMarginsRestriction.Top);
        double bottom = Math.Max(marginsToCoerce.Bottom, minPageMarginsRestriction.Bottom);
        Size maxPageMarginsRestriction = GetMaxPageMarginsRestriction(pageLayout);
        if (maxPageMarginsRestriction.Width.IsLessThan(left + right))
        {
            left = Math.Min(left, maxPageMarginsRestriction.Width / 2.0);
            right = Math.Min(right, maxPageMarginsRestriction.Width / 2.0);
        }
        if (maxPageMarginsRestriction.Height.IsLessThan(top + bottom))
        {
            top = Math.Min(top, maxPageMarginsRestriction.Height / 2.0);
            bottom = Math.Min(bottom, maxPageMarginsRestriction.Height / 2.0);
        }
        return new Thickness(left, top, right, bottom);
    }
}
