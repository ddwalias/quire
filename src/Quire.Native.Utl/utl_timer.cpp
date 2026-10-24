#include "utl.h"

namespace utl {

PerformanceTimer::PerformanceTimer()
    : anchor(0), anchor_copy(0), reserved(0), started(0), current(0), total(0), count(0), depth(0) {
    LARGE_INTEGER ticks;
    QueryPerformanceFrequency(&ticks);
    frequency = (double)ticks.QuadPart;
    InitializeCriticalSection(&lock);
}

void PerformanceTimer::Start() {
    EnterCriticalSection(&lock);
    if (++depth <= 1) {
        current = 0;
        ++count;
        LARGE_INTEGER ticks;
        QueryPerformanceCounter(&ticks);
        started = (double)ticks.QuadPart;
    }
    LeaveCriticalSection(&lock);
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
    if (anchor != anchor_copy) {
        anchor_copy = anchor;
    }
}

long PerformanceTimer::GetCount() const {
    return count;
}

}  // namespace utl
