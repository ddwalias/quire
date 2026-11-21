using System;
using System.Runtime.InteropServices;
using Quire.Automation;
using Quire.Mcd;

namespace Quire.Components;

public static partial class Efi
{
    public static object SendMessage(int info, int msg, object[] inputs)
    {
        Utilities.CALLBACK_INFO cALLBACK_INFO = (Utilities.CALLBACK_INFO)Marshal.PtrToStructure(new IntPtr(info), typeof(Utilities.CALLBACK_INFO));
        if (cALLBACK_INFO != null)
        {
            Utilities.CALLBACK_MSG cALLBACK_MSG = new Utilities.CALLBACK_MSG();
            cALLBACK_MSG.msg = msg;
            cALLBACK_MSG.handle = cALLBACK_INFO.handle;
            GC.KeepAlive(inputs);
            Utilities.SendMessage(cALLBACK_INFO.hwnd, 1525, inputs, cALLBACK_MSG);
        }
        return inputs;
    }

    public static object EvalComponent(string cwd, int origin, int comp, object[] inputs)
    {
        IMcxComponent mcxComponent = (IMcxComponent)Utilities.Unwrap(comp);
        try
        {
            Utilities.SetProperty(mcxComponent, "Origin", origin);
            Utilities.SetProperty(mcxComponent, "CWD", cwd);
            if (inputs == null)
            {
                inputs = new object[0];
            }
            for (int i = 0; i < inputs.Length; i++)
            {
                inputs[i] = ValueFactory.Instance.EngineToAutomation(inputs[i]);
            }
            object[] array = mcxComponent.Eval(inputs, null);
            if (array == null)
            {
                return null;
            }
            for (int j = 0; j < array.Length; j++)
            {
                array[j] = ValueFactory.Instance.AutomationToEngine(array[j]);
            }
            if (array.Length == 1)
            {
                return array[0];
            }
            return new MatVal(array);
        }
        catch (InvalidCastException)
        {
            throw (Exception)Utilities.GetProperty(mcxComponent, "LastError");
        }
        finally
        {
            Utilities.Release(mcxComponent);
        }
    }
}
