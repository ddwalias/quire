#include <string.h>

#include "utl.h"

namespace utl {

WordDatum::WordDatum(const char *name) : name(name) {
}

WordDatum &WordDatum::operator=(const WordDatum &other) {
    name = other.name;
    return *this;
}

WordDatum &WordDatum::operator=(WordDatum &&other) {
    name = other.name;
    return *this;
}

const char *WordDatum::Name() {
    return name;
}

WordChoice::WordChoice(WordDatum *datum, double score) : datum(datum), score(score) {
}

WordChoice &WordChoice::operator=(const WordChoice &other) {
    memcpy(this, &other, sizeof(*this));
    return *this;
}

WordChoice &WordChoice::operator=(WordChoice &&other) {
    datum = other.datum;
    score = other.score;
    return *this;
}

WordDatum *WordChoice::Datum() {
    return datum;
}

double WordChoice::Score() {
    return score;
}

bool WordChoice::CompareScores(WordChoice a, WordChoice b) {
    return a.score > b.score;
}

with_purecall_handler &with_purecall_handler::operator=(const with_purecall_handler &other) {
    previous = other.previous;
    return *this;
}

}  // namespace utl
