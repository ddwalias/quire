#include <windows.h>
#include <deque>
#include <cstdint>

#pragma warning(disable: 4700)

namespace Efi {
    enum MuPadFilter {
        None = 0
    };

    struct Request {
        wchar_t* expr;
        bool b6;
        int filter;
        unsigned long p4;
        unsigned long p5;
        int uninit1;
        int field_1c;
        void* uninit2;
        void* field_28;
    };

    struct Task {
        int opcode;
        int field4;
        void* pRequest;
        void* field10;
        CRITICAL_SECTION* pCS;
    };

    struct Response {
        int64_t field0;
        int64_t bSuccess;
        wchar_t* pResult;
        int64_t field18;
    };

    struct ResponseQueue : std::deque<Response> {
        __declspec(noinline) ~ResponseQueue() {
        }
    };

    CRITICAL_SECTION g_csQueue = { 0 };
    HANDLE g_hQueueEvent = NULL;
    std::deque<Task> g_queue;

    __declspec(noinline) bool sub_9390(wchar_t* dummy, wchar_t* expr, wchar_t** result, MuPadFilter filter, unsigned long p4, unsigned long p5, bool p6) {
        (void)dummy;

        Request request;
        request.expr = expr;
        request.b6 = p6;
        request.filter = filter;
        request.p4 = p4;
        request.p5 = p5;

        HANDLE hEvent = CreateEventW(NULL, TRUE, FALSE, NULL);
        ResponseQueue local_queue;
        CRITICAL_SECTION csResponse;
        InitializeCriticalSection(&csResponse);

        EnterCriticalSection(&g_csQueue);
        Task task;
        task.opcode = 0;
        task.field4 = request.field_1c;
        task.pRequest = &request;
        task.field10 = request.field_28;
        task.pCS = &csResponse;
        g_queue.push_back(task);
        SetEvent(g_hQueueEvent);
        LeaveCriticalSection(&g_csQueue);

        EnterCriticalSection(&csResponse);
        while (local_queue.empty()) {
            LeaveCriticalSection(&csResponse);
            while (WaitForSingleObject(hEvent, INFINITE) != 0) {
            }
            EnterCriticalSection(&csResponse);
        }

        Response resp = local_queue.front();
        local_queue.pop_front();
        if (local_queue.empty()) {
            ResetEvent(hEvent);
        }
        LeaveCriticalSection(&csResponse);

        *result = resp.pResult;
        bool success = (resp.bSuccess != 0);

        DeleteCriticalSection(&csResponse);
        CloseHandle(hEvent);

        return success;
    }

    __declspec(dllexport) bool _AxiomEval(wchar_t* expr, wchar_t** result, MuPadFilter filter, unsigned long p4, unsigned long p5, bool p6) {
        return sub_9390(expr, expr, result, filter, p4, p5, p6);
    }
}
