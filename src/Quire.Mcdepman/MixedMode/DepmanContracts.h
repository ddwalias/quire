#pragma once

namespace Quire { namespace depman {

    [Quire::Mirror::MirrorOfAttribute("depman.ScopingMode, mcdepman")]
    public enum class ScopingMode
    {
        NORMAL = 0,
        CHILD_ONLY = 1,
        EXCLUDE_CHILD = 2
    };

    [Quire::Mirror::MirrorOfAttribute("depman.ResolveMode, mcdepman")]
    public enum class ResolveMode
    {
        AT = 0,
        BEFORE = 1,
        AT_OR_BEFORE = 2
    };

    [Quire::Mirror::MirrorOfAttribute("depman.Scope, mcdepman")]
    public ref class Scope abstract
    {
    public:
        property Scope^ Parent { virtual Scope^ get() abstract; }
        property int Id { virtual int get() abstract; }

        virtual int GetHashCode() override
        {
            return Id;
        }

        virtual bool Equals(System::Object^ other) override
        {
            Scope^ scope = dynamic_cast<Scope^>(other);
            if (scope == nullptr)
                return false;

            return Id == scope->Id;
        }

        Scope() {}
    };

    [Quire::Mirror::MirrorOfAttribute("depman.DepNodeCallback, mcdepman")]
    public delegate void DepNodeCallback(long client_handle, long h, Quire::Engine::NodeStatus s);

    interface class Action;

    [Quire::Mirror::MirrorOfAttribute("depman.ActionResult, mcdepman")]
    public value struct ActionResult
    {
        System::Object^ result;
        Action^ next;
        System::Object^ state;

        ActionResult(Action^ action, System::Object^ actionState)
            : result(nullptr), next(action), state(actionState) {}

        ActionResult(System::Object^ data, System::Object^ actionState)
            : result(data), next(nullptr), state(actionState) {}

        System::Object^ GetResult() { return result; }
        Action^ GetNext() { return next; }
        System::Object^ GetState() { return state; }
    };

    [Quire::Mirror::MirrorOfAttribute("depman.Environment, mcdepman")]
    public interface class Environment
    {
        System::Collections::IDictionary^ Lookup(System::String^ name);
        System::Collections::IDictionary^ LookupEx(System::String^ name, ScopingMode m);
        void ExecuteInterruptibly(System::Threading::ThreadStart^ ts, bool bCoopOnly);
        int GetRegionId();
    };

    [Quire::Mirror::MirrorOfAttribute("depman.Action, mcdepman")]
    public interface class Action : System::IDisposable
    {
        ActionResult Invoke(Environment^ env, System::Object^ state);
        System::Collections::ICollection^ GetDefs();
        bool IsUndefine();
        bool IsChildOnly();
    };

    [Quire::Mirror::MirrorOfAttribute("depman.BooleanMediator, mcdepman")]
    public interface class BooleanMediator
    {
        void AddCallback(Quire::Engine::BooleanCallback^ b);
        void RemoveCallback(Quire::Engine::BooleanCallback^ b);
        void WaitValueTrue(int timeout);
        property bool Value { bool get(); }
    };

    [Quire::Mirror::MirrorOfAttribute("depman.SEnvironment, mcdepman")]
    interface class SEnvironment
    {
        int GetRegionId();
    };
}}
