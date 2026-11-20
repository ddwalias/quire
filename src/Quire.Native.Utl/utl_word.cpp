#include <stdlib.h>
#include <string.h>

#include <string>

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

bool WordChoice::MatchChar(char c, std::string text, int index) {
    return index >= 0 && index < static_cast<int>(text.size()) && text[index] == c;
}

class purecall_exception {
public:
    ~purecall_exception() {}
};

void ms_purecall_handler() {
    throw purecall_exception();
}

static CRITICAL_SECTION s_purecall_lock;

with_purecall_handler::with_purecall_handler() {
    EnterCriticalSection(&s_purecall_lock);
    previous = reinterpret_cast<void *>(_set_purecall_handler(ms_purecall_handler));
}

with_purecall_handler::~with_purecall_handler() {
    _set_purecall_handler(reinterpret_cast<_purecall_handler>(previous));
    LeaveCriticalSection(&s_purecall_lock);
}

with_purecall_handler &with_purecall_handler::operator=(const with_purecall_handler &other) {
    previous = other.previous;
    return *this;
}

class se_exception {
public:
    explicit se_exception(unsigned int code) : code(code) {}
    ~se_exception() {}
    unsigned int code;
};

void se_trans_func(unsigned int code, _EXCEPTION_POINTERS *) {
    throw se_exception(code);
}

}  // namespace utl
