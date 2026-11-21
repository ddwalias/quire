#define WIN32_LEAN_AND_MEAN
#define NOMINMAX
#include <sdkddkver.h>
#include <stdlib.h>
#pragma warning(disable: 4100)
#pragma implicit_keepalive(off)

namespace mslisp {
    namespace Error {
        __declspec(noinline) void report(System::String^ msg)
        {
            throw gcnew System::Exception(msg);
        }

        __declspec(noinline) void report(System::String^ msg, System::Object^ o)
        {
            report(System::String::Concat(msg, o));
        }
    }

    __declspec(noinline) bool is_true(System::Object^ o)
    {
        if (o != nullptr)
        {
            bool^ b = dynamic_cast<bool^>(o);
            if (b != nullptr)
            {
                return *b;
            }
            return true;
        }
        return false;
    }

    array<System::Reflection::MethodInfo^>^ FindMethods(System::Type^ t, System::String^ name, short arity)
    {
        array<System::Reflection::MethodInfo^>^ methods = t->GetMethods(
            (System::Reflection::BindingFlags)65884);
        System::Collections::Generic::List<System::Reflection::MethodInfo^>^ list =
            gcnew System::Collections::Generic::List<System::Reflection::MethodInfo^>(methods->Length);
        for (short i = 0; i < methods->Length; i++)
        {
            array<System::Reflection::ParameterInfo^>^ parameters = methods[i]->GetParameters();
            if (name->Equals(methods[i]->Name))
            {
                int num = methods[i]->IsStatic ? arity : (arity - 1);
                if (arity == -1 || parameters->Length == (short)num)
                {
                    list->Add(methods[i]);
                }
            }
        }
        return list->ToArray();
    }
}

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

    [System::SerializableAttribute]
    [Quire::Mirror::MirrorOfAttribute("mslisp.FileStream, mslisp")]
    public ref class FileStream : Stream
    {
    private:
        static System::Collections::Generic::Dictionary<System::String^, Quire::utl_mgd::SafeMutex^>^ s_mutex_map =
            gcnew System::Collections::Generic::Dictionary<System::String^, Quire::utl_mgd::SafeMutex^>();
        static Quire::utl_mgd::SafeMutex^ s_map_mutex =
            gcnew Quire::utl_mgd::SafeMutex("FileStream");
        System::String^ m_file;
        Quire::utl_mgd::SafeMutex^ m_mutex;

        static Quire::utl_mgd::SafeMutex^ GetMutex(System::String^ path)
        {
            s_map_mutex->WaitOne();
            try
            {
                if (s_mutex_map->ContainsKey(path))
                {
                    return s_mutex_map[path];
                }
                Quire::utl_mgd::SafeMutex^ safeMutex = gcnew Quire::utl_mgd::SafeMutex(path, 1);
                s_mutex_map[path] = safeMutex;
                return safeMutex;
            }
            finally
            {
                s_map_mutex->ReleaseMutex();
            }
        }

        static bool TryDelete(System::String^ f)
        {
            Quire::utl_mgd::SafeMutex^ mutex = GetMutex(f);
            mutex->WaitOne();
            try
            {
                System::IO::File::Delete(f);
                return true;
            }
            catch (System::Exception^)
            {
            }
            finally
            {
                mutex->ReleaseMutex();
            }
            return false;
        }

    public:
        FileStream(System::String^ f)
        {
            System::String^ text = f;
            int num = 0;
            if (!TryDelete(f))
            {
                do
                {
                    num++;
                    System::ValueType^ valueType = num;
                    text = System::String::Concat(System::String::Concat(f, "."), valueType);
                }
                while (!TryDelete(text));
            }
            m_file = text;
            m_mutex = GetMutex(text);
            System::GC::KeepAlive(this);
        }

        virtual System::String^ Write(System::String^ s)
        {
            try
            {
                m_mutex->WaitOne();
                System::IO::StreamWriter^ streamWriter = System::IO::File::AppendText(m_file);
                streamWriter->Write(s);
                streamWriter->Close();
                return s;
            }
            finally
            {
                m_mutex->ReleaseMutex();
            }
        }

        virtual System::String^ WriteLine(System::String^ s)
        {
            try
            {
                m_mutex->WaitOne();
                System::IO::StreamWriter^ streamWriter = System::IO::File::AppendText(m_file);
                streamWriter->WriteLine(s);
                streamWriter->Close();
                return s;
            }
            finally
            {
                m_mutex->ReleaseMutex();
            }
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

    [Quire::Mirror::MirrorOfAttribute("mslisp.Func0, mslisp")]
    delegate System::Object^ Func0();

    [Quire::Mirror::MirrorOfAttribute("mslisp.Func1, mslisp")]
    delegate System::Object^ Func1(System::Object^ A_0);

    [Quire::Mirror::MirrorOfAttribute("mslisp.Func2, mslisp")]
    delegate System::Object^ Func2(System::Object^ A_0, System::Object^ A_1);

    [Quire::Mirror::MirrorOfAttribute("mslisp.Func3, mslisp")]
    delegate System::Object^ Func3(System::Object^ A_0, System::Object^ A_1, System::Object^ A_2);

    [Quire::Mirror::MirrorOfAttribute("mslisp.Func4, mslisp")]
    delegate System::Object^ Func4(System::Object^ A_0, System::Object^ A_1, System::Object^ A_2, System::Object^ A_3);

    [Quire::Mirror::MirrorOfAttribute("mslisp.Func5, mslisp")]
    delegate System::Object^ Func5(System::Object^ A_0, System::Object^ A_1, System::Object^ A_2, System::Object^ A_3, System::Object^ A_4);

    [Quire::Mirror::MirrorOfAttribute("mslisp.Func6, mslisp")]
    delegate System::Object^ Func6(System::Object^ A_0, System::Object^ A_1, System::Object^ A_2, System::Object^ A_3, System::Object^ A_4, System::Object^ A_5);

    [Quire::Mirror::MirrorOfAttribute("mslisp.BuiltinDelegate, mslisp")]
    delegate System::Object^ BuiltinDelegate(array<System::Object^>^ args);

    [Quire::Mirror::MirrorOfAttribute("mslisp.GenCall, mslisp")]
    interface class GenCall
    {
        bool Emit(System::Reflection::Emit::ILGenerator^ g, short arity);
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

    [System::SerializableAttribute]
    [Quire::Mirror::MirrorOfAttribute("mslisp.TaggedSymbol, mslisp")]
    ref class TaggedSymbol : Symbol
    {
    private:
        Symbol^ m_base;
        Symbol^ m_tag;
        Quire::utl_mgd::LabelSet^ m_labels;

    public:
        TaggedSymbol(Symbol^ base, Symbol^ tag, Quire::utl_mgd::LabelSet^ labels)
            : m_base(base), m_tag(tag), m_labels(labels)
        {
        }

        virtual System::String^ ToString() override
        {
            System::String^ text = m_base->ToString();
            if (m_labels != nullptr && !m_labels->Empty())
            {
                text = System::String::Concat(text, m_labels->ToString());
            }
            if (m_tag != nullptr)
            {
                System::String^ text2 = m_tag->ToString();
                System::String^ text3 = ":";
                text = System::String::Concat(text, System::String::Concat(text3, text2));
            }
            System::GC::KeepAlive(this);
            return text;
        }

        virtual System::String^ get_name()
        {
            System::String^ name = m_base->get_name();
            System::GC::KeepAlive(this);
            return name;
        }

        virtual System::String^ get_fullname()
        {
            System::String^ fullname = m_base->get_fullname();
            System::GC::KeepAlive(this);
            return fullname;
        }

        virtual System::String^ get_package()
        {
            System::String^ package = m_base->get_package();
            System::GC::KeepAlive(this);
            return package;
        }

        virtual System::Object^ get_value()
        {
            System::Object^ value = m_base->get_value();
            System::GC::KeepAlive(this);
            return value;
        }

        virtual System::Object^ set_value(System::Object^ o)
        {
            System::Object^ result = m_base->set_value(o);
            System::GC::KeepAlive(this);
            return result;
        }

        virtual bool not_set()
        {
            bool result = m_base->not_set();
            System::GC::KeepAlive(this);
            return result;
        }

        property Symbol^ Base
        {
            virtual Symbol^ get()
            {
                return m_base;
            }
        }

        property Symbol^ Tag
        {
            virtual Symbol^ get()
            {
                return m_tag;
            }
        }

        property Quire::utl_mgd::LabelSet^ Labels
        {
            virtual Quire::utl_mgd::LabelSet^ get()
            {
                return m_labels;
            }
        }

        property bool Wild
        {
            virtual bool get()
            {
                bool b = (m_labels != nullptr && m_labels->Wild());
                System::GC::KeepAlive(this);
                return b;
            }
        }

        virtual Symbol^ WithWild(bool bWild)
        {
            if (bWild != Wild)
            {
                TaggedSymbol^ result = gcnew TaggedSymbol(Base, Tag, Quire::utl_mgd::LabelTrie::WithWild(Labels, bWild));
                System::GC::KeepAlive(this);
                return result;
            }
            System::GC::KeepAlive(this);
            return this;
        }
    };

    [System::SerializableAttribute]
    [Quire::Mirror::MirrorOfAttribute("mslisp.SymbolImp, mslisp")]
    ref class SymbolImp : Symbol, System::Runtime::Serialization::ISerializable
    {
    private:
        System::String^ m_name;
        System::String^ m_package;
        System::Object^ m_value;
        System::String^ m_fullname;
        bool m_not_set;

    public:
        property bool Wild
        {
            virtual bool get()
            {
                return false;
            }
        }

        property Quire::utl_mgd::LabelSet^ Labels
        {
            virtual Quire::utl_mgd::LabelSet^ get()
            {
                return nullptr;
            }
        }

        property Symbol^ Tag
        {
            virtual Symbol^ get()
            {
                return nullptr;
            }
        }

        property Symbol^ Base
        {
            virtual Symbol^ get()
            {
                return this;
            }
        }

        SymbolImp(System::Runtime::Serialization::SerializationInfo^ info, System::Runtime::Serialization::StreamingContext context)
        {
            m_name = info->GetString("m_name");
            System::String^ sPkg = info->GetString("m_package");
            m_package = sPkg;
            array<System::Object^>^ arr = gcnew array<System::Object^>(2);
            arr[0] = sPkg;
            arr[1] = m_name;
            m_fullname = System::String::Format("{0}.{1}", arr);
            if (info->GetBoolean("is_value"))
            {
                System::Type^ typeFromHandle = System::Object::typeid;
                System::String^ name = "m_value";
                m_value = info->GetValue(name, typeFromHandle);
            }
            m_not_set = info->GetBoolean("m_not_set");
            System::GC::KeepAlive(this);
        }

        SymbolImp(System::String^ package, System::String^ name)
            : m_name(name), m_package(package), m_not_set(true)
        {
            array<System::Object^>^ arr = gcnew array<System::Object^>(2);
            arr[0] = m_package;
            arr[1] = m_name;
            m_fullname = System::String::Format("{0}.{1}", arr);
            System::GC::KeepAlive(this);
        }

        virtual void GetObjectData(System::Runtime::Serialization::SerializationInfo^ info, System::Runtime::Serialization::StreamingContext context)
        {
            info->AddValue("m_name", (System::Object^)m_name);
            info->AddValue("m_package", (System::Object^)m_package);
            if (m_value != nullptr)
            {
                if (m_value->GetType()->IsSerializable)
                {
                    info->AddValue("is_value", true);
                    info->AddValue("m_value", m_value);
                }
                else
                {
                    info->AddValue("is_value", false);
                    m_not_set = true;
                }
            }
            else
            {
                info->AddValue("is_value", false);
            }
            info->AddValue("m_not_set", m_not_set);
            System::GC::KeepAlive(this);
        }

        virtual System::String^ ToString() override
        {
            return m_name;
        }

        virtual System::String^ get_name()
        {
            return m_name;
        }

        virtual System::String^ get_fullname()
        {
            return m_fullname;
        }

        virtual System::String^ get_package()
        {
            return m_package;
        }

        virtual System::Object^ get_value()
        {
            return m_value;
        }

        virtual System::Object^ set_value(System::Object^ o)
        {
            m_not_set = false;
            m_value = o;
            return o;
        }

        virtual bool not_set()
        {
            return m_not_set;
        }

        virtual Symbol^ WithWild(bool bWild)
        {
            if (bWild != Wild)
            {
                TaggedSymbol^ result = gcnew TaggedSymbol(this, nullptr, Quire::utl_mgd::LabelTrie::WithWild(Quire::utl_mgd::LabelTrie::Empty(), bWild));
                System::GC::KeepAlive(this);
                return result;
            }
            System::GC::KeepAlive(this);
            return this;
        }
    };

    [System::SerializableAttribute]
    [Quire::Mirror::MirrorOfAttribute("mslisp.Package, mslisp")]
    ref class Package
    {
    private:
        System::String^ m_name;
        System::Collections::Generic::Dictionary<System::String^, Symbol^>^ m_internal;
        System::Collections::Generic::Dictionary<System::String^, Symbol^>^ m_external;

    public:
        Package(System::String^ name)
            : m_name(name),
              m_internal(gcnew System::Collections::Generic::Dictionary<System::String^, Symbol^>()),
              m_external(gcnew System::Collections::Generic::Dictionary<System::String^, Symbol^>())
        {
        }

        Symbol^ intern(System::String^ name)
        {
            Symbol^ sym = nullptr;
            if (m_external->TryGetValue(name, sym))
            {
                System::GC::KeepAlive(this);
                return sym;
            }
            if (m_internal->TryGetValue(name, sym))
            {
                System::GC::KeepAlive(this);
                return sym;
            }
            sym = gcnew SymbolImp(m_name, name);
            m_internal->Add(name, sym);
            System::GC::KeepAlive(this);
            return sym;
        }

        Symbol^ _export(Symbol^ s)
        {
            if (m_external->ContainsKey(s->get_name()))
            {
                m_external->Remove(s->get_name());
            }
            m_external->Add(s->get_name(), s);
            System::GC::KeepAlive(this);
            return s;
        }

        Symbol^ _export(System::String^ name)
        {
            Symbol^ sym = nullptr;
            if (m_external->TryGetValue(name, sym))
            {
                System::GC::KeepAlive(this);
                return sym;
            }
            sym = nullptr;
            if (m_internal->TryGetValue(name, sym))
            {
                m_internal->Remove(name);
            }
            else
            {
                sym = gcnew SymbolImp(m_name, name);
            }
            m_external->Add(name, sym);
            System::GC::KeepAlive(this);
            return sym;
        }

        Symbol^ find_export(System::String^ name)
        {
            Symbol^ sym = nullptr;
            if (m_external->TryGetValue(name, sym))
            {
                System::GC::KeepAlive(this);
                return sym;
            }
            return nullptr;
        }

        System::String^ get_name()
        {
            return m_name;
        }
    };

    [Quire::Mirror::MirrorOfAttribute("mslisp._fcomparer, mslisp")]
    ref class _fcomparer : System::Collections::Generic::IComparer<System::Object^>
    {
    public:
        Quire::utl_mgd::Function^ f;

        _fcomparer(Quire::utl_mgd::Function^ _f) : f(_f)
        {
        }

        virtual int Compare(System::Object^ x, System::Object^ y)
        {
            System::Object^ obj = f->Invoke2(x, y);
            int^ val = dynamic_cast<int^>(obj);
            System::Diagnostics::Trace::Assert(val != nullptr, nullptr);
            System::GC::KeepAlive(this);
            return *val;
        }
    };

    [Quire::Mirror::MirrorOfAttribute("mslisp.InterpreterState, mslisp")]
    value struct InterpreterState
    {
    public:
        Package^ package;
        System::Collections::Generic::Stack<Package^>^ pstack;

        InterpreterState(Package^ p, System::Collections::Generic::Stack<Package^>^ ps)
            : package(p), pstack(ps)
        {
        }
    };

    ref class Env;
    ref class LocalVar;

    [Quire::Mirror::MirrorOfAttribute("mslisp.Expression, mslisp")]
    interface class Expression
    {
        System::Object^ execute(Env^ e);
    };

    [Quire::Mirror::MirrorOfAttribute("mslisp.Settable, mslisp")]
    interface class Settable
    {
        System::Object^ set(System::Object^ o, Env^ e);
    };

    [Quire::Mirror::MirrorOfAttribute("mslisp.Analyzer, mslisp")]
    interface class Analyzer
    {
        Expression^ analyze(System::Object^ o, Env^ env);
        Symbol^ intern(System::String^ name);
    };

    [System::SerializableAttribute]
    [Quire::Mirror::MirrorOfAttribute("mslisp.Env, mslisp")]
    ref class Env
    {
    private:
        Env^ m_parent;
        Quire::Engine::PCons^ m_vars;
        array<System::Object^>^ m_vals;
        int m_size;

    public:
        Env(Env^ parent, array<System::Object^>^ vals)
            : m_parent(parent), m_vals(vals)
        {
        }

        Env(Env^ parent, Quire::Engine::PCons^ vars)
            : m_parent(parent), m_vars(vars), m_size(Quire::Engine::PCons::length(vars))
        {
        }

        System::Object^% lookup(LocalVar^ v);
        LocalVar^ lookup(Symbol^ s);
    };

    [System::SerializableAttribute]
    [Quire::Mirror::MirrorOfAttribute("mslisp.LocalVar, mslisp")]
    ref class LocalVar : Expression, Settable
    {
    public:
        int level;
        int index;
        Symbol^ name;

        LocalVar(Symbol^ s, int l, int i)
            : level(l), index(i), name(s)
        {
        }

        virtual System::Object^ execute(Env^ e)
        {
            return e->lookup(this);
        }

        virtual System::Object^ set(System::Object^ o, Env^ e)
        {
            e->lookup(this) = o;
            return o;
        }

        virtual System::String^ ToString() override
        {
            array<System::Object^>^ args = gcnew array<System::Object^>(3);
            args[0] = name;
            args[1] = level;
            args[2] = index;
            System::String^ result = System::String::Format("{0} {{{1} {2}}}", args);
            System::GC::KeepAlive(this);
            return result;
        }
    };

    inline System::Object^% Env::lookup(LocalVar^ v)
    {
        Env^ env = this;
        for (int i = 0; i < v->level; i++)
        {
            env = env->m_parent;
        }
        return env->m_vals[v->index];
    }

    inline LocalVar^ Env::lookup(Symbol^ s)
    {
        int level = 0;
        Env^ env = this;
        if (this != nullptr)
        {
            do
            {
                int index = 0;
                Quire::Engine::PCons^ p = env->m_vars;
                int i = 0;
                int size = env->m_size;
                if (0 < size)
                {
                    do
                    {
                        if ((System::Object^)s != p->hd)
                        {
                            index++;
                            p = p->tl;
                            i++;
                            continue;
                        }
                        return gcnew LocalVar(s, level, index);
                    }
                    while (i < size);
                }
                level++;
                env = env->m_parent;
            }
            while (env != nullptr);
        }
        return nullptr;
    }

    [System::SerializableAttribute]
    [Quire::Mirror::MirrorOfAttribute("mslisp.QuoteExpr, mslisp")]
    ref class QuoteExpr : Expression
    {
    private:
        System::Object^ m_val;

    public:
        QuoteExpr(System::Object^ o) : m_val(o)
        {
        }

        virtual System::Object^ execute(Env^ e)
        {
            return m_val;
        }

        virtual System::String^ ToString() override
        {
            System::String^ result = System::String::Format("(quote {0})", m_val);
            System::GC::KeepAlive(this);
            return result;
        }
    };

    [System::SerializableAttribute]
    [Quire::Mirror::MirrorOfAttribute("mslisp.PrognExpr, mslisp")]
    ref class PrognExpr : Expression
    {
    private:
        array<Expression^>^ m_exprs;

    public:
        PrognExpr(Analyzer^ i, Env^ e, Quire::Engine::PCons^ args)
        {
            int num = Quire::Engine::PCons::length(args);
            if (num <= 0)
            {
                return;
            }
            m_exprs = gcnew array<Expression^>(num);
            for (int num2 = 0; num2 < num; num2++)
            {
                m_exprs[num2] = i->analyze(args->hd, e);
                args = args->tl;
            }
        }

        virtual System::Object^ execute(Env^ e)
        {
            if (m_exprs != nullptr)
            {
                int num = m_exprs->Length;
                for (int num2 = 0; num2 < num - 1; num2++)
                {
                    m_exprs[num2]->execute(e);
                }
                System::Object^ result = m_exprs[num - 1]->execute(e);
                System::GC::KeepAlive(this);
                return result;
            }
            return nullptr;
        }

        virtual System::String^ ToString() override
        {
            System::Text::StringBuilder^ sb = gcnew System::Text::StringBuilder();
            sb->Append("(progn");
            for (int num = 0; num < m_exprs->Length; num++)
            {
                sb->Append(" ");
                sb->Append(m_exprs[num]->ToString());
            }
            sb->Append(")");
            System::String^ result = sb->ToString();
            System::GC::KeepAlive(this);
            return result;
        }

        static Expression^ NewPrognExpr(Analyzer^ i, Env^ e, Quire::Engine::PCons^ args)
        {
            Expression^ result;
            if (args != nullptr && args->tl == nullptr)
            {
                result = i->analyze(args->hd, e);
            }
            else
            {
                result = gcnew PrognExpr(i, e, args);
            }
            return result;
        }
    };

    [System::SerializableAttribute]
    [Quire::Mirror::MirrorOfAttribute("mslisp.IfExpr, mslisp")]
    ref class IfExpr : Expression
    {
    private:
        Expression^ m_cond;
        Expression^ m_then;
        Expression^ m_else;

    public:
        IfExpr(Analyzer^ i, Env^ e, Quire::Engine::PCons^ args)
        {
            if (args != nullptr && args->hd != nullptr)
            {
                m_cond = i->analyze(args->hd, e);
                Quire::Engine::PCons^ tl = args->tl;
                if (tl != nullptr && tl->hd != nullptr)
                {
                    m_then = i->analyze(tl->hd, e);
                    Quire::Engine::PCons^ tl2 = tl->tl;
                    if (tl2 != nullptr && tl2->hd != nullptr)
                    {
                        m_else = i->analyze(tl2->hd, e);
                    }
                    else
                    {
                        m_else = gcnew QuoteExpr(nullptr);
                    }
                    return;
                }
                ::mslisp::Error::report("Malformed if statement (no consequent");
            }
            ::mslisp::Error::report("Malformed if statement (no arguments)");
        }

        virtual System::Object^ execute(Env^ e)
        {
            if (::mslisp::is_true(m_cond->execute(e)))
            {
                System::Object^ result = m_then->execute(e);
                System::GC::KeepAlive(this);
                return result;
            }
            System::Object^ result2 = m_else->execute(e);
            System::GC::KeepAlive(this);
            return result2;
        }

        virtual System::String^ ToString() override
        {
            array<System::Object^>^ args = gcnew array<System::Object^>(3);
            args[0] = m_cond;
            args[1] = m_then;
            args[2] = m_else;
            System::String^ result = System::String::Format("(if {0} {1} {2})", args);
            System::GC::KeepAlive(this);
            return result;
        }
    };

    [System::SerializableAttribute]
    [Quire::Mirror::MirrorOfAttribute("mslisp.IgnoreErrorsExpr, mslisp")]
    ref class IgnoreErrorsExpr : Expression
    {
    private:
        Expression^ m_expr;

    public:
        IgnoreErrorsExpr(Analyzer^ i, Env^ e, Quire::Engine::PCons^ args)
        {
            if (args != nullptr && args->hd != nullptr)
            {
                m_expr = PrognExpr::NewPrognExpr(i, e, args);
                System::GC::KeepAlive(this);
            }
            else
            {
                ::mslisp::Error::report("Bad ignore-errors form");
            }
        }

        virtual System::Object^ execute(Env^ e)
        {
            try
            {
                System::Object^ result = m_expr->execute(e);
                System::GC::KeepAlive(this);
                return result;
            }
            catch (System::Exception^)
            {
                return nullptr;
            }
        }

        virtual System::String^ ToString() override
        {
            array<System::Object^>^ args = gcnew array<System::Object^>(1);
            args[0] = m_expr;
            System::String^ result = System::String::Format("(ignore-errors {0})", args);
            System::GC::KeepAlive(this);
            return result;
        }
    };

    [System::SerializableAttribute]
    [Quire::Mirror::MirrorOfAttribute("mslisp.UnwindProtectExpr, mslisp")]
    ref class UnwindProtectExpr : Expression
    {
    private:
        Expression^ m_protected;
        Expression^ m_cleanup;

    public:
        UnwindProtectExpr(Analyzer^ i, Env^ e, Quire::Engine::PCons^ args)
        {
            if (args != nullptr && args->tl != nullptr)
            {
                m_protected = i->analyze(args->hd, e);
                m_cleanup = i->analyze(args->tl->hd, e);
                System::GC::KeepAlive(this);
            }
            else
            {
                ::mslisp::Error::report("Bad unwind-protect form");
            }
        }

        virtual System::Object^ execute(Env^ e)
        {
            try
            {
                System::Object^ result = m_protected->execute(e);
                System::GC::KeepAlive(this);
                return result;
            }
            finally
            {
                m_cleanup->execute(e);
            }
        }

        virtual System::String^ ToString() override
        {
            array<System::Object^>^ args = gcnew array<System::Object^>(2);
            args[0] = m_protected;
            args[1] = m_cleanup;
            System::String^ result = System::String::Format("(unwind-protect {0} {1})", args);
            System::GC::KeepAlive(this);
            return result;
        }
    };

    [System::SerializableAttribute]
    [Quire::Mirror::MirrorOfAttribute("mslisp.CarExpr, mslisp")]
    ref class CarExpr : Expression, Settable
    {
    private:
        Expression^ m_expr;

    public:
        CarExpr(Analyzer^ i, Env^ e, Quire::Engine::PCons^ args)
        {
            if (args == nullptr)
            {
                ::mslisp::Error::report("No arguments to car");
            }
            if (args->tl != nullptr)
            {
                ::mslisp::Error::report("Too many arguments to car");
            }
            m_expr = i->analyze(args->hd, e);
            System::GC::KeepAlive(this);
        }

        virtual System::Object^ execute(Env^ e)
        {
            System::Object^ obj = m_expr->execute(e);
            if (obj == nullptr)
            {
                System::GC::KeepAlive(this);
                return nullptr;
            }
            Quire::Engine::PCons^ p = dynamic_cast<Quire::Engine::PCons^>(obj);
            if (p != nullptr)
            {
                return p->hd;
            }
            ::mslisp::Error::report("Bad argument to car: ", obj);
            return nullptr;
        }

        virtual System::Object^ set(System::Object^ o, Env^ e)
        {
            System::Object^ obj = m_expr->execute(e);
            Quire::Engine::PCons^ p = dynamic_cast<Quire::Engine::PCons^>(obj);
            if (p != nullptr)
            {
                System::GC::KeepAlive(this);
                p->hd = o;
                return o;
            }
            ::mslisp::Error::report("Bad argument to car: ", obj);
            return nullptr;
        }

        virtual System::String^ ToString() override
        {
            System::String^ result = System::String::Format("(car {0})", m_expr);
            System::GC::KeepAlive(this);
            return result;
        }
    };

    [System::SerializableAttribute]
    [Quire::Mirror::MirrorOfAttribute("mslisp.CdrExpr, mslisp")]
    ref class CdrExpr : Expression, Settable
    {
    private:
        Expression^ m_expr;

    public:
        CdrExpr(Analyzer^ i, Env^ e, Quire::Engine::PCons^ args)
        {
            if (args == nullptr)
            {
                ::mslisp::Error::report("No arguments to cdr");
            }
            if (args->tl != nullptr)
            {
                ::mslisp::Error::report("Too many arguments to cdr");
            }
            m_expr = i->analyze(args->hd, e);
            System::GC::KeepAlive(this);
        }

        virtual System::Object^ execute(Env^ e)
        {
            System::Object^ obj = m_expr->execute(e);
            if (obj == nullptr)
            {
                System::GC::KeepAlive(this);
                return nullptr;
            }
            Quire::Engine::PCons^ p = dynamic_cast<Quire::Engine::PCons^>(obj);
            if (p != nullptr)
            {
                return p->tl;
            }
            ::mslisp::Error::report("Bad argument to cdr: ", obj);
            return nullptr;
        }

        virtual System::Object^ set(System::Object^ o, Env^ e)
        {
            Quire::Engine::PCons^ pCons = dynamic_cast<Quire::Engine::PCons^>(o);
            if (pCons == nullptr)
            {
                ::mslisp::Error::report("Attempt to set cdr to non-cons value: ", o);
            }
            System::Object^ obj = m_expr->execute(e);
            Quire::Engine::PCons^ p = dynamic_cast<Quire::Engine::PCons^>(obj);
            if (p != nullptr)
            {
                System::GC::KeepAlive(this);
                p->tl = pCons;
                return pCons;
            }
            ::mslisp::Error::report("Bad argument to cdr: ", obj);
            return nullptr;
        }

        virtual System::String^ ToString() override
        {
            System::String^ result = System::String::Format("(cdr {0})", m_expr);
            System::GC::KeepAlive(this);
            return result;
        }
    };

    [System::SerializableAttribute]
    [Quire::Mirror::MirrorOfAttribute("mslisp.ArgSpec, mslisp")]
    ref class ArgSpec
    {
    private:
        Quire::Engine::PCons^ m_param_names;
        int m_num_required_args;
        int m_num_optional_args;
        bool m_has_rest;

    public:
        ArgSpec(Analyzer^ i, System::Object^ params)
        {
            Quire::Engine::PCons^ pCons = dynamic_cast<Quire::Engine::PCons^>(params);
            System::Object^ obj = i->intern("lisp.&rest");
            System::Object^ obj2 = i->intern("lisp.&optional");
            bool flag = false;
            if (pCons != nullptr)
            {
                do
                {
                    if (pCons->hd != obj)
                    {
                        if (pCons->hd == obj2)
                        {
                            flag = true;
                            pCons = pCons->tl;
                            continue;
                        }
                        m_param_names = gcnew Quire::Engine::PCons(pCons->hd, m_param_names);
                        if (flag)
                        {
                            m_num_optional_args++;
                        }
                        else
                        {
                            m_num_required_args++;
                        }
                        pCons = pCons->tl;
                        continue;
                    }
                    if (pCons->tl != nullptr && pCons->tl->hd != nullptr && pCons->tl->tl == nullptr)
                    {
                        m_param_names = gcnew Quire::Engine::PCons(pCons->tl->hd, m_param_names);
                        m_has_rest = true;
                    }
                    else
                    {
                        ::mslisp::Error::report("Invalid argument specification: ", params);
                    }
                    break;
                }
                while (pCons != nullptr);
            }
            m_param_names = Quire::Engine::PCons::reverse(m_param_names);
            System::GC::KeepAlive(this);
        }

        Quire::Engine::PCons^ __identifier(params)()
        {
            return m_param_names;
        }

        array<System::Object^>^ bind(array<System::Object^>^ args)
        {
            int num = (args == nullptr) ? 0 : args->Length;
            if (num < m_num_required_args)
            {
                System::ValueType^ vNum = num;
                System::String^ tab = "\t";
                System::ValueType^ vReq = m_num_required_args;
                ::mslisp::Error::report(System::String::Concat(System::String::Concat(System::String::Concat(System::String::Concat("too_few_arguments", "\t"), vReq), tab), vNum));
            }
            if (!m_has_rest && num > m_num_optional_args + m_num_required_args)
            {
                System::ValueType^ vNum = num;
                System::String^ tab2 = "\t";
                System::ValueType^ vMax = m_num_optional_args + m_num_required_args;
                ::mslisp::Error::report(System::String::Concat(System::String::Concat(System::String::Concat(System::String::Concat("too_many_arguments", "\t"), vMax), tab2), vNum));
            }
            if (!m_has_rest && m_num_optional_args == 0)
            {
                return args;
            }
            int total = m_num_optional_args + m_num_required_args;
            if (m_has_rest)
            {
                total++;
            }
            array<System::Object^>^ res = gcnew array<System::Object^>(total);
            int i = 0;
            for (int num4 = 0; num4 < m_num_required_args; i++, num4++)
            {
                res[i] = args[i];
            }
            int num5 = System::Math::Min(num - m_num_required_args, m_num_optional_args);
            for (int num6 = 0; num6 < num5; i++, num6++)
            {
                res[i] = args[i];
            }
            if (m_has_rest)
            {
                res[i] = Quire::Engine::PCons::FromArray(args, i);
            }
            System::GC::KeepAlive(this);
            return res;
        }

        virtual System::String^ ToString() override
        {
            System::String^ result = Quire::Engine::PCons::_ToString(m_param_names);
            System::GC::KeepAlive(this);
            return result;
        }
    };

    [System::SerializableAttribute]
    [Quire::Mirror::MirrorOfAttribute("mslisp.LambdaExpr, mslisp")]
    ref class LambdaExpr : Quire::utl_mgd::FunctionBase, Expression
    {
    private:
        ArgSpec^ m_argspec;
        Env^ m_parent_env;
        Expression^ m_body;

    public:
        LambdaExpr(Analyzer^ i, Env^ e, Quire::Engine::PCons^ args)
        {
            if (args != nullptr)
            {
                ArgSpec^ argSpec = gcnew ArgSpec(i, args->hd);
                m_argspec = argSpec;
                m_parent_env = e;
                if (args->tl == nullptr)
                {
                    ::mslisp::Error::report("Lambda has no body");
                }
                Env^ e2 = gcnew Env(m_parent_env, argSpec->__identifier(params)());
                m_body = PrognExpr::NewPrognExpr(i, e2, args->tl);
                System::GC::KeepAlive(this);
            }
            else
            {
                ::mslisp::Error::report("No arguments to lambda");
            }
        }

    private:
        LambdaExpr(ArgSpec^ a, Env^ e, Expression^ body)
            : m_argspec(a), m_parent_env(e), m_body(body)
        {
        }

    public:
        virtual System::Object^ Invoke(array<System::Object^>^ args) override
        {
            Expression^ body = m_body;
            ArgSpec^ argSpec = m_argspec;
            Env^ e = gcnew Env(m_parent_env, argSpec->bind(args));
            System::Object^ result = body->execute(e);
            System::GC::KeepAlive(this);
            return result;
        }

        virtual System::Object^ execute(Env^ e)
        {
            return gcnew LambdaExpr(m_argspec, e, m_body);
        }

        virtual System::String^ ToString() override
        {
            array<System::String^>^ parts = gcnew array<System::String^>(5);
            parts[0] = "(lambda ";
            parts[1] = Quire::Engine::PCons::_ToString(m_argspec);
            parts[2] = " ";
            parts[3] = Quire::Engine::PCons::_ToString(m_body);
            parts[4] = ")";
            System::String^ result = System::String::Concat(parts);
            System::GC::KeepAlive(this);
            return result;
        }
    };

    [System::SerializableAttribute]
    [Quire::Mirror::MirrorOfAttribute("mslisp.MacroExpr, mslisp")]
    ref class MacroExpr : LambdaExpr
    {
    public:
        MacroExpr(Analyzer^ i, Env^ e, Quire::Engine::PCons^ args)
            : LambdaExpr(i, e, args)
        {
        }

        virtual System::Object^ execute(Env^ e) override
        {
            return this;
        }
    };

    [System::SerializableAttribute]
    [Quire::Mirror::MirrorOfAttribute("mslisp.DefmacroExpr, mslisp")]
    ref class DefmacroExpr : Expression
    {
    private:
        Symbol^ m_sym;
        MacroExpr^ m_expr;

    public:
        DefmacroExpr(Analyzer^ i, Env^ e, Quire::Engine::PCons^ args)
        {
            if (args != nullptr)
            {
                Symbol^ sym = dynamic_cast<Symbol^>(args->hd);
                m_sym = sym;
                if (sym == nullptr)
                {
                    ::mslisp::Error::report("Illegal arguments to defmacro: ", args);
                }
                if (args->tl != nullptr)
                {
                    m_expr = gcnew MacroExpr(i, e, args->tl);
                }
                System::GC::KeepAlive(this);
            }
            else
            {
                ::mslisp::Error::report("No arguments to defmacro");
            }
        }

        virtual System::Object^ execute(Env^ e)
        {
            Symbol^ sym = m_sym;
            System::Object^ value = m_expr->execute(e);
            System::Object^ result = sym->set_value(value);
            System::GC::KeepAlive(this);
            return result;
        }
    };

    [System::SerializableAttribute]
    [Quire::Mirror::MirrorOfAttribute("mslisp.HandlerCaseExpr, mslisp")]
    ref class HandlerCaseExpr : Expression
    {
    private:
        Expression^ m_expr;
        LambdaExpr^ m_handler;

    public:
        HandlerCaseExpr(Analyzer^ i, Env^ e, Quire::Engine::PCons^ args)
        {
            if (args != nullptr && args->hd != nullptr && args->tl != nullptr && args->tl->hd != nullptr)
            {
                Quire::Engine::PCons^ pCons = dynamic_cast<Quire::Engine::PCons^>(args->tl->hd);
                if (pCons != nullptr && pCons->tl != nullptr)
                {
                    m_expr = i->analyze(args->hd, e);
                    m_handler = gcnew LambdaExpr(i, e, pCons->tl);
                    return;
                }
            }
            ::mslisp::Error::report("Bad handler-case form");
        }

#pragma implicit_keepalive(on)
        virtual System::Object^ execute(Env^ e)
        {
            try
            {
                System::Object^ result = m_expr->execute(e);
                System::GC::KeepAlive(this);
                return result;
            }
            catch (System::Exception^ ex)
            {
                LambdaExpr^ lambdaExpr = dynamic_cast<LambdaExpr^>(m_handler->execute(e));
                if (lambdaExpr != nullptr)
                {
                    return lambdaExpr->Invoke1(ex);
                }
                throw gcnew System::Exception("handler-case: could not execute handler", ex);
            }
        }
#pragma implicit_keepalive(off)

        virtual System::String^ ToString() override
        {
            array<System::Object^>^ args = gcnew array<System::Object^>(2);
            args[0] = m_expr;
            args[1] = m_handler;
            System::String^ result = System::String::Format("(handler-case {0} {1})", args);
            System::GC::KeepAlive(this);
            return result;
        }
    };

    [System::SerializableAttribute]
    [Quire::Mirror::MirrorOfAttribute("mslisp.SetfExpr, mslisp")]
    ref class SetfExpr : Expression
    {
    private:
        Settable^ m_place;
        Expression^ m_expr;

    public:
        SetfExpr(Analyzer^ i, Env^ e, Quire::Engine::PCons^ args)
        {
            if (args != nullptr)
            {
                Settable^ place = dynamic_cast<Settable^>(i->analyze(args->hd, e));
                m_place = place;
                if (place == nullptr)
                {
                    ::mslisp::Error::report("Cannot be assigned to: ", args->hd);
                }
                if (args->tl != nullptr)
                {
                    m_expr = i->analyze(args->tl->hd, e);
                }
                System::GC::KeepAlive(this);
            }
            else
            {
                ::mslisp::Error::report("No arguments to define");
            }
        }

        virtual System::Object^ execute(Env^ e)
        {
            Settable^ place = m_place;
            System::Object^ o = m_expr->execute(e);
            System::Object^ result = place->set(o, e);
            System::GC::KeepAlive(this);
            return result;
        }

        virtual System::String^ ToString() override
        {
            array<System::Object^>^ args = gcnew array<System::Object^>(2);
            args[0] = m_place;
            args[1] = m_expr;
            System::String^ result = System::String::Format("(setf {0} {1})", args);
            System::GC::KeepAlive(this);
            return result;
        }
    };

    [System::SerializableAttribute]
    [Quire::Mirror::MirrorOfAttribute("mslisp.StrcatFunction, mslisp")]
    ref class StrcatFunction : Quire::utl_mgd::FunctionBase
    {
    public:
        virtual System::Object^ Invoke(array<System::Object^>^ args) override
        {
            System::String^ result = System::String::Concat(args);
            System::GC::KeepAlive(this);
            return result;
        }

        StrcatFunction()
        {
        }
    };

    [System::SerializableAttribute]
    [Quire::Mirror::MirrorOfAttribute("mslisp.SignalFunction, mslisp")]
    ref class SignalFunction : Quire::utl_mgd::FunctionBase
    {
    public:
        virtual System::Object^ Invoke(array<System::Object^>^ args) override
        {
            if (args->Length == 1)
            {
                Condition^ condition = dynamic_cast<Condition^>(args[0]);
                if (condition != nullptr)
                {
                    throw condition;
                }
            }
            throw gcnew Condition(args);
        }

        SignalFunction()
        {
        }
    };

    [System::SerializableAttribute]
    [Quire::Mirror::MirrorOfAttribute("mslisp.SingleMethod, mslisp")]
    ref class SingleMethod : Quire::utl_mgd::FunctionBase
    {
    private:
        System::Reflection::MethodInfo^ m_info;

    public:
        SingleMethod(System::Reflection::MethodInfo^ m) : m_info(m)
        {
        }

        virtual System::Object^ Invoke(array<System::Object^>^ args) override
        {
            System::Object^ result = m_info->Invoke(nullptr, args);
            System::GC::KeepAlive(this);
            return result;
        }

        virtual System::String^ ToString() override
        {
            System::String^ result = System::String::Concat("mslisp.SingleMethod: ", m_info);
            System::GC::KeepAlive(this);
            return result;
        }
    };

    [System::SerializableAttribute]
    [Quire::Mirror::MirrorOfAttribute("mslisp.DelegateFunction, mslisp")]
    ref class DelegateFunction : Quire::utl_mgd::FunctionBase
    {
    private:
        BuiltinDelegate^ m_delegate;

    public:
        DelegateFunction(BuiltinDelegate^ d) : m_delegate(d)
        {
        }

        virtual System::Object^ Invoke(array<System::Object^>^ args) override
        {
            System::Object^ result = m_delegate(args);
            System::GC::KeepAlive(this);
            return result;
        }

        virtual System::String^ ToString() override
        {
            System::String^ result = System::String::Concat("mslisp.DelegateFunction: ", m_delegate);
            System::GC::KeepAlive(this);
            return result;
        }
    };

    [Quire::Mirror::MirrorOfAttribute("mslisp.CodeGen, mslisp")]
    ref class CodeGen
    {
    public:
        static array<System::Type^>^ s_dtype = gcnew array<System::Type^>(7);

        static void EmitLdarg(System::Reflection::Emit::ILGenerator^ g, short i)
        {
            switch (i)
            {
            case 0:
                g->Emit(System::Reflection::Emit::OpCodes::Ldarg_0);
                break;
            case 1:
                g->Emit(System::Reflection::Emit::OpCodes::Ldarg_1);
                break;
            case 2:
                g->Emit(System::Reflection::Emit::OpCodes::Ldarg_2);
                break;
            case 3:
                g->Emit(System::Reflection::Emit::OpCodes::Ldarg_3);
                break;
            default:
                g->Emit(System::Reflection::Emit::OpCodes::Ldarg, i);
                break;
            }
        }

        static void EmitLdargs(System::Reflection::Emit::ILGenerator^ g, short offset, short arity, array<System::Reflection::ParameterInfo^>^ p)
        {
            short num = offset;
            if (offset < arity)
            {
                do
                {
                    EmitLdarg(g, num);
                    if (p[num - offset]->ParameterType->IsValueType)
                    {
                        System::Type^ parameterType = p[num - offset]->ParameterType;
                        System::Reflection::Emit::OpCode unbox_Any = System::Reflection::Emit::OpCodes::Unbox_Any;
                        g->Emit(unbox_Any, parameterType);
                    }
                    num++;
                }
                while (num < arity);
            }
        }

        static void MaybeBox(System::Reflection::Emit::ILGenerator^ g, System::Type^ t)
        {
            if (t == System::Void::typeid)
            {
                g->Emit(System::Reflection::Emit::OpCodes::Ldnull);
            }
            else if (t->IsValueType)
            {
                g->Emit(System::Reflection::Emit::OpCodes::Box, t);
            }
        }

        static System::Object^ GenDelegate(System::Object^ f, int arity)
        {
            try
            {
                GenCall^ genCall = dynamic_cast<GenCall^>(f);
                if (genCall == nullptr)
                {
                    return nullptr;
                }
                array<System::Type^>^ arrayType = gcnew array<System::Type^>(arity);
                for (short num = 0; num < arity; num++)
                {
                    System::Type^ typeFromHandle = System::Object::typeid;
                    arrayType[num] = typeFromHandle;
                }
                System::Reflection::Emit::DynamicMethod^ dynamicMethod = gcnew System::Reflection::Emit::DynamicMethod(
                    f->ToString(),
                    System::Object::typeid,
                    arrayType,
                    CodeGen::typeid);
                System::Reflection::Emit::ILGenerator^ iLGenerator = dynamicMethod->GetILGenerator();
                if (!genCall->Emit(iLGenerator, (short)arity))
                {
                    return nullptr;
                }
                return dynamicMethod->CreateDelegate(s_dtype[arity]);
            }
            catch (System::Exception^)
            {
                return nullptr;
            }
        }

        CodeGen()
        {
        }

    private:
        static CodeGen()
        {
            s_dtype[0] = Func0::typeid;
            s_dtype[1] = Func1::typeid;
            s_dtype[2] = Func2::typeid;
            s_dtype[3] = Func3::typeid;
            s_dtype[4] = Func4::typeid;
            s_dtype[5] = Func5::typeid;
            s_dtype[6] = Func6::typeid;
        }
    };

    [System::SerializableAttribute]
    [Quire::Mirror::MirrorOfAttribute("mslisp.Constructor, mslisp")]
    ref class Constructor : Quire::utl_mgd::FunctionBase, GenCall
    {
    private:
        System::Type^ m_type;

    public:
        Constructor(System::Type^ t) : m_type(t)
        {
        }

        virtual System::Object^ Invoke(array<System::Object^>^ args) override
        {
            System::Object^ result = System::Activator::CreateInstance(m_type, args);
            System::GC::KeepAlive(this);
            return result;
        }

        virtual bool Emit(System::Reflection::Emit::ILGenerator^ g, short arity)
        {
            array<System::Reflection::ConstructorInfo^>^ constructors = m_type->GetConstructors();
            short num = 0;
            short num2 = -1;
            short num3 = 0;
            if (0 < constructors->Length)
            {
                do
                {
                    if (constructors[num3]->GetParameters()->Length == arity)
                    {
                        num++;
                        num2 = num3;
                    }
                    num3++;
                }
                while (num3 < constructors->Length);
                if (num == 1)
                {
                    CodeGen::EmitLdargs(g, 0, arity, constructors[num2]->GetParameters());
                    System::Reflection::ConstructorInfo^ con = constructors[num2];
                    g->Emit(System::Reflection::Emit::OpCodes::Newobj, con);
                    g->Emit(System::Reflection::Emit::OpCodes::Ret);
                    System::GC::KeepAlive(this);
                    return true;
                }
            }
            System::GC::KeepAlive(this);
            return false;
        }

        virtual System::String^ ToString() override
        {
            System::String^ result = System::String::Concat("mslisp.Constructor: ", m_type);
            System::GC::KeepAlive(this);
            return result;
        }
    };

    [System::SerializableAttribute]
    [Quire::Mirror::MirrorOfAttribute("mslisp.InstanceMethod, mslisp")]
    ref class InstanceMethod : Quire::utl_mgd::FunctionBase
    {
    private:
        System::String^ m_name;
    public:
        InstanceMethod(System::String^ name) : m_name(name) {}
        virtual System::Object^ Invoke(array<System::Object^>^ args) override
        {
            array<System::Object^>^ arr;
            int num;
            System::Object^ target = args[0];
            num = args->Length - 1;
            arr = gcnew array<System::Object^>(num);
            System::Array::Copy(args, 1, arr, 0, num);
            try
            {
                System::Type^ type = target->GetType();
                System::Object^ result = type->InvokeMember(m_name,
                    (System::Reflection::BindingFlags)65876,
                    nullptr, target, arr);
                System::GC::KeepAlive(this);
                return result;
            }
            catch (System::MissingMethodException^)
            {
                ::mslisp::Error::report("Could not find method ", m_name);
            }
            return nullptr;
        }
        virtual System::String^ ToString() override
        {
            System::String^ result = System::String::Concat("mslisp.InstanceMethod: ", m_name);
            System::GC::KeepAlive(this);
            return result;
        }
    };

    [System::SerializableAttribute]
    [Quire::Mirror::MirrorOfAttribute("mslisp.StaticOrInstanceMethod, mslisp")]
    ref class StaticOrInstanceMethod : Quire::utl_mgd::FunctionBase, GenCall
    {
    private:
        System::Type^ m_type;
        System::String^ m_name;
    public:
        StaticOrInstanceMethod(System::Type^ t, System::String^ name) : m_type(t), m_name(name) {}
        virtual System::Object^ Invoke(array<System::Object^>^ args) override
        {
            try
            {
                System::Object^ result = m_type->InvokeMember(m_name,
                    (System::Reflection::BindingFlags)65880,
                    nullptr, nullptr, args);
                System::GC::KeepAlive(this);
                return result;
            }
            catch (System::MissingMethodException^)
            {
                ::mslisp::Error::report("Could not find method ", m_name);
            }
            return nullptr;
        }
        virtual bool Emit(System::Reflection::Emit::ILGenerator^ g, short arity)
        {
            array<System::Reflection::MethodInfo^>^ methods = ::mslisp::FindMethods(m_type, m_name, arity);
            System::Reflection::MethodInfo^ methodInfo = nullptr;
            if (methods->Length == 1)
            {
                methodInfo = methods[0];
            }
            else
            {
                int num = 0;
                if (0 < methods->Length)
                {
                    do
                    {
                        if (!methods[num]->IsVirtual)
                        {
                            num++;
                            continue;
                        }
                        methodInfo = methods[num];
                        break;
                    }
                    while (num < methods->Length);
                }
            }
            if (methodInfo == nullptr)
            {
                System::GC::KeepAlive(this);
                return false;
            }
            short offset = (short)(methodInfo->IsStatic ? 0 : 1);
            if (!methodInfo->IsStatic)
            {
                g->Emit(System::Reflection::Emit::OpCodes::Ldarg_0);
            }
            CodeGen::EmitLdargs(g, offset, arity, methodInfo->GetParameters());
            if (methodInfo->IsVirtual)
            {
                g->Emit(System::Reflection::Emit::OpCodes::Callvirt, methodInfo);
            }
            else
            {
                g->Emit(System::Reflection::Emit::OpCodes::Call, methodInfo);
            }
            CodeGen::MaybeBox(g, methodInfo->ReturnType);
            g->Emit(System::Reflection::Emit::OpCodes::Ret);
            System::GC::KeepAlive(this);
            return true;
        }
        virtual System::String^ ToString() override
        {
            System::String^ result = System::String::Concat("mslisp.StaticOrInstanceMethod: ", m_name);
            System::GC::KeepAlive(this);
            return result;
        }
    };

    static bool HasMethods(System::Type^ type, System::String^ name)
    {
        return ::mslisp::FindMethods(type, name, -1)->Length > 0;
    }

    [System::SerializableAttribute]
    [Quire::Mirror::MirrorOfAttribute("mslisp.GlobalVar, mslisp")]
    ref class GlobalVar : Expression, Settable
    {
    public:
        Symbol^ m_sym;
        GlobalVar(Symbol^ s) : m_sym(s) {}
        virtual System::Object^ execute(Env^ e)
        {
            System::Object^ value = m_sym->get_value();
            if (value != nullptr)
            {
                System::GC::KeepAlive(this);
                return value;
            }
            if (m_sym->not_set())
            {
                System::String^ name = m_sym->get_name();
                if (name[0] == '*')
                {
                    System::GC::KeepAlive(this);
                    return nullptr;
                }
                System::Object^ obj = nullptr;
                System::Type^ type = Quire::Engine::Reflection::FindType(m_sym->get_package());
                if ((System::Object^)type != nullptr)
                {
                    if (type->IsEnum)
                    {
                        array<System::String^>^ names = System::Enum::GetNames(type);
                        System::Array^ values = System::Enum::GetValues(type);
                        for (int num = 0; num < names->Length; num++)
                        {
                            if (names[num]->Equals(name))
                            {
                                obj = values->GetValue(num);
                                break;
                            }
                        }
                    }
                    else if (HasMethods(type, name))
                    {
                        obj = gcnew StaticOrInstanceMethod(type, name);
                    }
                    else
                    {
                        array<System::Reflection::MemberInfo^>^ member = type->GetMember(name,
                            (System::Reflection::BindingFlags)26);
                        if (member != nullptr)
                        {
                            System::Reflection::FieldInfo^ fieldInfo = dynamic_cast<System::Reflection::FieldInfo^>(member[0]);
                            if ((System::Object^)fieldInfo != nullptr)
                            {
                                obj = fieldInfo->GetValue(nullptr);
                            }
                        }
                    }
                }
                else
                {
                    System::Type^ type2 = Quire::Engine::Reflection::FindType(m_sym->get_fullname());
                    if ((System::Object^)type2 != nullptr)
                    {
                        obj = gcnew Constructor(type2);
                    }
                }
                if (obj == nullptr)
                {
                    obj = gcnew InstanceMethod(name);
                }
                m_sym->set_value(obj);
            }
            System::Object^ result = m_sym->get_value();
            System::GC::KeepAlive(this);
            return result;
        }
        virtual System::Object^ set(System::Object^ o, Env^ e)
        {
            System::Object^ result = m_sym->set_value(o);
            System::GC::KeepAlive(this);
            return result;
        }
        virtual System::String^ ToString() override
        {
            System::String^ result = System::String::Format("{0} {{global}}", m_sym);
            System::GC::KeepAlive(this);
            return result;
        }
    };

    [Quire::Mirror::MirrorOfAttribute("mslisp.FastApp, mslisp")]
    ref class FastApp abstract
    {
    public:
        virtual System::Object^ execute(Env^ e, int trace_level) abstract;
        FastApp() {}
    };

    [Quire::Mirror::MirrorOfAttribute("mslisp.FastApp0, mslisp")]
    ref class FastApp0 : FastApp
    {
    private:
        Func0^ m_func;
    public:
        FastApp0(Func0^ f) : m_func(f) {}
        virtual System::Object^ execute(Env^ e, int trace_level) override
        {
            System::Object^ result = m_func->Invoke();
            System::GC::KeepAlive(this);
            return result;
        }
        virtual System::String^ ToString() override
        {
            array<System::Object^>^ args = gcnew cli::array<System::Object^>(1);
            args[0] = m_func;
            System::String^ result = System::String::Format("(FastApp0 {0})", args);
            System::GC::KeepAlive(this);
            return result;
        }
    };

    [Quire::Mirror::MirrorOfAttribute("mslisp.FastApp1, mslisp")]
    ref class FastApp1 : FastApp
    {
    private:
        Expression^ m_arg1;
        Func1^ m_func;
    public:
        FastApp1(Func1^ f, Expression^ arg1) : m_arg1(arg1), m_func(f) {}
        virtual System::Object^ execute(Env^ e, int trace_level) override
        {
            System::Object^ result = m_func->Invoke(m_arg1->execute(e));
            System::GC::KeepAlive(this);
            return result;
        }
        virtual System::String^ ToString() override
        {
            array<System::Object^>^ args = gcnew cli::array<System::Object^>(2);
            args[0] = m_func;
            args[1] = m_arg1;
            System::String^ result = System::String::Format("(FastApp1 {0} {1})", args);
            System::GC::KeepAlive(this);
            return result;
        }
    };

    [Quire::Mirror::MirrorOfAttribute("mslisp.FastApp2, mslisp")]
    ref class FastApp2 : FastApp
    {
    private:
        Expression^ m_arg1;
        Expression^ m_arg2;
        Func2^ m_func;
    public:
        FastApp2(Func2^ f, Expression^ arg1, Expression^ arg2) : m_arg1(arg1), m_arg2(arg2), m_func(f) {}
        virtual System::Object^ execute(Env^ e, int trace_level) override
        {
            System::Object^ a1 = m_arg1->execute(e);
            System::Object^ a2 = m_arg2->execute(e);
            System::Object^ result = m_func->Invoke(a1, a2);
            System::GC::KeepAlive(this);
            return result;
        }
        virtual System::String^ ToString() override
        {
            array<System::Object^>^ args = gcnew cli::array<System::Object^>(3);
            args[0] = m_func;
            args[1] = m_arg1;
            args[2] = m_arg2;
            System::String^ result = System::String::Format("(FastApp2 {0} {1} {2})", args);
            System::GC::KeepAlive(this);
            return result;
        }
    };

    [Quire::Mirror::MirrorOfAttribute("mslisp.FastApp3, mslisp")]
    ref class FastApp3 : FastApp
    {
    private:
        Expression^ m_arg1;
        Expression^ m_arg2;
        Expression^ m_arg3;
        Func3^ m_func;
    public:
        FastApp3(Func3^ f, Expression^ arg1, Expression^ arg2, Expression^ arg3)
            : m_arg1(arg1), m_arg2(arg2), m_arg3(arg3), m_func(f) {}
        virtual System::Object^ execute(Env^ e, int trace_level) override
        {
            System::Object^ a1 = m_arg1->execute(e);
            System::Object^ a2 = m_arg2->execute(e);
            System::Object^ a3 = m_arg3->execute(e);
            System::Object^ result = m_func->Invoke(a1, a2, a3);
            System::GC::KeepAlive(this);
            return result;
        }
        virtual System::String^ ToString() override
        {
            array<System::Object^>^ args = gcnew cli::array<System::Object^>(4);
            args[0] = m_func;
            args[1] = m_arg1;
            args[2] = m_arg2;
            args[3] = m_arg3;
            System::String^ result = System::String::Format("(FastApp3 {0} {1} {2} {3})", args);
            System::GC::KeepAlive(this);
            return result;
        }
    };

    [Quire::Mirror::MirrorOfAttribute("mslisp.FastApp4, mslisp")]
    ref class FastApp4 : FastApp
    {
    private:
        Expression^ m_arg1;
        Expression^ m_arg2;
        Expression^ m_arg3;
        Expression^ m_arg4;
        Func4^ m_func;
    public:
        FastApp4(Func4^ f, Expression^ arg1, Expression^ arg2, Expression^ arg3, Expression^ arg4)
            : m_arg1(arg1), m_arg2(arg2), m_arg3(arg3), m_arg4(arg4), m_func(f) {}
        virtual System::Object^ execute(Env^ e, int trace_level) override
        {
            System::Object^ a1 = m_arg1->execute(e);
            System::Object^ a2 = m_arg2->execute(e);
            System::Object^ a3 = m_arg3->execute(e);
            System::Object^ a4 = m_arg4->execute(e);
            System::Object^ result = m_func->Invoke(a1, a2, a3, a4);
            System::GC::KeepAlive(this);
            return result;
        }
        virtual System::String^ ToString() override
        {
            array<System::Object^>^ args = gcnew cli::array<System::Object^>(5);
            args[0] = m_func;
            args[1] = m_arg1;
            args[2] = m_arg2;
            args[3] = m_arg3;
            args[4] = m_arg4;
            System::String^ result = System::String::Format("(FastApp4 {0} {1} {2} {3} {4})", args);
            System::GC::KeepAlive(this);
            return result;
        }
    };

    [Quire::Mirror::MirrorOfAttribute("mslisp.FastApp5, mslisp")]
    ref class FastApp5 : FastApp
    {
    private:
        Expression^ m_arg1;
        Expression^ m_arg2;
        Expression^ m_arg3;
        Expression^ m_arg4;
        Expression^ m_arg5;
        Func5^ m_func;
    public:
        FastApp5(Func5^ f, Expression^ arg1, Expression^ arg2, Expression^ arg3, Expression^ arg4, Expression^ arg5)
            : m_arg1(arg1), m_arg2(arg2), m_arg3(arg3), m_arg4(arg4), m_arg5(arg5), m_func(f) {}
        virtual System::Object^ execute(Env^ e, int trace_level) override
        {
            System::Object^ a1 = m_arg1->execute(e);
            System::Object^ a2 = m_arg2->execute(e);
            System::Object^ a3 = m_arg3->execute(e);
            System::Object^ a4 = m_arg4->execute(e);
            System::Object^ a5 = m_arg5->execute(e);
            System::Object^ result = m_func->Invoke(a1, a2, a3, a4, a5);
            System::GC::KeepAlive(this);
            return result;
        }
        virtual System::String^ ToString() override
        {
            array<System::Object^>^ args = gcnew cli::array<System::Object^>(6);
            args[0] = m_func;
            args[1] = m_arg1;
            args[2] = m_arg2;
            args[3] = m_arg3;
            args[4] = m_arg4;
            args[5] = m_arg5;
            System::String^ result = System::String::Format("(FastApp5 {0} {1} {2} {3} {4} {5})", args);
            System::GC::KeepAlive(this);
            return result;
        }
    };

    [Quire::Mirror::MirrorOfAttribute("mslisp.FastApp6, mslisp")]
    ref class FastApp6 : FastApp
    {
    private:
        Expression^ m_arg1;
        Expression^ m_arg2;
        Expression^ m_arg3;
        Expression^ m_arg4;
        Expression^ m_arg5;
        Expression^ m_arg6;
        Func6^ m_func;
    public:
        FastApp6(Func6^ f, Expression^ arg1, Expression^ arg2, Expression^ arg3, Expression^ arg4, Expression^ arg5, Expression^ arg6)
            : m_arg1(arg1), m_arg2(arg2), m_arg3(arg3), m_arg4(arg4), m_arg5(arg5), m_arg6(arg6), m_func(f) {}
        virtual System::Object^ execute(Env^ e, int trace_level) override
        {
            System::Object^ a1 = m_arg1->execute(e);
            System::Object^ a2 = m_arg2->execute(e);
            System::Object^ a3 = m_arg3->execute(e);
            System::Object^ a4 = m_arg4->execute(e);
            System::Object^ a5 = m_arg5->execute(e);
            System::Object^ a6 = m_arg6->execute(e);
            System::Object^ result = m_func->Invoke(a1, a2, a3, a4, a5, a6);
            System::GC::KeepAlive(this);
            return result;
        }
        virtual System::String^ ToString() override
        {
            array<System::Object^>^ args = gcnew cli::array<System::Object^>(7);
            args[0] = m_func;
            args[1] = m_arg1;
            args[2] = m_arg2;
            args[3] = m_arg3;
            args[4] = m_arg4;
            args[5] = m_arg5;
            args[6] = m_arg6;
            System::String^ result = System::String::Format("(FastApp5 {0} {1} {2} {3} {4} {5} {6})", args);
            System::GC::KeepAlive(this);
            return result;
        }
    };

    [System::SerializableAttribute]
    [Quire::Mirror::MirrorOfAttribute("mslisp.ApplyFunction, mslisp")]
    ref class ApplyFunction : Quire::utl_mgd::FunctionBase
    {
    public:
        ApplyFunction() {}
        virtual System::Object^ Invoke(array<System::Object^>^ args) override
        {
            Quire::utl_mgd::Function^ function = dynamic_cast<Quire::utl_mgd::Function^>(args[0]);
            if (function == nullptr)
            {
                ::mslisp::Error::report("Cannot apply: ", args[0]);
            }
            int num = args->Length;
            int num2 = num - 1;
            Quire::Engine::PCons^ pCons = dynamic_cast<Quire::Engine::PCons^>(args[num2]);
            array<System::Object^>^ array = gcnew cli::array<System::Object^>(Quire::Engine::PCons::length(pCons) + (num - 2));
            int num3 = 0;
            int num4 = 1;
            if (1 < num - 1)
            {
                do
                {
                    array[num3] = args[num4];
                    num3++;
                    num4++;
                }
                while (num4 < num - 1);
            }
            if (pCons != nullptr)
            {
                do
                {
                    array[num3] = pCons->hd;
                    num3++;
                    pCons = pCons->tl;
                }
                while (pCons != nullptr);
            }
            System::Object^ result = function->Invoke(array);
            System::GC::KeepAlive(this);
            return result;
        }
    };

    [System::SerializableAttribute]
    [Quire::Mirror::MirrorOfAttribute("mslisp.PlusFunction, mslisp")]
    ref class PlusFunction : Quire::utl_mgd::FunctionBase
    {
    private:
        int m_multiplier;

        int isum(array<System::Object^>^ args, bool^ ok)
        {
            int num = 0;
            int num2 = 1;
            int num3 = 0;
            int num4 = args->Length;
            if (0 < num4)
            {
                do
                {
                    int^ i = dynamic_cast<int^>(args[num3]);
                    if (i != nullptr)
                    {
                        num += (*i) * num2;
                        num2 = m_multiplier;
                        num3++;
                        continue;
                    }
                    *ok = false;
                    return 0;
                }
                while (num3 < num4);
            }
            *ok = true;
            return num;
        }

        double fsum(array<System::Object^>^ args)
        {
            double num = 0.0;
            double num2 = 1.0;
            int num3 = 0;
            int num4 = args->Length;
            if (0 < num4)
            {
                do
                {
                    double^ d = dynamic_cast<double^>(args[num3]);
                    if (d == nullptr)
                    {
                        int^ i = dynamic_cast<int^>(args[num3]);
                        if (i == nullptr)
                        {
                            ::mslisp::Error::report("Bad argument to +: ", args[num3]);
                            break;
                        }
                        num = (double)(*i) * num2 + num;
                    }
                    else
                    {
                        num = (*d) * num2 + num;
                    }
                    num2 = m_multiplier;
                    num3++;
                }
                while (num3 < num4);
            }
            return num;
        }

    public:
        PlusFunction(int multiplier)
        {
            m_multiplier = multiplier;
        }
        virtual System::Object^ Invoke(array<System::Object^>^ args) override
        {
            bool^ ok = false;
            int num = isum(args, ok);
            if (*ok)
            {
                return num;
            }
            return fsum(args);
        }
    };

    [Quire::Mirror::MirrorOfAttribute("mslisp.StandardBuiltins, mslisp")]
    public ref class StandardBuiltins
    {
    public:
        static System::Object^ True;

    private:
        static Quire::Engine::PCons^ maptree_accum1(
            System::Collections::Generic::Dictionary<System::Object^, bool>^ A_0,
            Quire::utl_mgd::Function^ A_1,
            Quire::Engine::PCons^ A_2,
            System::Object^ A_3)
        {
            System::Object^ obj = (A_3 == nullptr || dynamic_cast<Quire::Engine::PCons^>(A_3) == nullptr) ? True : nullptr;
            if (obj != nullptr)
            {
                array<System::Object^>^ args = gcnew cli::array<System::Object^>(1);
                args[0] = A_3;
                A_2 = gcnew Quire::Engine::PCons(A_1->Invoke(args), A_2);
            }
            else
            {
                Quire::Engine::PCons^ pCons = dynamic_cast<Quire::Engine::PCons^>(A_3);
                if (A_0->ContainsKey(pCons))
                {
                    A_2 = gcnew Quire::Engine::PCons("%circular", A_2);
                }
                else
                {
                    A_0[pCons] = true;
                    if (pCons != nullptr)
                    {
                        do
                        {
                            A_2 = maptree_accum1(A_0, A_1, A_2, pCons->hd);
                            pCons = pCons->tl;
                        }
                        while (pCons != nullptr);
                    }
                }
            }
            return A_2;
        }

        static double to_double(System::Object^ A_0)
        {
            double^ d = dynamic_cast<double^>(A_0);
            if (d != nullptr) return *d;
            int^ i = dynamic_cast<int^>(A_0);
            if (i != nullptr) return (double)*i;
            unsigned int^ u = dynamic_cast<unsigned int^>(A_0);
            if (u != nullptr) return (double)*u;
            ::mslisp::Error::report("Cannot convert to double: ", A_0);
            return 0.0;
        }

    public:
        static System::Object^ _car(System::Object^ o)
        {
            Quire::Engine::PCons^ pCons = dynamic_cast<Quire::Engine::PCons^>(o);
            return (pCons != nullptr) ? pCons->hd : nullptr;
        }

        static Quire::Engine::PCons^ _cdr(System::Object^ o)
        {
            Quire::Engine::PCons^ pCons = dynamic_cast<Quire::Engine::PCons^>(o);
            return (pCons != nullptr) ? pCons->tl : nullptr;
        }

        static Quire::Engine::PCons^ cons(System::Object^ o, System::Object^ l)
        {
            Quire::Engine::PCons^ pCons = dynamic_cast<Quire::Engine::PCons^>(l);
            System::Diagnostics::Trace::Assert(l == nullptr || pCons != nullptr, nullptr);
            return gcnew Quire::Engine::PCons(o, pCons);
        }

        static int length(System::Object^ o)
        {
            if (o == nullptr) return 0;
            Quire::Engine::PCons^ pCons = dynamic_cast<Quire::Engine::PCons^>(o);
            if (pCons != nullptr)
            {
                int num = 0;
                do
                {
                    num++;
                    pCons = pCons->tl;
                }
                while (pCons != nullptr);
                return num;
            }
            ::mslisp::Error::report("Cannot take length of ", o);
            return 0;
        }

        static System::Object^ identity(System::Object^ o)
        {
            return o;
        }

        static Quire::Engine::PCons^ mapcar(System::Object^ f, System::Object^ l)
        {
            Quire::Engine::PCons^ pCons = dynamic_cast<Quire::Engine::PCons^>(l);
            System::Diagnostics::Trace::Assert(l == nullptr || pCons != nullptr, nullptr);
            Quire::utl_mgd::Function^ function = dynamic_cast<Quire::utl_mgd::Function^>(f);
            if (function == nullptr)
            {
                ::mslisp::Error::report("Bad first argument to mapcar: ", f);
            }
            Quire::Engine::PCons^ pCons2 = nullptr;
            array<System::Object^>^ array = gcnew cli::array<System::Object^>(1);
            if (pCons != nullptr)
            {
                do
                {
                    array[0] = pCons->hd;
                    pCons2 = gcnew Quire::Engine::PCons(function->Invoke(array), pCons2);
                    pCons = pCons->tl;
                }
                while (pCons != nullptr);
            }
            return Quire::Engine::PCons::nreverse(pCons2);
        }

        static System::Object^ every(System::Object^ f, System::Object^ l)
        {
            Quire::Engine::PCons^ pCons = dynamic_cast<Quire::Engine::PCons^>(l);
            System::Diagnostics::Trace::Assert(l == nullptr || pCons != nullptr, nullptr);
            Quire::utl_mgd::Function^ function = dynamic_cast<Quire::utl_mgd::Function^>(f);
            if (function == nullptr)
            {
                ::mslisp::Error::report("Bad first argument to every: ", f);
            }
            array<System::Object^>^ array = gcnew cli::array<System::Object^>(1);
            if (pCons != nullptr)
            {
                do
                {
                    array[0] = pCons->hd;
                    if (::mslisp::is_true(function->Invoke(array)))
                    {
                        pCons = pCons->tl;
                        continue;
                    }
                    return nullptr;
                }
                while (pCons != nullptr);
            }
            return True;
        }

        static System::Object^ some(System::Object^ f, System::Object^ l)
        {
            Quire::Engine::PCons^ pCons = dynamic_cast<Quire::Engine::PCons^>(l);
            System::Diagnostics::Trace::Assert(l == nullptr || pCons != nullptr, nullptr);
            Quire::utl_mgd::Function^ function = dynamic_cast<Quire::utl_mgd::Function^>(f);
            if (function == nullptr)
            {
                ::mslisp::Error::report("Bad first argument to some: ", f);
            }
            array<System::Object^>^ array = gcnew cli::array<System::Object^>(1);
            if (pCons != nullptr)
            {
                do
                {
                    array[0] = pCons->hd;
                    System::Object^ obj = function->Invoke(array);
                    if (!::mslisp::is_true(obj))
                    {
                        pCons = pCons->tl;
                        continue;
                    }
                    return obj;
                }
                while (pCons != nullptr);
            }
            return nullptr;
        }

        static Quire::Engine::PCons^ mapenum(System::Object^ f, System::Object^ o)
        {
            Quire::utl_mgd::Function^ function = dynamic_cast<Quire::utl_mgd::Function^>(f);
            if (function == nullptr)
            {
                ::mslisp::Error::report("Bad 1st argument to mapenum: ", f);
            }
            System::Collections::IEnumerator^ enumerator2;
            System::Collections::IEnumerator^ enumerator = dynamic_cast<System::Collections::IEnumerator^>(o);
            if (enumerator != nullptr)
            {
                enumerator2 = enumerator;
            }
            else
            {
                System::Collections::IEnumerable^ enumerable = dynamic_cast<System::Collections::IEnumerable^>(o);
                if (enumerable != nullptr)
                {
                    enumerator2 = enumerable->GetEnumerator();
                }
                else
                {
                    return nullptr;
                }
            }
            Quire::Engine::PCons^ pCons = nullptr;
            array<System::Object^>^ array = gcnew cli::array<System::Object^>(1);
            if (enumerator2->MoveNext())
            {
                do
                {
                    array[0] = enumerator2->Current;
                    pCons = gcnew Quire::Engine::PCons(function->Invoke(array), pCons);
                }
                while (enumerator2->MoveNext());
            }
            return Quire::Engine::PCons::nreverse(pCons);
        }

        static Quire::Engine::PCons^ mapenumby(System::Object^ n, System::Object^ f, System::Object^ o)
        {
            int^ val = dynamic_cast<int^>(n);
            if (val == nullptr)
            {
                ::mslisp::Error::report("Bad first argument to mapenumby: ", n);
            }
            Quire::utl_mgd::Function^ function = dynamic_cast<Quire::utl_mgd::Function^>(f);
            if (function == nullptr)
            {
                ::mslisp::Error::report("Bad 2nd argument to mapenumby: ", f);
            }
            System::Collections::IEnumerator^ enumerator2;
            System::Collections::IEnumerator^ enumerator = dynamic_cast<System::Collections::IEnumerator^>(o);
            if (enumerator != nullptr)
            {
                enumerator2 = enumerator;
            }
            else
            {
                System::Collections::IEnumerable^ enumerable = dynamic_cast<System::Collections::IEnumerable^>(o);
                if (enumerable != nullptr)
                {
                    enumerator2 = enumerable->GetEnumerator();
                }
                else
                {
                    return nullptr;
                }
            }
            Quire::Engine::PCons^ pCons = nullptr;
            int num = 0;
            array<System::Object^>^ array = gcnew cli::array<System::Object^>(*val);
            if (enumerator2->MoveNext())
            {
                do
                {
                    array[num] = enumerator2->Current;
                    num++;
                    if (num == *val)
                    {
                        pCons = gcnew Quire::Engine::PCons(function->Invoke(array), pCons);
                        num = 0;
                    }
                }
                while (enumerator2->MoveNext());
            }
            return Quire::Engine::PCons::nreverse(pCons);
        }

        static Quire::Engine::PCons^ maptree_accum(System::Object^ f, System::Object^ o)
        {
            Quire::utl_mgd::Function^ function = dynamic_cast<Quire::utl_mgd::Function^>(f);
            if (function == nullptr)
            {
                ::mslisp::Error::report("Bad 1st argument to maptree_accum: ", f);
            }
            return Quire::Engine::PCons::nreverse(maptree_accum1(gcnew System::Collections::Generic::Dictionary<System::Object^, bool>(), function, nullptr, o));
        }

        static Quire::Engine::PCons^ remove_if(System::Object^ f, System::Object^ l)
        {
            Quire::Engine::PCons^ pCons = dynamic_cast<Quire::Engine::PCons^>(l);
            System::Diagnostics::Trace::Assert(l == nullptr || pCons != nullptr, nullptr);
            Quire::utl_mgd::Function^ function = dynamic_cast<Quire::utl_mgd::Function^>(f);
            if (function == nullptr)
            {
                ::mslisp::Error::report("Bad first argument to remove-if: ", f);
            }
            Quire::Engine::PCons^ pCons2 = nullptr;
            array<System::Object^>^ array = gcnew cli::array<System::Object^>(1);
            if (pCons != nullptr)
            {
                do
                {
                    array[0] = pCons->hd;
                    if (!::mslisp::is_true(function->Invoke(array)))
                    {
                        pCons2 = gcnew Quire::Engine::PCons(pCons->hd, pCons2);
                    }
                    pCons = pCons->tl;
                }
                while (pCons != nullptr);
            }
            return Quire::Engine::PCons::nreverse(pCons2);
        }

        static System::Object^ nth(System::Object^ n, System::Object^ l)
        {
            Quire::Engine::PCons^ pCons = dynamic_cast<Quire::Engine::PCons^>(l);
            System::Diagnostics::Trace::Assert(l == nullptr || pCons != nullptr, nullptr);
            int^ val = dynamic_cast<int^>(n);
            if (val == nullptr)
            {
                ::mslisp::Error::report("Bad first argument to nth: ", n);
            }
            int num = 0;
            if (0 < *val)
            {
                do
                {
                    pCons = pCons->tl;
                    if (pCons != nullptr)
                    {
                        num++;
                        continue;
                    }
                    return nullptr;
                }
                while (num < *val);
            }
            return pCons->hd;
        }

        static System::Object^ last(System::Object^ l)
        {
            Quire::Engine::PCons^ pCons = dynamic_cast<Quire::Engine::PCons^>(l);
            System::Diagnostics::Trace::Assert(l == nullptr || pCons != nullptr, nullptr);
            while (pCons != nullptr)
            {
                Quire::Engine::PCons^ tl = pCons->tl;
                if (tl == nullptr)
                {
                    return pCons;
                }
                pCons = tl;
            }
            return pCons;
        }

        static System::Object^ butlast(System::Object^ l)
        {
            Quire::Engine::PCons^ pCons = dynamic_cast<Quire::Engine::PCons^>(l);
            System::Diagnostics::Trace::Assert(l == nullptr || pCons != nullptr, nullptr);
            Quire::Engine::PCons^ pCons2 = nullptr;
            if (pCons != nullptr)
            {
                do
                {
                    if (pCons->tl != nullptr)
                    {
                        pCons2 = gcnew Quire::Engine::PCons(pCons->hd, pCons2);
                    }
                    pCons = pCons->tl;
                }
                while (pCons != nullptr);
            }
            return Quire::Engine::PCons::nreverse(pCons2);
        }

        static System::Object^ eq(System::Object^ a, System::Object^ b)
        {
            return (a == b) ? True : nullptr;
        }

        static System::Object^ atom(System::Object^ a)
        {
            System::Object^ obj = (a == nullptr || dynamic_cast<Quire::Engine::PCons^>(a) == nullptr) ? True : nullptr;
            return obj;
        }

        static System::Object^ symbolp(System::Object^ a)
        {
            System::Object^ obj = (a == nullptr || dynamic_cast<Symbol^>(a) == nullptr) ? True : nullptr;
            return obj;
        }

        static System::Object^ symbol_name(System::Object^ a)
        {
            Symbol^ symbol = dynamic_cast<Symbol^>(a);
            if (symbol == nullptr)
            {
                ::mslisp::Error::report("Cannot take symbol-name of: ", a);
            }
            return symbol->get_name();
        }

        static System::Object^ error(System::Object^ a)
        {
            throw gcnew System::Exception(a->ToString());
        }

#pragma warning(push)
#pragma warning(disable: 4702)
        static System::Object^ quit()
        {
            exit(0);
            return nullptr;
        }
#pragma warning(pop)

        static System::Object^ _unsigned(System::Object^ n)
        {
            int^ val = dynamic_cast<int^>(n);
            if (val != nullptr)
            {
                return (unsigned int)*val;
            }
            return n;
        }

        static System::Object^ _gt(System::Object^ a, System::Object^ b)
        {
            return (to_double(a) > to_double(b)) ? True : nullptr;
        }

        static System::Object^ _lt(System::Object^ a, System::Object^ b)
        {
            return (to_double(a) < to_double(b)) ? True : nullptr;
        }

        static System::Object^ _eq(System::Object^ a, System::Object^ b)
        {
            return (to_double(a) == to_double(b)) ? True : nullptr;
        }

        static System::Object^ eqt(System::Object^ x, System::Object^ y)
        {
            TaggedSymbol^ taggedSymbol = dynamic_cast<TaggedSymbol^>(x);
            if (taggedSymbol != nullptr)
            {
                x = taggedSymbol->Base;
            }
            TaggedSymbol^ taggedSymbol2 = dynamic_cast<TaggedSymbol^>(y);
            if (taggedSymbol2 != nullptr)
            {
                y = taggedSymbol2->Base;
            }
            return (x == y) ? True : nullptr;
        }

        static System::Object^ no_tag(System::Object^ x)
        {
            TaggedSymbol^ taggedSymbol = dynamic_cast<TaggedSymbol^>(x);
            if (taggedSymbol != nullptr)
            {
                return gcnew TaggedSymbol(taggedSymbol->Base, nullptr, taggedSymbol->Labels);
            }
            return x;
        }

        static System::Object^ no_tag_or_labels(System::Object^ x)
        {
            TaggedSymbol^ taggedSymbol = dynamic_cast<TaggedSymbol^>(x);
            if (taggedSymbol != nullptr)
            {
                return taggedSymbol->Base;
            }
            return x;
        }

        static System::Object^ find_tag(System::Object^ x)
        {
            TaggedSymbol^ taggedSymbol = dynamic_cast<TaggedSymbol^>(x);
            if (taggedSymbol != nullptr)
            {
                return taggedSymbol->Tag;
            }
            Quire::Engine::PCons^ pCons = dynamic_cast<Quire::Engine::PCons^>(x);
            if (pCons != nullptr)
            {
                do
                {
                    System::Object^ obj = find_tag(pCons->hd);
                    if (obj == nullptr)
                    {
                        pCons = pCons->tl;
                        continue;
                    }
                    return obj;
                }
                while (pCons != nullptr);
            }
            return nullptr;
        }

        static System::Object^ find_tag_on(System::Object^ x, System::Object^ y)
        {
            TaggedSymbol^ taggedSymbol = dynamic_cast<TaggedSymbol^>(y);
            if (taggedSymbol != nullptr && taggedSymbol->Base == x)
            {
                return taggedSymbol->Tag;
            }
            Quire::Engine::PCons^ pCons = dynamic_cast<Quire::Engine::PCons^>(y);
            if (pCons != nullptr)
            {
                do
                {
                    System::Object^ obj = find_tag_on(x, pCons->hd);
                    if (obj == nullptr)
                    {
                        pCons = pCons->tl;
                        continue;
                    }
                    return obj;
                }
                while (pCons != nullptr);
            }
            return nullptr;
        }

        static System::Object^ set_tag(System::Object^ x, System::Object^ tag)
        {
            Symbol^ symbol = dynamic_cast<Symbol^>(x);
            Symbol^ symbol2 = dynamic_cast<Symbol^>(tag);
            if (symbol != nullptr && symbol2 != nullptr)
            {
                Symbol^ symbol3 = symbol;
                TaggedSymbol^ taggedSymbol = dynamic_cast<TaggedSymbol^>(symbol);
                if (taggedSymbol != nullptr)
                {
                    symbol3 = taggedSymbol->Base;
                }
                x = gcnew TaggedSymbol(symbol3, symbol2, symbol->Labels);
            }
            return x;
        }

        static System::Object^ _append(System::Object^ l)
        {
            Quire::Engine::PCons^ pCons = dynamic_cast<Quire::Engine::PCons^>(l);
            System::Diagnostics::Trace::Assert(l == nullptr || pCons != nullptr, nullptr);
            Quire::Engine::PCons^ pCons2 = nullptr;
            if (pCons != nullptr)
            {
                do
                {
                    Quire::Engine::PCons^ pCons3 = dynamic_cast<Quire::Engine::PCons^>(pCons->hd);
                    if (pCons3 != nullptr)
                    {
                        do
                        {
                            pCons2 = gcnew Quire::Engine::PCons(pCons3->hd, pCons2);
                            pCons3 = pCons3->tl;
                        }
                        while (pCons3 != nullptr);
                    }
                    pCons = pCons->tl;
                }
                while (pCons != nullptr);
            }
            return Quire::Engine::PCons::nreverse(pCons2);
        }

        static System::Object^ _clr_vec(System::Object^ ll)
        {
            Quire::Engine::PCons^ pCons = dynamic_cast<Quire::Engine::PCons^>(ll);
            System::Diagnostics::Trace::Assert(ll == nullptr || pCons != nullptr, nullptr);
            array<System::Object^>^ array = nullptr;
            int num = Quire::Engine::PCons::length(pCons);
            if (num > 0)
            {
                array = gcnew cli::array<System::Object^>(num);
                for (int i = 0; i < num; i++)
                {
                    array[i] = pCons->hd;
                    pCons = pCons->tl;
                }
            }
            return array;
        }

        static System::Object^ print(System::Object^ a)
        {
            System::Console::WriteLine(Quire::Engine::PCons::_ToString(a));
            return a;
        }

        static System::Object^ printn(System::Object^ a)
        {
            System::Console::Write(Quire::Engine::PCons::_ToString(a));
            return a;
        }

        static System::Object^ debug_out(System::Object^ a)
        {
            System::Diagnostics::Trace::Write(Quire::Engine::PCons::_ToString(a));
            return a;
        }

        static System::Object^ pass()
        {
            return nullptr;
        }

        static System::Object^ pquote(System::Object^ a)
        {
            System::String^ text = "\"";
            return System::String::Concat(text + a->ToString(), text);
        }

        static System::Collections::IDictionary^ make_hash_table()
        {
            return gcnew System::Collections::Hashtable();
        }

        static System::Collections::IDictionary^ make_hash_table1(System::Object^ size)
        {
            int^ val = dynamic_cast<int^>(size);
            System::Diagnostics::Trace::Assert(val != nullptr, nullptr);
            return gcnew System::Collections::Hashtable(*val);
        }

        static System::Object^ set_hash(System::Object^ h, System::Object^ k, System::Object^ v)
        {
            System::Collections::IDictionary^ dictionary = dynamic_cast<System::Collections::IDictionary^>(h);
            System::Diagnostics::Trace::Assert(dictionary != nullptr, nullptr);
            dictionary[k] = v;
            return v;
        }

        static System::Object^ get_hash(System::Object^ h, System::Object^ k)
        {
            System::Collections::IDictionary^ dictionary = dynamic_cast<System::Collections::IDictionary^>(h);
            System::Diagnostics::Trace::Assert(dictionary != nullptr, nullptr);
            return dictionary[k];
        }

        static System::Object^ remove_hash(System::Object^ h, System::Object^ k)
        {
            System::Collections::IDictionary^ dictionary = dynamic_cast<System::Collections::IDictionary^>(h);
            System::Diagnostics::Trace::Assert(dictionary != nullptr, nullptr);
            dictionary->Remove(k);
            return dictionary;
        }

        static System::Object^ contains_hash(System::Object^ h, System::Object^ k)
        {
            System::Collections::IDictionary^ dictionary = dynamic_cast<System::Collections::IDictionary^>(h);
            System::Diagnostics::Trace::Assert(dictionary != nullptr, nullptr);
            return dictionary->Contains(k) ? True : nullptr;
        }

        static System::Object^ comparer(System::Object^ A_0)
        {
            Quire::utl_mgd::Function^ function = dynamic_cast<Quire::utl_mgd::Function^>(A_0);
            System::Diagnostics::Trace::Assert(function != nullptr, nullptr);
            return gcnew _fcomparer(function);
        }

        StandardBuiltins() {}
    private:
        static StandardBuiltins()
        {
            True = true;
        }
    };

}}
