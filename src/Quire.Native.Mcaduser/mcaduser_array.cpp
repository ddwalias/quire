#include "mcaduser.h"

#include <cstdlib>

namespace {

// The interface passes byte counts as unsigned int, so a count that does not fit in 32 bits cannot be allocated.
bool FitsUnsignedInt(size_t bytes) {
    return bytes == static_cast<unsigned int>(bytes);
}

}  // namespace

extern "C" BOOL MathcadArrayAllocate(COMPLEXARRAY *const array, unsigned int rows, unsigned int cols,
                                     BOOL allocateReal, BOOL allocateImag) {
    if (array == nullptr) {
        return FALSE;
    }
    const size_t columnBytes = static_cast<size_t>(cols) * sizeof(double *);
    const size_t dataBytes = static_cast<size_t>(rows) * cols * sizeof(double);
    if (!FitsUnsignedInt(columnBytes) || !FitsUnsignedInt(dataBytes)) {
        return FALSE;
    }

    if (allocateReal) {
        array->hReal = reinterpret_cast<double **>(MathcadAllocate(static_cast<unsigned int>(columnBytes)));
        if (array->hReal == nullptr) {
            return FALSE;
        }
        array->hReal[0] = reinterpret_cast<double *>(MathcadAllocate(static_cast<unsigned int>(dataBytes)));
        if (array->hReal[0] == nullptr) {
            std::free(array->hReal);
            return FALSE;
        }
        for (unsigned int column = 1; column < cols; ++column) {
            array->hReal[column] = array->hReal[column - 1] + rows;
        }
    } else {
        array->hReal = nullptr;
    }

    if (allocateImag) {
        array->hImag = reinterpret_cast<double **>(MathcadAllocate(static_cast<unsigned int>(columnBytes)));
        if (array->hImag == nullptr) {
            if (allocateReal) {
                std::free(array->hReal[0]);
                std::free(array->hReal);
            }
            return FALSE;
        }
        array->hImag[0] = reinterpret_cast<double *>(MathcadAllocate(static_cast<unsigned int>(dataBytes)));
        if (array->hImag[0] == nullptr) {
            if (allocateReal) {
                std::free(array->hReal[0]);
                std::free(array->hReal);
            }
            std::free(array->hImag);
            return FALSE;
        }
        for (unsigned int column = 1; column < cols; ++column) {
            array->hImag[column] = array->hImag[column - 1] + rows;
        }
    } else {
        array->hImag = nullptr;
    }

    if (array->hReal == nullptr && array->hImag == nullptr) {
        cols = 0;
        rows = 0;
    }
    array->rows = rows;
    array->cols = cols;
    return TRUE;
}
