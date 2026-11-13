#define WIN32_LEAN_AND_MEAN
#define NOMINMAX
#include <sdkddkver.h>
#include "DepmanContracts.h"
#include "DepmanNodeTimer.h"

namespace Quire { namespace depman {

    [Quire::Mirror::MirrorOfAttribute("depman.IDepman, mcdepman")]
    public interface class IDepman : System::IDisposable
    {
        void AddNodeCallback(Quire::utl_mgd::Function^ f);
        void AddNodeCallback(DepNodeCallback^ c);
        void AddBusyCallback(Quire::utl_mgd::Function^ f);
        void AddBusyCallback(Quire::Engine::BooleanCallback^ b);
        void RemoveBusyCallback(Quire::Engine::BooleanCallback^ b);
        void AddUpToDateCallback(Quire::utl_mgd::Function^ f);
        void AddUpToDateCallback(Quire::Engine::BooleanCallback^ b);
        void RemoveUpToDateCallback(Quire::Engine::BooleanCallback^ b);
        Quire::Mcd::RuntimeOptions^ GetRuntimeOptions();
        long AddAfter(long h, Scope^ s, Action^ a, System::Collections::ICollection^ static_deps, int priority, long client_handle);
        long AddAfterWithDef(long h, Scope^ s, Action^ a, System::Collections::ICollection^ static_deps, System::Collections::ICollection^ static_defs, int priority, long client_handle);
        long AddBefore(long h, Scope^ s, Action^ a, System::Collections::ICollection^ static_deps, int priority, long client_handle);
        void MoveAfter(long h1, long h2, Scope^ s);
        void MoveBefore(long h1, long h2, Scope^ s);
        void Update(long h, Action^ a, System::Collections::ICollection^ static_deps, int priority);
        void SetEnvScope(long h, Scope^ s);
        System::Collections::IDictionary^ GetValueAt(long h, System::String^ name, bool bSync, ResolveMode mode, System::Collections::IDictionary^ pending_value);
        property bool IsBusy { bool get(); }
        property bool IsUpToDate { bool get(); }
        void BeginMove();
        void EndMove();
        void Remove(long h);
        void Lock(long h);
        void LockInvalid(long h);
        void UnLock(long h);
        void UnLockAll();
        void SetPriority(long h, int priority);
        int GetPriority(long h);
        void TreatAsReady(long h);
        void SetNotifyComputing(long h, bool b);
        void SetTimer(long h, NodeTimer^ timer);
        void SetAutoCalc(bool b);
        bool GetAutoCalc();
        void SetCalcNow(bool b);
        bool GetCalcNow();
        void SetCalcNowTrue();
        int IsBoundAt(long h, System::String^ name);
        System::Collections::IDictionary^ GetAllValues(long h, bool bSync);
        System::Collections::IDictionary^ Ready(long h);
        System::Collections::IList^ GetDefined(long h);
        long GetDefinition(System::String^ name, long h);
        System::Collections::IList^ GetDependencies(long h);
        System::Collections::IList^ GetDependents(System::String^ name, long h);
        void StartWorkers(int n);
        void StopWorkers();
        void SetNumThreads(int n);
        int GetNumThreads();
        void WaitUpToDate(int timeout);
        void StopMessages();
        System::Collections::ICollection^ GetHandles();
        void DisableDeps();
        void EnableDeps();
        void DumpNodes(System::String^ filename);
    };
}}
