#include "utl.h"

namespace utl {

WordData::WordData() = default;
WordData::WordData(WordData &&other) = default;
WordData::~WordData() = default;
WordData &WordData::operator=(const WordData &other) = default;
WordData &WordData::operator=(WordData &&other) = default;

void WordData::Add(WordDatum *datum) {
    data.push_front(datum);
}

int WordData::Count() {
    return static_cast<int>(data.size());
}

}  // namespace utl
