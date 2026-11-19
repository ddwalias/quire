#nullable disable
#pragma warning disable

using Quire.Mcd;
using Quire.Mpl.Run;

namespace Quire.Efi;

public partial class Contour
{
    private object m_levels;

    public Contour(int origin, object exp, object scale, object limits, object gridn, object levels, object[] param)
        : base(origin, exp, scale, limits, gridn, param)
    {
        m_levels = levels;
    }

    public MatVal plot_Contour()
    {
        MatVal matVal;
        MatVal matVal2;
        MatVal matVal3;
        switch (m_plotType)
        {
            case -2:
            {
                ContourMain contourMain = new ContourMain(explicitSurface_rangized(try_multithreading: true).get_1(1) as MatVal, m_origin, m_limits, new NumVal(1.0, 0.0, Dim.identity));
                return contourMain.getContours(m_levels);
            }
            case 1:
            {
                ContourMain contourMain2 = new ContourMain(parametricSurface().get_1(1) as MatVal, m_origin, m_limits, new NumVal(1.0, 0.0, Dim.identity));
                return contourMain2.getContours(m_levels);
            }
            case 2:
            {
                ContourMain contourMain3 = new ContourMain(m_F, m_gridn, m_limits, m_scale);
                return contourMain3.getContours(m_levels);
            }
            case 3:
            case 4:
            {
                if (!m_mdata.IsUniformScalar)
                {
                    AppException.Raise("plot3D non uniform data", 1u);
                }
                if (m_mdata.Rows < 2 || m_mdata.Cols < 2)
                {
                    AppException.Raise("plot3D unsupported data format", 1u);
                }
                ContourMain contourMain4 = new ContourMain(m_mdata, m_origin, m_limits, m_scale);
                return contourMain4.getContours(m_levels);
            }
            case 6:
            {
                if (m_mdata.Rows != 3)
                {
                    AppException.Raise("plot3D unsupported data format", 1u);
                }
                matVal = m_mdata.get_1(0) as MatVal;
                matVal2 = m_mdata.get_1(1) as MatVal;
                matVal3 = m_mdata.get_1(2) as MatVal;
                if (matVal.Cols != matVal2.Cols || matVal.Rows != matVal2.Rows)
                {
                    AppException.Raise("plot3D inconsistent Create format", 1u);
                }
                if (matVal.Cols != matVal3.Cols || matVal.Rows != matVal3.Rows)
                {
                    AppException.Raise("plot3D inconsistent Create format", 1u);
                }
                if (matVal.Cols < 2)
                {
                    AppException.Raise("plot3D unsupported data format", 1u);
                }
                ContourMain contourMain5 = new ContourMain(m_mdata, m_origin, m_limits, m_scale);
                return contourMain5.getContours(m_levels);
            }
            default:
                AppException.Raise("plot3D unsupported data format", 1u);
                return null;
        }
    }

}
