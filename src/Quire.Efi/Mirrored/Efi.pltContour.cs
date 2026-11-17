#nullable disable
#pragma warning disable

using System;
using System.Collections.Generic;

namespace Quire.Efi;

internal partial class pltContour
{
    private int myNx;

    private int myNy;

    private int myLastCol;

    private int myLastRow;

    public glMesh mySurf;

    private double myZmax;

    private double myZmin;

    private double myZrange;

    private double[] myLevels;

    private int myNlevels;

    private int myNtraces;

    private pltGridCell.EdgeID myStartEdge;

    private pltGridCell.EdgeID myStopEdge;

    public List<List<shpPoint3D>> m_allcontours;

    public List<int> m_bCntrIdx;

    public pltContour(double[] levels, int nCustom, int nx, int ny, glMesh surf)
    {
        myNlevels = nCustom;
        myNx = nx;
        myNy = ny;
        mySurf = surf;
        myLevels = levels;
        init();
    }

    private void init()
    {
        myLastCol = myNx - 1;
        myLastRow = myNy - 1;
        LRect3 lRect = mySurf.Rect();
        myZmin = lRect.zmin;
        myZmax = lRect.zmax;
        if (myLevels == null)
        {
            defaultLevels();
        }
        else
        {
            myNlevels = myLevels.Length;
        }
        myZrange = myZmax - myZmin;
        m_allcontours = new List<List<shpPoint3D>>();
        m_bCntrIdx = new List<int>();
    }

    public bool drawConts()
    {
        if (myZmin == myZmax)
        {
            return false;
        }
        pltGridCell cell = new pltGridCell(myLastCol, myLastRow, -1.0, 1.0, this);
        BoolMap boolMap = new BoolMap(myLastCol, myLastRow);
        myNtraces = 0;
        bool result = true;
        for (int i = 0; i < myNlevels; i++)
        {
            boolMap.zero();
            if (!trcBndCntrs(i, cell, boolMap))
            {
                result = false;
                break;
            }
            if (!trcIntCntrs(i, cell, boolMap))
            {
                result = false;
                break;
            }
        }
        return result;
    }

    private void saveContour(List<shpPoint3D> cont)
    {
        m_allcontours.Add(cont);
    }

    public shpPoint3D transform(shpPoint3D nPt)
    {
        LRect3 lRect = mySurf.Rect();
        double z = lRect.Dz * (nPt.Z() + 1.0) * 0.5 + lRect.zmin;
        double x = lRect.Dx * (nPt.X() + 1.0) * 0.5 + lRect.xmin;
        double y = lRect.Dy * (nPt.Y() + 1.0) * 0.5 + lRect.ymin;
        return new shpPoint3D(x, y, z);
    }

    private double normalizeZ(double oldZ)
    {
        return -1.0 + 2.0 * (oldZ - myZmin) / myZrange;
    }

    public glMesh Surf()
    {
        return mySurf;
    }

    private bool trcBndCntrs(int levelNum, pltGridCell cell, BoolMap gridMap)
    {
        int r = 1;
        myStartEdge = pltGridCell.EdgeID.EDGE_BOTTOM;
        cell.startEdge(pltGridCell.EdgeID.EDGE_BOTTOM);
        if (traceBoundary(1, r, levelNum, cell, gridMap, isSecond: false) == 10)
        {
            myStartEdge = pltGridCell.EdgeID.EDGE_LEFT;
            cell.startEdge(pltGridCell.EdgeID.EDGE_LEFT);
            traceBoundary(1, r, levelNum, cell, gridMap, isSecond: true);
        }
        int i;
        for (i = 2; i < myLastCol; i++)
        {
            myStartEdge = pltGridCell.EdgeID.EDGE_BOTTOM;
            cell.startEdge(pltGridCell.EdgeID.EDGE_BOTTOM);
            traceBoundary(i, r, levelNum, cell, gridMap, isSecond: false);
        }
        myStartEdge = pltGridCell.EdgeID.EDGE_BOTTOM;
        cell.startEdge(pltGridCell.EdgeID.EDGE_BOTTOM);
        if (traceBoundary(myLastCol, r, levelNum, cell, gridMap, isSecond: false) == 5)
        {
            myStartEdge = pltGridCell.EdgeID.EDGE_RIGHT;
            cell.startEdge(pltGridCell.EdgeID.EDGE_RIGHT);
            traceBoundary(myLastCol, r, levelNum, cell, gridMap, isSecond: true);
        }
        i = 1;
        for (r = 1; r <= myLastRow; r++)
        {
            myStartEdge = pltGridCell.EdgeID.EDGE_LEFT;
            cell.startEdge(pltGridCell.EdgeID.EDGE_LEFT);
            traceBoundary(i, r, levelNum, cell, gridMap, isSecond: false);
        }
        r = myLastRow;
        myStartEdge = pltGridCell.EdgeID.EDGE_TOP;
        cell.startEdge(pltGridCell.EdgeID.EDGE_TOP);
        if (traceBoundary(1, r, levelNum, cell, gridMap, isSecond: false) == 5)
        {
            myStartEdge = pltGridCell.EdgeID.EDGE_TOP;
            cell.startEdge(pltGridCell.EdgeID.EDGE_TOP);
            traceBoundary(1, r, levelNum, cell, gridMap, isSecond: true);
        }
        for (i = 2; i < myLastCol; i++)
        {
            myStartEdge = pltGridCell.EdgeID.EDGE_TOP;
            cell.startEdge(pltGridCell.EdgeID.EDGE_TOP);
            traceBoundary(i, r, levelNum, cell, gridMap, isSecond: false);
        }
        myStartEdge = pltGridCell.EdgeID.EDGE_TOP;
        cell.startEdge(pltGridCell.EdgeID.EDGE_TOP);
        if (traceBoundary(myLastCol, r, levelNum, cell, gridMap, isSecond: false) == 10)
        {
            myStartEdge = pltGridCell.EdgeID.EDGE_TOP;
            cell.startEdge(pltGridCell.EdgeID.EDGE_TOP);
            traceBoundary(myLastCol, r, levelNum, cell, gridMap, isSecond: true);
        }
        i = myLastCol;
        for (r = 1; r <= myLastRow; r++)
        {
            myStartEdge = pltGridCell.EdgeID.EDGE_RIGHT;
            cell.startEdge(pltGridCell.EdgeID.EDGE_RIGHT);
            traceBoundary(i, r, levelNum, cell, gridMap, isSecond: false);
        }
        return true;
    }

    private int traceBoundary(int c, int r, int levelNum, pltGridCell cell, BoolMap gridMap, bool isSecond)
    {
        int col = c;
        int row = r;
        int result = 0;
        bool flag = true;
        cell.initTrace();
        double level = normalizeZ(myLevels[levelNum]);
        cell.newCell(col, row, level, mySurf);
        if (cell.isDouble())
        {
            result = cell.mapIndex();
        }
        flag = cell.hasBoundContr();
        if (!flag && (myStartEdge == pltGridCell.EdgeID.EDGE_BOTTOM || myStartEdge == pltGridCell.EdgeID.EDGE_TOP) && (col == 1 || col == myLastCol))
        {
            myStartEdge = ((col == 1) ? pltGridCell.EdgeID.EDGE_LEFT : pltGridCell.EdgeID.EDGE_RIGHT);
            flag = cell.hasBoundContr();
        }
        if (isSecond || (!gridMap.bitSet(col - 1, row - 1) & flag))
        {
            myNtraces++;
            cell.getCell();
            gridMap.setBit(col - 1, row - 1);
            do
            {
                cell.nextCell(out col, out row);
                if (col < 1 || row < 1 || col > myLastCol || row > myLastRow)
                {
                    break;
                }
                cell.newCell(col, row, level, mySurf);
                gridMap.setBit(col - 1, row - 1);
                if (cell.hasContour())
                {
                    cell.getCell();
                }
            }
            while (cell.isInterior(col, row));
            List<shpPoint3D> cont = cell.trace();
            if (flag)
            {
                m_bCntrIdx.Add(m_allcontours.Count);
                saveContour(cont);
            }
            myStopEdge = cell.stopEdge();
        }
        return result;
    }

    private bool trcIntCntrs(int levelNum, pltGridCell cell, BoolMap gridMap)
    {
        for (int i = 1; i <= myLastCol; i++)
        {
            for (int j = 1; j <= myLastRow; j++)
            {
                if (traceInterior(i, j, levelNum, cell, gridMap, isSecond: false) > 0)
                {
                    traceInterior(i, j, levelNum, cell, gridMap, isSecond: true);
                }
            }
        }
        return true;
    }

}
