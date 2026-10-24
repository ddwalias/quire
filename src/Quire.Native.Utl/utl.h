#pragma once

#include <Windows.h>

#include <cstddef>

// Class layouts for utl.dll's exports. Each member is declared at the offset its machine code uses; the
// layouts of members that no exported function touches yet are not inferred.

namespace utl {

int GetNumberOfProcessors();
wchar_t *toBSTR(unsigned int codePage, const char *source, size_t length);

class InterruptControl {
public:
    InterruptControl &operator=(const InterruptControl &other);
    InterruptControl &operator=(InterruptControl &&other);
    void RequestInterrupt(bool requested);
    static bool IsInterrupted();
    static void ThreadInitialize(InterruptControl *control);

private:
    bool interrupted;
    static DWORD s_tls_index;
};

class WordDatum {
public:
    WordDatum(const char *name);
    WordDatum &operator=(const WordDatum &other);
    WordDatum &operator=(WordDatum &&other);
    const char *Name();

private:
    const char *name;
};

class WordChoice {
public:
    WordChoice(WordDatum *datum, double score);
    WordChoice &operator=(const WordChoice &other);
    WordChoice &operator=(WordChoice &&other);
    WordDatum *Datum();
    double Score();
    static bool CompareScores(WordChoice a, WordChoice b);

private:
    WordDatum *datum;
    double score;
};

class with_purecall_handler {
public:
    with_purecall_handler &operator=(const with_purecall_handler &other);

private:
    void *previous;
};

}  // namespace utl
