using System;
using System.Windows;
using System.Windows.Markup;

namespace Quire.Controls;

public partial class VisualCategoriesDictionary
{
    public const string SymbolicsCategory = "SymbolicsCategory";
    public const string ProgrammingCategory = "ProgrammingCategory";
    public const string BesselCategoryForFunctions = "BesselCategoryForFunctions";
    public const string CurveFittingAndSmoothingCategoryForFunctions = "CurveFittingAndSmoothingCategoryForFunctions";
    public const string DataAnalysisCategoryForFunctions = "DataAnalysisCategoryForFunctions";
    public const string DesignOfExperimentsCategoryForFunctions = "DesignOfExperimentsCategoryForFunctions";
    public const string DifferentialEquationsCategoryForFunctions = "DifferentialEquationsCategoryForFunctions";
    public const string FileAccessCategoryForFunctions = "FileAccessCategoryForFunctions";
    public const string ImageProcessingCategoryForFunctions = "ImageProcessingCategoryForFunctions";
    public const string ProbabilityDistributionCategoryForFunctions = "ProbabilityDistributionCategoryForFunctions";
    public const string SignalProcessingCategoryForFunctions = "SignalProcessingCategoryForFunctions";
    public const string SolvingCategoryForFunctions = "SolvingCategoryForFunctions";
    public const string StatisticsCategoryForFunctions = "StatisticsCategoryForFunctions";
    public const string VectorAndMatrixCategory = "VectorAndMatrixCategory";

    private bool _contentLoaded;

    public VisualCategoriesDictionary()
    {
        InitializeComponent();
    }

    public void InitializeComponent()
    {
        if (_contentLoaded)
        {
            return;
        }

        _contentLoaded = true;
        Uri resourceLocator = new Uri("/Quire.Controls.EquationControl;component/visualcategoriesdictionary.xaml", UriKind.Relative);
        Application.LoadComponent(this, resourceLocator);
    }

    void IComponentConnector.Connect(int connectionId, object target)
    {
        _contentLoaded = true;
    }
}
