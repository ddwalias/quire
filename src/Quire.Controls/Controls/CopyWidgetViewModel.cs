#nullable disable
using System;
using Quire.Mirror;

namespace Quire.Controls;

public partial class CopyWidgetViewModel
{
    [MirrorVerified("PtcControlsCopyWidgetViewModelMechanicalDifferentialTests")]
    public CopyWidgetViewModel() { }

    private int _totalPages;

    public int TotalPages
    {
        get => _totalPages;
        set
        {
            if (_totalPages == value)
            {
                return;
            }
            _totalPages = value;
            OnPropertyChanged("TotalPages");
        }
    }

    private DateTime? _displaySavedDate;

    public DateTime? DisplaySavedDate
    {
        get => _displaySavedDate;
        set
        {
            if (_displaySavedDate == value)
            {
                return;
            }
            _displaySavedDate = value;
            OnPropertyChanged("DisplaySavedDate");
        }
    }

    private string _displayFileName;

    public string DisplayFileName
    {
        get => _displayFileName;
        set
        {
            if (_displayFileName == value)
            {
                return;
            }
            _displayFileName = value;
            OnPropertyChanged("DisplayFileName");
        }
    }

    private string _displayFilePath;

    public string DisplayFilePath
    {
        get => _displayFilePath;
        set
        {
            if (_displayFilePath == value)
            {
                return;
            }
            _displayFilePath = value;
            OnPropertyChanged("DisplayFilePath");
        }
    }

    private string _displayDocumentName;

    public string DisplayDocumentName
    {
        get => _displayDocumentName;
        set
        {
            if (_displayDocumentName == value)
            {
                return;
            }
            _displayDocumentName = value;
            OnPropertyChanged("DisplayDocumentName");
        }
    }

    private int _pageNumber;

    public int PageNumber
    {
        get => _pageNumber;
        set
        {
            if (_pageNumber == value)
            {
                return;
            }
            _pageNumber = value;
            OnPropertyChanged("PageNumber");
        }
    }

    private double _parentWidth;

    public double ParentWidth
    {
        get => _parentWidth;
        set
        {
            if (_parentWidth == value)
            {
                return;
            }
            _parentWidth = value;
            OnPropertyChanged("ParentWidth");
        }
    }

    private double _includeWidth;

    public double IncludeWidth
    {
        get => _includeWidth;
        set
        {
            if (_includeWidth == value)
            {
                return;
            }
            _includeWidth = value;
            OnPropertyChanged("IncludeWidth");
        }
    }
}
