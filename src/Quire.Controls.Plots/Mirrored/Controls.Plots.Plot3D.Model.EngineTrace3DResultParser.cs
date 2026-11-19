using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Media3D;
using Quire.Calculation.Core;
using Quire.Calculation.Core.Trace2DResults;
using Quire.Controls.Core;
using Quire.Core;

namespace Quire.Controls.Plots.Plot3D.Model;

internal abstract partial class EngineTrace3DResultParser : IEngineTrace3DResultParser
{
    private partial class Undefined : EngineTrace3DResultParser
    {
        private EngineError _error;

        public override EngineError Error
        {
            get => _error;
            protected set
            {
                DebugTools.FailAssertion(null, Array.Empty<object>());
            }
        }

        internal Undefined(EngineTraceType3D engineTraceType3D)
            : base(engineTraceType3D, TraceType3D.Undefined)
        {
            IsInputRangeFullyConstrained = true;
        }

        internal Undefined(EngineError error)
            : this(EngineTraceType3D.Unknown)
        {
            _error = error;
        }

        protected override PlotTraceVisualData3D CreateVisualData(IMatrixResultNode xCoordinates, IMatrixResultNode yCoordinates, IMatrixResultNode zCoordinates, Transform3D transformForVisual)
        {
            DebugTools.DevAssert(false, null);
            return PlotTraceVisualData3D.Undefined;
        }
    }

    private partial class Surface : EngineTrace3DResultParser
    {
        public override EngineError Error { get; protected set; }

        internal Surface(EngineTraceType3D engineTraceType3D)
            : base(engineTraceType3D, TraceType3D.Surface)
        {
        }

        protected override PlotTraceVisualData3D CreateVisualData(IMatrixResultNode xCoordinates, IMatrixResultNode yCoordinates, IMatrixResultNode zCoordinates, Transform3D transformForVisual)
        {
            int rows = xCoordinates.Rows;
            int columns = xCoordinates.Columns;
            int verticesCount = rows * columns;
            SurfaceVisualData3D surfaceVisualData3D = new SurfaceVisualData3D(transformForVisual, verticesCount);
            for (int num = 0; num < rows; num++)
            {
                int num2 = num * columns;
                for (int num3 = 0; num3 < columns; num3++)
                {
                    Point3D value = GetValue(xCoordinates, yCoordinates, zCoordinates, num, num3);
                    if (IsValidAndInRange(value.X, value.Y, value.Z))
                    {
                        surfaceVisualData3D.FilteredVerticesMap[num2 + num3] = true;
                    }
                    surfaceVisualData3D.AddVertex(value);
                }
            }
            int[] array = new int[4];
            for (int num4 = 0; num4 < rows - 1; num4++)
            {
                for (int num5 = 0; num5 < columns - 1; num5++)
                {
                    array[0] = num4 * columns + num5;
                    array[1] = num4 * columns + (num5 + 1);
                    array[2] = (num4 + 1) * columns + (num5 + 1);
                    array[3] = (num4 + 1) * columns + num5;
                    if (array.Any(surfaceVisualData3D.FilteredVerticesMap.Get))
                    {
                        array.Each(surfaceVisualData3D.Indices.Add);
                    }
                }
            }
            return surfaceVisualData3D;
        }
    }

    private partial class Curve : EngineTrace3DResultParser
    {
        public override EngineError Error { get; protected set; }

        internal Curve(EngineTraceType3D engineTraceType3D)
            : base(engineTraceType3D, TraceType3D.Curve)
        {
        }

        protected override PlotTraceVisualData3D CreateVisualData(IMatrixResultNode xCoordinates, IMatrixResultNode yCoordinates, IMatrixResultNode zCoordinates, Transform3D transformForVisual)
        {
            int rows = xCoordinates.Rows;
            Point3D? point3D = null;
            bool flag = false;
            CurveVisualData3D curveVisualData3D = new CurveVisualData3D(transformForVisual, rows * 2);
            for (int num = 0; num < rows; num++)
            {
                Point3D value = GetValue(xCoordinates, yCoordinates, zCoordinates, num, 0);
                double x = value.X;
                double y = value.Y;
                double z = value.Z;
                if (IsValid(x, y, z))
                {
                    bool flag2 = Contains(x, y, z);
                    if (point3D.HasValue && (flag | flag2))
                    {
                        curveVisualData3D.AddVertex(point3D.Value);
                        curveVisualData3D.AddVertex(value);
                    }
                    flag = flag2;
                    point3D = value;
                }
                else
                {
                    flag = false;
                    point3D = null;
                }
            }
            return curveVisualData3D;
        }
    }

    private Rect3D _traceBounds = Rect3D.Empty;

    public TraceType3D TraceType { get; private set; }

    public EngineTraceType3D EngineTraceType { get; private set; }

    public Rect3D TraceBounds
    {
        get => _traceBounds;
        private set => _traceBounds = value;
    }

    public IMatrixResultNode PointsMatrix { get; private set; }

    public bool IsInputRangeFullyConstrained { get; private set; }

    public Func<double, double, double, bool> Contains { get; set; }

    public IList<Unit> Units { get; private set; }

    public abstract EngineError Error { get; protected set; }

    private EngineTrace3DResultParser(EngineTraceType3D engineTraceType, TraceType3D traceType)
    {
        EngineTraceType = engineTraceType;
        TraceType = traceType;
    }

    public static EngineTrace3DResultParser CreatePrintParser(IResultNode resultNode)
    {
        EngineTrace3DResultParser engineTrace3DResultParser = CreateUnknownParser();
        IUnitedValueResultNode unitedValueResultNode = resultNode as IUnitedValueResultNode;
        if (DebugTools.DevAssert(unitedValueResultNode != null, null).Passed && unitedValueResultNode.Units.IsNotEmpty())
        {
            engineTrace3DResultParser.Units = unitedValueResultNode.Units;
        }
        return engineTrace3DResultParser;
    }

    public static EngineTrace3DResultParser CreateParser(IResultNode resultNode)
    {
        if (resultNode == null || resultNode is IEmptyResultNode)
        {
            return CreateUnknownParser();
        }
        IEagerMatrixResultNode eagerMatrixResultNode = null;
        IRealResultNode realResultNode = null;
        Rect3D empty = Rect3D.Empty;
        IUnitedValueResultNode unitedValueResultNode = null;
        bool isInputRangeFullyConstrained = true;
        IMatrixResultNode matrixResultNode = resultNode as IMatrixResultNode;
        EngineTraceType3D enumValue;
        EngineTrace3DResultParser engineTrace3DResultParser;
        if (matrixResultNode != null && matrixResultNode.Rows == 4)
        {
            realResultNode = StripUnits(matrixResultNode[0, 0]) as IRealResultNode;
            IMatrixResultNode matrixResultNode2 = matrixResultNode[1, 0] as IMatrixResultNode;
            if (matrixResultNode2 != null)
            {
                if (!ValidateResult(matrixResultNode2, out var error))
                {
                    return CreateErrorParser(error);
                }
                eagerMatrixResultNode = EagerMatrixSimpleResultNode.ConvertToEagerMatrix(matrixResultNode2);
                unitedValueResultNode = eagerMatrixResultNode[2, 0] as IUnitedValueResultNode;
            }
            IMatrixResultNode matrixResultNode3 = matrixResultNode[2, 0] as IMatrixResultNode;
            if (matrixResultNode3 != null && matrixResultNode3.Rows == 6)
            {
                try
                {
                    empty.Union(new Point3D(((IRealResultNode)matrixResultNode3[0, 0]).Value, ((IRealResultNode)matrixResultNode3[2, 0]).Value, ((IRealResultNode)StripUnits(matrixResultNode3[4, 0])).Value));
                    empty.Union(new Point3D(((IRealResultNode)matrixResultNode3[1, 0]).Value, ((IRealResultNode)matrixResultNode3[3, 0]).Value, ((IRealResultNode)StripUnits(matrixResultNode3[5, 0])).Value));
                }
                catch (Exception ex)
                {
                    empty = Rect3D.Empty;
                    DebugTools.DevAssert(false, ex.Message);
                }
            }
            IRealResultNode realResultNode2 = matrixResultNode[3, 0] as IRealResultNode;
            if (realResultNode2 != null)
            {
                isInputRangeFullyConstrained = (realResultNode2.Value == 1.0);
            }
        }
        if (realResultNode != null && EnumHelper.TryParse<EngineTraceType3D>(realResultNode.Value, out enumValue, assert: false))
        {
            engineTrace3DResultParser = CreateParser(enumValue);
            engineTrace3DResultParser.PointsMatrix = eagerMatrixResultNode;
            engineTrace3DResultParser.TraceBounds = empty;
            engineTrace3DResultParser.IsInputRangeFullyConstrained = isInputRangeFullyConstrained;
            if (unitedValueResultNode != null)
            {
                engineTrace3DResultParser.Units = unitedValueResultNode.Units;
            }
        }
        else
        {
            engineTrace3DResultParser = CreateUnknownParser();
            DebugTools.DevAssert(false, null);
        }
        return engineTrace3DResultParser;
    }

    private static bool ValidateResult(IMatrixResultNode matrixResultNode, out EngineError error)
    {
        int maxNumberOf3dPoints = ControlSettings.Instance.MaxNumberOf3dPoints;
        error = null;
        bool result = true;
        if (GetCoordinates(matrixResultNode, out var xCoordinates, out var _, out var _))
        {
            if (xCoordinates.Rows * xCoordinates.Columns > maxNumberOf3dPoints)
            {
                error = new EngineError
                {
                    Message = TraceErrorCodes.CreateTooManyPointsInResultMessage(maxNumberOf3dPoints)
                };
                result = false;
            }
        }
        else
        {
            DebugTools.FailAssertion(null, Array.Empty<object>());
            error = new EngineError
            {
                Message = "plot3D_inconsistent_Create_format",
                ResourceString = EngineErrorMessages.GetStringByKey("plot3D_inconsistent_Create_format")
            };
            result = false;
        }
        return result;
    }

    public static EngineTrace3DResultParser CreateUnknownParser()
    {
        return new Undefined(EngineTraceType3D.Unknown);
    }

    public static EngineTrace3DResultParser CreateErrorParser(EngineError error)
    {
        return new Undefined(error);
    }

    private static EngineTrace3DResultParser CreateParser(EngineTraceType3D engineTraceType3D)
    {
        EngineTrace3DResultParser result;
        switch (engineTraceType3D)
        {
            case EngineTraceType3D.Unknown:
                result = CreateUnknownParser();
                break;
            case EngineTraceType3D.ParametricCurve:
                result = new Curve(EngineTraceType3D.ParametricCurve);
                break;
            case EngineTraceType3D.ParametricSurface:
                result = new Surface(EngineTraceType3D.ParametricSurface);
                break;
            case EngineTraceType3D.NonParametricSurface:
                result = new Surface(EngineTraceType3D.NonParametricSurface);
                break;
            case EngineTraceType3D.MatrixData:
                result = new Surface(EngineTraceType3D.MatrixData);
                break;
            case EngineTraceType3D.ScatteredData:
                result = new Curve(EngineTraceType3D.ScatteredData);
                break;
            case EngineTraceType3D.CreateSpaceData:
                result = new Curve(EngineTraceType3D.CreateSpaceData);
                break;
            case EngineTraceType3D.CreateMeshData:
                result = new Surface(EngineTraceType3D.CreateMeshData);
                break;
            default:
                result = CreateUnknownParser();
                DebugTools.DevAssert(false, null);
                break;
        }
        return result;
    }

    private static IResultNode StripUnits(IResultNode node)
    {
        if (!(node is IUnitedValueResultNode))
        {
            return node;
        }
        return (node as IUnitedValueResultNode).Value;
    }

    protected static bool GetCoordinates(IMatrixResultNode resultMatrix, out IMatrixResultNode xCoordinates, out IMatrixResultNode yCoordinates, out IMatrixResultNode zCoordinates)
    {
        if (resultMatrix == null)
        {
            xCoordinates = (yCoordinates = (zCoordinates = null));
            return false;
        }
        xCoordinates = StripUnits(resultMatrix[0, 0]) as IMatrixResultNode;
        yCoordinates = StripUnits(resultMatrix[1, 0]) as IMatrixResultNode;
        zCoordinates = StripUnits(resultMatrix[2, 0]) as IMatrixResultNode;
        if (xCoordinates != null && yCoordinates != null && zCoordinates != null && xCoordinates.Count == yCoordinates.Count)
        {
            return yCoordinates.Count == zCoordinates.Count;
        }
        return false;
    }

    public PlotTraceVisualData3D CreateVisualData(Transform3D transformForVisual)
    {
        if (!GetCoordinates(PointsMatrix, out var xCoordinates, out var yCoordinates, out var zCoordinates))
        {
            return PlotTraceVisualData3D.Undefined;
        }
        return CreateVisualData(xCoordinates, yCoordinates, zCoordinates, transformForVisual);
    }

    protected abstract PlotTraceVisualData3D CreateVisualData(IMatrixResultNode xCoordinates, IMatrixResultNode yCoordinates, IMatrixResultNode zCoordinates, Transform3D transformForVisual);

    protected static Point3D GetValue(IMatrixResultNode xCoordinates, IMatrixResultNode yCoordinates, IMatrixResultNode zCoordinates, int rowIndex, int columnIndex)
    {
        return new Point3D(GetValue(xCoordinates, rowIndex, columnIndex), GetValue(yCoordinates, rowIndex, columnIndex), GetValue(zCoordinates, rowIndex, columnIndex));
    }

    private static double GetValue(IMatrixResultNode coordinates, int rowIndex, int columnIndex)
    {
        return ResultNodeUtils.ParsePlottableValue(coordinates[rowIndex, columnIndex]);
    }

    private bool IsValidAndInRange(double xCoordinate, double yCoordinate, double zCoordinate)
    {
        if (IsValid(xCoordinate, yCoordinate, zCoordinate))
        {
            return Contains(xCoordinate, yCoordinate, zCoordinate);
        }
        return false;
    }

    private static bool IsValid(double xCoordinate, double yCoordinate, double zCoordinate)
    {
        if (xCoordinate.IsValid() && yCoordinate.IsValid())
        {
            return zCoordinate.IsValid();
        }
        return false;
    }
}
