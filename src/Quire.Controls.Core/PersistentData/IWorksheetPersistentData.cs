#nullable disable
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using Quire.Controls.Core;

namespace Quire.PersistentData;

public partial interface IWorksheetPersistentData
{
    string LayoutSize { get; set; }
    string GridSize { get; set; }
    string PageOrientation { get; set; }
    string MarginType { get; set; }
    Thickness? PageMargin { get; set; }
    bool DisplayGrid { get; set; }
    bool IsSolveBlockLabelVisible { get; set; }
    bool DisplayHFGrid { get; set; }
    bool DisplayFrame { get; set; }
    bool DisplayHeaderFrame { get; set; }
    bool DisplayFooterFrame { get; set; }
    bool UseMajorGridLines { get; set; }
    bool UseHFMajorGridLines { get; set; }
    int StartingPageNumber { get; set; }
    int TotalPagesOffset { get; set; }
    BackgroundTypeE PlotBackgroundType { get; set; }
    Color PageBackgroundColor { get; set; }
    IWorksheetSectionPersistentData Header { get; }
    IWorksheetSectionPersistentData WorksheetContent { get; }
    Dictionary<string, ITextStyleObject> TextStyles { get; set; }
    IWorksheetSectionPersistentData Footer { get; }
    IWorksheetSectionPersistentData FirstPageHeader { get; }
    IWorksheetSectionPersistentData FirstPageFooter { get; }
    bool IsFirstPageHeaderFooterDifferent { get; set; }
    bool ShowIOTags { get; set; }
    bool OleObjectAutoResize { get; set; }
    bool SpellCheckEnabled { get; set; }
    XmlLanguage ProofingLanguage { get; set; }
}
