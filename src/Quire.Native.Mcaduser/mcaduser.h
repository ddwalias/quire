#pragma once

// The custom-function interface that mcaduser.dll exports (Custom Functions/MCADINCL.H in the Mathcad install).
// Only the parts that are ported so far are declared here.

#include <windows.h>

struct COMPLEXARRAY {
    unsigned int rows;
    unsigned int cols;
    double **hReal;  // hReal[cols][rows]; null when the real part is zero
    double **hImag;  // hImag[cols][rows]; null when the imaginary part is zero
};

extern "C" {

char *MathcadAllocate(unsigned int size);
void MathcadFree(char *address);

BOOL MathcadArrayAllocate(COMPLEXARRAY *const array, unsigned int rows, unsigned int cols, BOOL allocateReal,
                          BOOL allocateImag);
void MathcadArrayFree(COMPLEXARRAY *const array);

BOOL isUserInterrupted();

}  // extern "C"
