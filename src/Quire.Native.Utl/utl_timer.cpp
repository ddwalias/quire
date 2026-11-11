#include "utl.h"

namespace utl {

namespace {

// Holds a critical section for the lifetime of the object.
class CriticalSectionLock {
public:
    explicit CriticalSectionLock(CRITICAL_SECTION &section) : section(&section) { EnterCriticalSection(&section); }
    ~CriticalSectionLock() { LeaveCriticalSection(section); }

private:
    CRITICAL_SECTION *section;
};

}  // namespace

PerformanceTimer::PerformanceTimer()
    : started(0), current(0), total(0), frequency(0), count(0), depth(0) {
    LARGE_INTEGER ticks;
    QueryPerformanceFrequency(&ticks);
    frequency = (double)ticks.QuadPart;
    InitializeCriticalSection(&lock);
}

PerformanceTimer::~PerformanceTimer() {
    DeleteCriticalSection(&lock);
}

void PerformanceTimer::Start() {
    CriticalSectionLock guard(lock);
    if (++depth <= 1) {
        current = 0;
        ++count;
        LARGE_INTEGER ticks;
        QueryPerformanceCounter(&ticks);
        started = (double)ticks.QuadPart;
    }
}

void PerformanceTimer::Stop() {
    CriticalSectionLock guard(lock);
    if (--depth <= 0) {
        LARGE_INTEGER ticks;
        QueryPerformanceCounter(&ticks);
        double elapsed = (double)ticks.QuadPart - started;
        started = 0;
        current += elapsed;
        total += elapsed;
        if (current > (frequency * 1e7) / 1e3) {
            --count;
            total -= current;
        } else {
            samples.push_back(current);
        }
    }
}

void PerformanceTimer::Pause() {
    LARGE_INTEGER ticks;
    QueryPerformanceCounter(&ticks);
    double elapsed = (double)ticks.QuadPart - started;
    started = 0;
    current += elapsed;
    total += elapsed;
}

void PerformanceTimer::Resume() {
    LARGE_INTEGER ticks;
    QueryPerformanceCounter(&ticks);
    started = (double)ticks.QuadPart;
}

void PerformanceTimer::Reset() {
    total = 0;
    started = 0;
    current = 0;
    count = 0;
    samples.clear();
}

long PerformanceTimer::GetCount() const {
    return count;
}

double PerformanceTimer::GetLast() const {
    return current * 1000.0 / frequency;
}

double PerformanceTimer::GetElapsed() const {
    return total * 1000.0 / frequency;
}

double PerformanceTimer::GetAverage() const {
    if (count == 0) {
        return 0;
    }
    return (1000.0 / frequency) * (total / count);
}

}  // namespace utl
