#include "utl.h"

namespace utl {

namespace {

// Inlines copy construction and assignment of a dummy struct containing vector<double>
// so MSVC outlines vector<double>::operator= and keeps PerformanceTimer::operator= at 105 bytes.
struct VectorCopyOutliner {
    std::vector<double> v;
};

void ForceVectorCopyOutlining(VectorCopyOutliner &dst, const VectorCopyOutliner &src) {
    dst = src;
    VectorCopyOutliner copy(src);
}

}  // namespace

PerformanceTimer &PerformanceTimer::operator=(const PerformanceTimer &other) = default;

}  // namespace utl
