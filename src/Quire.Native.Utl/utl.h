#pragma once

#include <Windows.h>

#include <cstddef>
#include <list>
#include <string>
#include <vector>

// Class layouts for utl.dll's exports. Each member is declared at the offset its machine code uses; the
// layouts of members that no exported function touches yet are not inferred.

namespace utl {

int GetNumberOfProcessors();
bool IsTimingEnabled();
bool BoostThreadPriority();
wchar_t *toBSTR(unsigned int codePage, const char *source, size_t length);
wchar_t *toBSTR(unsigned int codePage, const char *source);
unsigned char *FromHex(const char *text);
void die(const wchar_t *message);

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
    WordChoice(const WordChoice &other) = default;
    WordChoice &operator=(const WordChoice &other);
    WordChoice &operator=(WordChoice &&other);
    WordDatum *Datum();
    double Score();
    static bool CompareScores(WordChoice a, WordChoice b);
    // GetScore, GetSuggestions and BestGuess(es) are declared for their callers; no body matches yet.
    static double GetScore(const char *word, const char *candidate);
    static std::list<WordChoice> GetSuggestions(const char *word, std::list<WordDatum *> data);
    static const char *BestGuess(const char *word, std::list<WordDatum *> data, double threshold);
    static std::list<const char *> BestGuesses(int count, const char *word, std::list<WordDatum *> data,
                                               double threshold);

protected:
    static bool MatchChar(char c, std::string text, int index);

private:
    WordDatum *datum;
    double score;
};

class WordData {
public:
    WordData();
    WordData(const WordData &other);
    WordData(WordData &&other);
    ~WordData();
    WordData &operator=(const WordData &other);
    WordData &operator=(WordData &&other);
    void Add(WordDatum *datum);
    int Count();
    // The copy constructor and the three queries are declared only; no body matches yet.
    const char *BestGuess(const char *word, double threshold);
    std::list<const char *> BestGuesses(int count, const char *word, double threshold);
    std::list<WordChoice> GetSuggestions(const char *word);

private:
    std::list<WordDatum *> data;
};

// Layout inferred from the machine code. The first three words are the begin, end and capacity of a vector of samples
// (the original's GetStdDev reads them as such; it is not ported yet); the doubles' roles are inferred from the getters.
class PerformanceTimer {
public:
    PerformanceTimer();
    ~PerformanceTimer();
    void Start();
    void Stop();
    void Pause();
    void Resume();
    void Reset();
    long GetCount() const;
    double GetLast() const;
    double GetElapsed() const;
    double GetAverage() const;

private:
    std::vector<double> samples;
    double started;
    double current;
    double total;
    double frequency;
    int count;
    CRITICAL_SECTION lock;
    int depth;
};

class with_purecall_handler {
public:
    with_purecall_handler &operator=(const with_purecall_handler &other);

private:
    void *previous;
};

}  // namespace utl
