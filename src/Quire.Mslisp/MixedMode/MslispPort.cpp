#define WIN32_LEAN_AND_MEAN
#define NOMINMAX
#include <sdkddkver.h>

namespace Quire { namespace mslisp {

    [Quire::Mirror::MirrorOfAttribute("mslisp.Stream, mslisp")]
    public interface class Stream
    {
        System::String^ Write(System::String^ s);
        System::String^ WriteLine(System::String^ s);
    };

    [System::SerializableAttribute]
    [Quire::Mirror::MirrorOfAttribute("mslisp.ConsoleStream, mslisp")]
    public ref class ConsoleStream : Stream
    {
    public:
        ConsoleStream()
        {
        }

        virtual System::String^ Write(System::String^ s)
        {
            System::Console::Write(s);
            return s;
        }

        virtual System::String^ WriteLine(System::String^ s)
        {
            System::Console::WriteLine(s);
            return s;
        }
    };

    [System::SerializableAttribute]
    [Quire::Mirror::MirrorOfAttribute("mslisp.DebugStream, mslisp")]
    public ref class DebugStream : Stream
    {
    public:
        DebugStream()
        {
        }

        virtual System::String^ Write(System::String^ s)
        {
            System::Diagnostics::Trace::Write(s);
            return s;
        }

        virtual System::String^ WriteLine(System::String^ s)
        {
            System::Diagnostics::Trace::WriteLine(s);
            return s;
        }
    };

    [Quire::Mirror::MirrorOfAttribute("mslisp.Condition, mslisp")]
    public ref class Condition : System::Exception
    {
    public:
        array<System::Object^>^ args;

        Condition(array<System::Object^>^ a) : args(a)
        {
        }

        Condition(System::String^ msg)
        {
            array<System::Object^>^ arr = gcnew array<System::Object^>(1);
            arr[0] = msg;
            args = arr;
        }

        Condition(System::String^ msg, System::Object^ handle)
        {
            array<System::Object^>^ arr = gcnew array<System::Object^>(2);
            arr[0] = msg;
            arr[1] = handle;
            args = arr;
        }
    };

    [Quire::Mirror::MirrorOfAttribute("mslisp.Symbol, mslisp")]
    public interface class Symbol
    {
        System::String^ get_name();
        System::String^ get_fullname();
        System::String^ get_package();
        System::Object^ get_value();
        System::Object^ set_value(System::Object^ o);
        bool not_set();

        property Symbol^ Base
        {
            Symbol^ get();
        }

        property Symbol^ Tag
        {
            Symbol^ get();
        }

        property Quire::utl_mgd::LabelSet^ Labels
        {
            Quire::utl_mgd::LabelSet^ get();
        }

        property bool Wild
        {
            bool get();
        }

        Symbol^ WithWild(bool bWild);
    };

}}
