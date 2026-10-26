#nullable disable
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using Quire.Controls.Core;
using Quire.Mirror;

namespace Quire.PersistentData;

public partial class WorksheetPersistentData
{
    [MirrorVerified("PtcPersistentDataWorksheetPersistentDataMechanicalDifferentialTests")]
    public Dictionary<string, ITextStyleObject> TextStyles { get; set; }

    [MirrorVerified("PtcPersistentDataWorksheetPersistentDataMechanicalDifferentialTests")]
    public string LayoutSize { get; set; }

    [MirrorVerified("PtcPersistentDataWorksheetPersistentDataMechanicalDifferentialTests")]
    public string GridSize { get; set; }

    [MirrorVerified("PtcPersistentDataWorksheetPersistentDataMechanicalDifferentialTests")]
    public string PageOrientation { get; set; }

    [MirrorVerified("PtcPersistentDataWorksheetPersistentDataMechanicalDifferentialTests")]
    public string MarginType { get; set; }

    [MirrorVerified("PtcPersistentDataWorksheetPersistentDataMechanicalDifferentialTests")]
    public Nullable<Thickness> PageMargin { get; set; }

    [MirrorVerified("PtcPersistentDataWorksheetPersistentDataMechanicalDifferentialTests")]
    public bool DisplayGrid { get; set; }

    [MirrorVerified("PtcPersistentDataWorksheetPersistentDataMechanicalDifferentialTests")]
    public bool IsSolveBlockLabelVisible { get; set; }

    [MirrorVerified("PtcPersistentDataWorksheetPersistentDataMechanicalDifferentialTests")]
    public bool DisplayHFGrid { get; set; }

    [MirrorVerified("PtcPersistentDataWorksheetPersistentDataMechanicalDifferentialTests")]
    public bool DisplayFrame { get; set; }

    [MirrorVerified("PtcPersistentDataWorksheetPersistentDataMechanicalDifferentialTests")]
    public bool DisplayHeaderFrame { get; set; }

    [MirrorVerified("PtcPersistentDataWorksheetPersistentDataMechanicalDifferentialTests")]
    public bool DisplayFooterFrame { get; set; }

    [MirrorVerified("PtcPersistentDataWorksheetPersistentDataMechanicalDifferentialTests")]
    public bool UseMajorGridLines { get; set; }

    [MirrorVerified("PtcPersistentDataWorksheetPersistentDataMechanicalDifferentialTests")]
    public bool UseHFMajorGridLines { get; set; }

    [MirrorVerified("PtcPersistentDataWorksheetPersistentDataMechanicalDifferentialTests")]
    public BackgroundTypeE PlotBackgroundType { get; set; }

    [MirrorVerified("PtcPersistentDataWorksheetPersistentDataMechanicalDifferentialTests")]
    public Color PageBackgroundColor { get; set; }

    [MirrorVerified("PtcPersistentDataWorksheetPersistentDataMechanicalDifferentialTests")]
    public int StartingPageNumber { get; set; }

    [MirrorVerified("PtcPersistentDataWorksheetPersistentDataMechanicalDifferentialTests")]
    public int TotalPagesOffset { get; set; }

    [MirrorVerified("PtcPersistentDataWorksheetPersistentDataMechanicalDifferentialTests")]
    public IWorksheetSectionPersistentData Header { get; private set; }

    [MirrorVerified("PtcPersistentDataWorksheetPersistentDataMechanicalDifferentialTests")]
    public IWorksheetSectionPersistentData WorksheetContent { get; private set; }

    [MirrorVerified("PtcPersistentDataWorksheetPersistentDataMechanicalDifferentialTests")]
    public IWorksheetSectionPersistentData Footer { get; private set; }

    [MirrorVerified("PtcPersistentDataWorksheetPersistentDataMechanicalDifferentialTests")]
    public IWorksheetSectionPersistentData FirstPageHeader { get; private set; }

    [MirrorVerified("PtcPersistentDataWorksheetPersistentDataMechanicalDifferentialTests")]
    public IWorksheetSectionPersistentData FirstPageFooter { get; private set; }

    [MirrorVerified("PtcPersistentDataWorksheetPersistentDataMechanicalDifferentialTests")]
    public bool IsFirstPageHeaderFooterDifferent { get; set; }

    [MirrorVerified("PtcPersistentDataWorksheetPersistentDataMechanicalDifferentialTests")]
    public bool ShowIOTags { get; set; }

    [MirrorVerified("PtcPersistentDataWorksheetPersistentDataMechanicalDifferentialTests")]
    public bool OleObjectAutoResize { get; set; }

    [MirrorVerified("PtcPersistentDataWorksheetPersistentDataMechanicalDifferentialTests")]
    public bool SpellCheckEnabled { get; set; }

    [MirrorVerified("PtcPersistentDataWorksheetPersistentDataMechanicalDifferentialTests")]
    public XmlLanguage ProofingLanguage { get; set; }

    public WorksheetPersistentData()
    {
        WorksheetContent = new WorksheetSectionPersistentData();
        Header = new WorksheetSectionPersistentData();
        Footer = new WorksheetSectionPersistentData();
        FirstPageHeader = new WorksheetSectionPersistentData();
        FirstPageFooter = new WorksheetSectionPersistentData();
    }
}
