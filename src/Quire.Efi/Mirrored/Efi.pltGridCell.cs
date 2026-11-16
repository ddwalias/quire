#nullable disable
#pragma warning disable

using System;
using System.Collections.Generic;
using Quire.Mpl.Run;

namespace Quire.Efi;

internal partial class pltGridCell
{
    private int myNcols;

    private int myNrows;

    private int myStartCol;

    private int myStartRow;

    private double myZmin;

    private double myZmax;

    private pltContour myContPlot;

    private List<shpPoint3D> myTrace;

    private int myTraceCount;

    private EdgeID myStartEdge;

    private EdgeID myStopEdge;

    private int myCol;

    private int myRow;

    private EdgeID myEntryEdge;

    private EdgeID myExitEdge;

    private double myLevel;

    private double myLLz;

    private double myLRz;

    private double myULz;

    private double myURz;

    private int myIndex;

    private double myZavg;

    private Array cellMap;

    public pltGridCell(int nGridCols, int nGridRows, double zMin, double zMax, pltContour cont)
    {
        myNcols = nGridCols;
        myNrows = nGridRows;
        myZmin = zMin;
        myZmax = zMax;
        myContPlot = cont;
        myTraceCount = 0;
        myEntryEdge = EdgeID.EDGE_NONE;
        myExitEdge = EdgeID.EDGE_NONE;
        myStartEdge = EdgeID.EDGE_NONE;
        myStopEdge = EdgeID.EDGE_NONE;
        myStartCol = 0;
        myStartRow = 0;
        myCol = 0;
        myRow = 0;
        myLevel = double.MinValue;
        myLLz = double.MinValue;
        myLRz = double.MinValue;
        myULz = double.MinValue;
        myURz = double.MinValue;
        myZavg = double.MinValue;
        init();
    }

    private void init()
    {
        cellMap = Array.CreateInstance(typeof(int[]), 15);
        cellMap.SetValue(new int[6] { 1, 5, 6, 7, 4, 5 }, 0);
        cellMap.SetValue(new int[6] { 2, 6, 7, 8, 1, 5 }, 1);
        cellMap.SetValue(new int[6] { 2, 6, 7, 4, 0, 4 }, 2);
        cellMap.SetValue(new int[6] { 2, 5, 8, 7, 3, 5 }, 3);
        cellMap.SetValue(new int[6] { 2, 5, 1, 0, 0, 3 }, 4);
        cellMap.SetValue(new int[6] { 3, 7, 8, 1, 0, 4 }, 5);
        cellMap.SetValue(new int[6] { 3, 7, 4, 0, 0, 3 }, 6);
        cellMap.SetValue(new int[6] { 3, 6, 5, 8, 4, 5 }, 7);
        cellMap.SetValue(new int[6] { 3, 6, 5, 1, 0, 4 }, 8);
        cellMap.SetValue(new int[6] { 1, 8, 4, 0, 0, 3 }, 9);
        cellMap.SetValue(new int[6] { 2, 6, 3, 0, 0, 3 }, 10);
        cellMap.SetValue(new int[6] { 2, 5, 8, 4, 0, 4 }, 11);
        cellMap.SetValue(new int[6] { 2, 5, 1, 0, 0, 3 }, 12);
        cellMap.SetValue(new int[6] { 1, 8, 4, 0, 0, 3 }, 13);
        cellMap.SetValue(new int[6] { 9, 10, 11, 12, 0, 4 }, 14);
    }

    public void newCell(int col, int row, double level, glMesh surfMat)
    {
        myCol = col;
        myRow = row;
        myLevel = level;
        myLLz = surfMat.NormalizedVertex(col - 1, row - 1).Z();
        myLRz = surfMat.NormalizedVertex(col, row - 1).Z();
        myULz = surfMat.NormalizedVertex(col - 1, row).Z();
        myURz = surfMat.NormalizedVertex(col, row).Z();
        myZavg = (myLLz + myLRz + myULz + myURz) / 4.0;
        computeMapIndex();
    }

    public void startEdge(EdgeID edge)
    {
        myStartEdge = edge;
    }

    private EdgeID startEdge()
    {
        return myStartEdge;
    }

    public void startCell(int startCol, int startRow)
    {
        myStartCol = startCol;
        myStartRow = startRow;
        int[] array = mapRow();
        int num = array[5];
        myEntryEdge = (EdgeID)array[0];
        myExitEdge = (EdgeID)array[num - 1];
    }

    public bool isStartCell(int col, int row)
    {
        if (col == myStartCol)
        {
            return row == myStartRow;
        }
        return false;
    }

    public bool hasContour()
    {
        if (myIndex > 0)
        {
            return myIndex < 15;
        }
        return false;
    }

    public bool isDouble()
    {
        if (myIndex != 5)
        {
            return myIndex == 10;
        }
        return true;
    }

    public bool hasBoundContr()
    {
        if (hasContour())
        {
            myEntryEdge = myStartEdge;
            myStartCol = myCol;
            myStartRow = myRow;
            int[] array = mapRow();
            int num = array[5];
            if (array[0] == (int)myStartEdge || array[num - 1] == (int)myStartEdge)
            {
                myEntryEdge = myStartEdge;
                return true;
            }
        }
        return false;
    }

    public bool isInterior(int col, int row)
    {
        int[] array = mapRow();
        int num = array[5];
        myExitEdge = (EdgeID)((array[0] == (int)myEntryEdge) ? array[num - 1] : array[0]);
        if ((row == 1 && myExitEdge == EdgeID.EDGE_BOTTOM) || (col == 1 && myExitEdge == EdgeID.EDGE_LEFT) || (row == myNrows && myExitEdge == EdgeID.EDGE_TOP) || (col == myNcols && myExitEdge == EdgeID.EDGE_RIGHT))
        {
            myStopEdge = myExitEdge;
            return false;
        }
        return true;
    }

    public void nextCell(out int col, out int row)
    {
        switch (myExitEdge)
        {
            case EdgeID.EDGE_BOTTOM:
                row = myRow - 1;
                col = myCol;
                myEntryEdge = EdgeID.EDGE_TOP;
                break;
            case EdgeID.EDGE_LEFT:
                col = myCol - 1;
                row = myRow;
                myEntryEdge = EdgeID.EDGE_RIGHT;
                break;
            case EdgeID.EDGE_TOP:
                row = myRow + 1;
                col = myCol;
                myEntryEdge = EdgeID.EDGE_BOTTOM;
                break;
            case EdgeID.EDGE_RIGHT:
                col = myCol + 1;
                row = myRow;
                myEntryEdge = EdgeID.EDGE_LEFT;
                break;
            default:
                col = 0;
                row = 0;
                AppException.Raise("contour unknown error");
                break;
        }
        myStopEdge = myExitEdge;
    }

    public void initTrace()
    {
        myTrace = new List<shpPoint3D>();
        myTraceCount = 0;
        myTrace.Clear();
    }

    public List<shpPoint3D> trace()
    {
        return myTrace;
    }

    public int traceCount()
    {
        return myTraceCount;
    }

    public EdgeID stopEdge()
    {
        return myExitEdge;
    }

    public int mapIndex()
    {
        return myIndex;
    }

    public void mapIndex(int newIndex)
    {
        myIndex = newIndex;
    }

    public void startCol(int col)
    {
        myStartCol = col;
    }

    public void startRow(int row)
    {
        myStartRow = row;
    }

    public int startCol()
    {
        return myStartCol;
    }

    public int startRow()
    {
        return myStartRow;
    }

    private int[] mapRow()
    {
        switch (myIndex)
        {
            case 0:
            case 15:
                return null;
            case 5:
                return ((myEntryEdge == EdgeID.EDGE_BOTTOM || myEntryEdge == EdgeID.EDGE_LEFT) ? cellMap.GetValue(4) : cellMap.GetValue(6)) as int[];
            case 10:
                return ((myEntryEdge == EdgeID.EDGE_BOTTOM || myEntryEdge == EdgeID.EDGE_RIGHT) ? cellMap.GetValue(9) : cellMap.GetValue(10)) as int[];
            default:
                return cellMap.GetValue(myIndex - 1) as int[];
        }
    }

    private void computeMapIndex()
    {
        int num = 0;
        int num2 = 1;
        if (myZavg < myLevel)
        {
            num = 1;
            num2 = 0;
        }
        int num3 = ((myLRz >= myLevel) ? num2 : num);
        int num4 = ((myLLz >= myLevel) ? num2 : num);
        int num5 = ((myULz >= myLevel) ? num2 : num);
        int num6 = ((myURz >= myLevel) ? num2 : num);
        myIndex = num3 | (num4 << 1) | (num5 << 2) | (num6 << 3);
    }

    private double X(double u, double v, double x0, double x1, double x2, double x3)
    {
        return x0 + (x1 - x0) * u + (x2 - x0 + (x0 - x1 - x2 + x3) * u) * v;
    }

    private double Y(double u, double v, double y0, double y1, double y2, double y3)
    {
        return y0 + (y1 - y0) * u + (y2 - y0 + (y0 - y1 - y2 + y3) * u) * v;
    }

}
