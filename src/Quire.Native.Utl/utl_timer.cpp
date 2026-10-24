#include "utl.h"

namespace utl {

PerformanceTimer::PerformanceTimer()
    : samples_begin(nullptr), samples_end(nullptr), samples_capacity(nullptr), started(0), current(0), total(0), count(0), depth(0) {
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
    if (samples_begin != samples_end) {
        samples_end = samples_begin;
    }
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
