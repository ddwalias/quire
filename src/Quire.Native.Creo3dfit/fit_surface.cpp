extern "C" {

__declspec(noinline) int internal_fit_surface(
    int a1, double* a2, double* a3, double* a4,
    const double* a5, const double* a6,
    int a7, int a8, int a9, int a10,
    double* a11, double* a12, double* a13,
    void* a14)
{
    (void)a1; (void)a2; (void)a3; (void)a4;
    (void)a5; (void)a6; (void)a7; (void)a8;
    (void)a9; (void)a10; (void)a11; (void)a12;
    (void)a13; (void)a14;
    return 0;
}

int fit_surface_impl(
    int a1, double* a2, double* a3, double* a4,
    const double* a5, const double* a6,
    int a7, int a8, int a9, int a10,
    double* a11, double* a12, double* a13)
{
    return internal_fit_surface(a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11, a12, a13, nullptr);
}

}
