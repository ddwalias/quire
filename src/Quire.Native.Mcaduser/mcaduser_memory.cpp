#include "mcaduser.h"

#include <cstdlib>
#include <cstring>

extern "C" {

char *MathcadAllocate(unsigned int size) {
    char *address = static_cast<char *>(std::malloc(size));
    if (address) {
        std::memset(address, 0, size);
        return address;
    }
    return nullptr;
}

void MathcadFree(char *address) {
    std::free(address);
}

void MathcadArrayFree(COMPLEXARRAY *const array) {
    if (array == nullptr) {
        return;
    }
    if (array->hReal) {
        array->hReal[0] = nullptr;
        std::free(array->hReal);
        array->hReal = nullptr;
    }
    if (array->hImag) {
        array->hImag[0] = nullptr;
        std::free(array->hImag);
        array->hImag = nullptr;
    }
    array->rows = 0;
    array->cols = 0;
}

}  // extern "C"
