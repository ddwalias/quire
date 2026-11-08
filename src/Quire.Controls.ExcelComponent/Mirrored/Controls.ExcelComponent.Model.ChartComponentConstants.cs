#nullable disable
#pragma warning disable

using System.Collections.Generic;

namespace Quire.Controls.ExcelComponent.Model;

public static partial class ChartComponentConstants
{
    public const string ChartComponentHelpContext = "ChartComponentControl";
    private const string Xidentifier = "X";
    private const string Yidentifier = "Y";
    private const string Zidentifier = "Z";
    private const string XMinIdentifier = "Xmin";
    private const string XMaxIdentifier = "Xmax";
    private const string YMinIdentifier = "Ymin";
    private const string YMaxIdentifier = "Ymax";
    private const string Y2MinIdentifier = "Y2min";
    private const string Y2MaxIdentifier = "Y2max";
    private const string ZMinIdentifier = "Zmin";
    private const string ZMaxIdentifier = "Zmax";

    internal static readonly Dictionary<ChartWrapperType, Pair<string, bool>> TypeToIdentifier = new Dictionary<ChartWrapperType, Pair<string, bool>>
    {
        {
            ChartWrapperType.XTrace,
            new Pair<string, bool>("X", true)
        },
        {
            ChartWrapperType.XCommon,
            new Pair<string, bool>("X", false)
        },
        {
            ChartWrapperType.YTrace,
            new Pair<string, bool>("Y", true)
        },
        {
            ChartWrapperType.YCommon,
            new Pair<string, bool>("Y", false)
        },
        {
            ChartWrapperType.ZTrace,
            new Pair<string, bool>("Z", true)
        },
        {
            ChartWrapperType.ZCommon,
            new Pair<string, bool>("Z", false)
        },
        {
            ChartWrapperType.XMin,
            new Pair<string, bool>("Xmin", false)
        },
        {
            ChartWrapperType.XMax,
            new Pair<string, bool>("Xmax", false)
        },
        {
            ChartWrapperType.YMin,
            new Pair<string, bool>("Ymin", false)
        },
        {
            ChartWrapperType.YMax,
            new Pair<string, bool>("Ymax", false)
        },
        {
            ChartWrapperType.Y2Min,
            new Pair<string, bool>("Y2min", false)
        },
        {
            ChartWrapperType.Y2Max,
            new Pair<string, bool>("Y2max", false)
        },
        {
            ChartWrapperType.ZMin,
            new Pair<string, bool>("Zmin", false)
        },
        {
            ChartWrapperType.ZMax,
            new Pair<string, bool>("Zmax", false)
        }
    };
}
