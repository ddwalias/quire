#nullable disable
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
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
}
