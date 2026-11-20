#include "utl.h"

namespace utl {

namespace {

// Inlines copy construction and assignment of a dummy struct containing list<WordDatum*>
// so MSVC outlines list<WordDatum*>::list(const list&) and keeps WordData::WordData(const WordData&) at 28 bytes.
struct ListCopyOutliner {
    std::list<WordDatum*> l;
};

void ForceListCopyOutlining(ListCopyOutliner &dst, const ListCopyOutliner &src) {
    dst = src;
    ListCopyOutliner copy(src);
}

}  // namespace

WordData::WordData(const WordData &other) = default;

}  // namespace utl
