#include "creo3dfit.h"

extern "C" {

int fit_surface_impl(
    int a1, double* a2, double* a3, double* a4,
    const double* a5, const double* a6,
    int a7, int a8, int a9, int a10,
    double* a11, double* a12, double* a13);

int mathcad_fitsurface_(
    int a1, double* a2, double* a3, double* a4,
    const double* a5, const double* a6,
    int a7, int a8, int a9, int a10,
    double* a11, double* a12, double* a13)
{
    return fit_surface_impl(a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11, a12, a13);
}

}
